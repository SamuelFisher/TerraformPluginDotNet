using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TerraformPluginDotNet.Testing;

class TerraformTestInstance : ITerraformTestInstance
{
    private readonly string _terraformBin;
    private readonly string _providerName;
    private readonly int _port;

    public TerraformTestInstance(string terraformBin, string providerName, int port, string workDir)
    {
        _terraformBin = terraformBin;
        _providerName = providerName;
        _port = port;
        WorkDir = workDir;
    }

    public string WorkDir { get; private set; }

    public TimeSpan DefaultCommandTimeout { get; set; } = TimeSpan.FromMinutes(1);

    public async Task<string> RunCommandAsync(string command, TimeSpan? timeout = null)
    {
        timeout ??= DefaultCommandTimeout;

        var startInfo = new ProcessStartInfo(_terraformBin, command)
        {
            WorkingDirectory = WorkDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        startInfo.EnvironmentVariables.Add("TF_REATTACH_PROVIDERS", $@"{{""example.com/example/{_providerName}"":{{""Protocol"":""grpc"",""Pid"":{Environment.ProcessId},""Test"":true,""Addr"":{{""Network"":""tcp"",""String"":""127.0.0.1:{_port}""}}}}}}");
        var p = Process.Start(startInfo);

        var output = new StringBuilder();
        p.OutputDataReceived += (sender, e) => output.AppendLine(e.Data);
        p.ErrorDataReceived += (sender, e) => output.AppendLine(e.Data);
        p.BeginOutputReadLine();
        p.BeginErrorReadLine();

        var timeoutCts = new CancellationTokenSource();
        timeoutCts.CancelAfter(timeout.Value);

        try
        {
            await p.WaitForExitAsync(timeoutCts.Token);
        }
        catch (TaskCanceledException)
        {
            p.Kill();
            await p.WaitForExitAsync();
            throw new TerraformCommandException(command, timeout, output.ToString());
        }

        if (p.ExitCode != 0)
        {
            throw new TerraformCommandException(command, p.ExitCode, output.ToString());
        }

        return output.ToString();
    }

    public void Dispose()
    {
        Directory.Delete(WorkDir, true);
    }
}
