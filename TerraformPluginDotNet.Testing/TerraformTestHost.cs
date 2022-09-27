using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TerraformPluginDotNet.ResourceProvider;

namespace TerraformPluginDotNet.Testing;

public class TerraformTestHost : IAsyncDisposable
{
    private readonly string _terraformBin;
    private readonly int _port;

    private readonly CancellationTokenSource _cancelHost;
    private Task _host;

    public TerraformTestHost(string terraformBin, int? port = default)
    {
        port ??= FindFreePort();

        if (string.IsNullOrEmpty(terraformBin) || !File.Exists(terraformBin))
        {
            throw new ArgumentException($"Terraform binary not found at '{terraformBin}'.", nameof(terraformBin));
        }

        _cancelHost = new CancellationTokenSource();
        _terraformBin = terraformBin;
        _port = port.Value;
    }

    public void Start(string fullProviderName, Action<IServiceCollection, IResourceRegistryContext> configure)
    {
        if (_host != null)
        {
            throw new InvalidOperationException("Host has already been started.");
        }

        _host = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(x => x.ConfigureTerraformPlugin(configure, _port))
            .ConfigureServices(x => x.Configure<TerraformPluginHostOptions>(x =>
            {
                x.FullProviderName = fullProviderName;
                x.DebugMode = true;
            }))
            .Build()
            .RunAsync(_cancelHost.Token);
    }

    public async Task<ITerraformTestInstance> CreateTerraformTestInstanceAsync(string providerName, bool configureProvider = true, bool configureTerraform = true)
    {
        var workDir = Path.Combine(Path.GetTempPath(), $"TerraformPluginDotNet_{Guid.NewGuid()}");
        Directory.CreateDirectory(workDir);

        var terraform = new TerraformTestInstance(_terraformBin, providerName, _port, workDir);

        if (configureProvider || configureTerraform)
        {
            var configFile = new StringBuilder();

            if (configureProvider)
            {
                configFile.Append($@"
provider ""{providerName}"" {{}}
");
            }

            if (configureTerraform)
            {
                configFile.Append($@"
terraform {{
  required_providers {{
    {providerName} = {{
      source = ""example.com/example/{providerName}""
      version = ""1.0.0""
    }}
  }}
}}
");
            }

            await File.WriteAllTextAsync(workDir + "/conf.tf", configFile.ToString());

            await terraform.InitAsync();
        }

        return terraform;
    }

    public async ValueTask DisposeAsync()
    {
        if (_host == null)
        {
            return;
        }

        _cancelHost.Cancel();
        await _host;
    }

    private static int FindFreePort()
    {
        var port = 0;
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            var endPoint = new IPEndPoint(IPAddress.Loopback, 0);
            socket.Bind(endPoint);
            endPoint = (IPEndPoint)socket.LocalEndPoint;
            port = endPoint.Port;
        }
        finally
        {
            socket.Close();
        }

        return port;
    }
}
