using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TerraformPluginDotNet;
using TerraformPluginDotNet.ResourceProvider;
using TerraformPluginDotNet.Testing;
using static Tfplugin5.Provider;

namespace DataSourceProvider.Test;

[TestFixture(Category = "Functional", Explicit = true)]
public class SampleProviderTest
{
    private const string ProviderName = "datasourceprovider";

    private TerraformTestHost _host;

    [OneTimeSetUp]
    public void Setup()
    {
        _host = new TerraformTestHost(Environment.GetEnvironmentVariable("TF_PLUGIN_DOTNET_TEST_TF_BIN"));
        _host.Start($"example.com/example/{ProviderName}", Configure);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _host.DisposeAsync();
    }

    private void Configure(IServiceCollection services, IResourceRegistryContext registryContext)
    {
        services.AddSingleton<SampleConfigurator>();
        services.AddTerraformProviderConfigurator<Configuration, SampleConfigurator>();
        services.AddSingleton<IDataSourceProvider<SampleDataSource>, SampleDataSourceProvider>();
        registryContext.RegisterDataSource<SampleDataSource>($"{ProviderName}_data");
    }

    [Test]
    public async Task TestReadDataSource()
    {
        using var terraform = await _host.CreateTerraformTestInstanceAsync(ProviderName);

        var resourcePath = Path.Combine(terraform.WorkDir, "file.tf");

        await File.WriteAllTextAsync(resourcePath, $@"
data ""{ProviderName}_data"" ""demo_data"" {{
id = ""test""
}}
");

        var output = await terraform.PlanWithOutputAsync();

        Assert.That(output.PriorState.Values.RootModule.Resources[0].Values.GetProperty("data").GetString(), Is.EqualTo("No dummy data configured"));
    }

    [Test]
    public async Task TestConfigureDataSource()
    {
        using var terraform = await _host.CreateTerraformTestInstanceAsync(ProviderName, configureProvider: false);

        var resourcePath = Path.Combine(terraform.WorkDir, "file.tf");

        await File.WriteAllTextAsync(resourcePath, $@"
provider ""{ProviderName}"" {{
    dummy_data = ""This is dummy data""
}}

data ""{ProviderName}_data"" ""demo_data"" {{
id = ""test""
}}

");

        var output = await terraform.PlanWithOutputAsync();

        Assert.That(output.PriorState.Values.RootModule.Resources[0].Values.GetProperty("data").GetString(), Is.EqualTo("This is dummy data"));
    }
}
