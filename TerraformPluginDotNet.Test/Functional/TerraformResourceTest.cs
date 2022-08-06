using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TerraformPluginDotNet.ResourceProvider;
using TerraformPluginDotNet.Testing;

namespace TerraformPluginDotNet.Test.Functional;

[TestFixture(Category = "Functional", Explicit = true)]
public class TerraformResourceTest
{
    private const string ProviderName = "test";

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
        services.AddSingleton<IResourceProvider<TestResource>, TestResourceProvider>();
        registryContext.RegisterResource<TestResource>($"{ProviderName}_resource");
    }

    [Test]
    public async Task TestPlanCreateAllFields()
    {
        using var terraform = await _host.CreateTerraformTestInstanceAsync(ProviderName);

        var resourcePath = Path.Combine(terraform.WorkDir, "file.tf");

        await File.WriteAllTextAsync(resourcePath, $@"
resource ""test_resource"" ""test"" {{
  required_attribute = ""value""
  int_attribute = 1
  boolean_attribute = true
  float_attribute = 1.0
  double_attribute = 1.0
}}
");

        await terraform.PlanAsync();
    }
}
