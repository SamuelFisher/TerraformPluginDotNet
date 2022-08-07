using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TerraformPluginDotNet.ResourceProvider;
using TerraformPluginDotNet.Testing;

namespace SchemaUpgrade.Test;

[TestFixture(Category = "Functional", Explicit = true)]
public class UpgradableResourceProviderTest
{
    private const string ProviderName = "schemaupgrade";

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
        services.AddSingleton<IResourceProvider<UpgradableResourceV2>, UpgradableResourceProvider>();
        registryContext.RegisterResource<UpgradableResourceV2>($"{ProviderName}_sampleresource");
        services.AddTransient<IResourceUpgrader<UpgradableResourceV2>, UpgradableResourceUpgrader>();
    }

    [Test]
    public async Task TestUpdateOlderResourceVersion()
    {
        using var terraform = await _host.CreateTerraformTestInstanceAsync(ProviderName);

        var resourcePath = Path.Combine(terraform.WorkDir, "file.tf");

        await File.WriteAllTextAsync(resourcePath, $@"
resource ""{ProviderName}_sampleresource"" ""item1"" {{
  data = ""some value""
}}
");

        var tfstate = Assembly.GetExecutingAssembly().GetManifestResourceStream("SchemaUpgrade.Test.terraform.tfstate");
        using (var tfStateFile = File.Create(Path.Combine(terraform.WorkDir, "terraform.tfstate")))
        {
            await tfstate.CopyToAsync(tfStateFile);
        }

        var plan = await terraform.PlanWithOutputAsync();

        // State should be upgraded, but there are no changes to apply.
        var resourceChange = plan.ResourceChanges.Single().Change;
        Assert.That(resourceChange.Actions.Single(), Is.EqualTo("no-op"));
        Assert.That(resourceChange.After.GetProperty("data").GetString(), Is.EqualTo(resourceChange.Before.GetProperty("data").GetString()));
    }
}
