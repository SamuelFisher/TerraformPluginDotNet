using System;
using System.IO;
using System.Linq;
using System.Text.Json;
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

        await File.WriteAllTextAsync(resourcePath, """
            resource "test_resource" "test" {
              required_string = "value"
              required_int = 1
              int_attribute = 1
              boolean_attribute = true
              float_attribute = 1.0
              double_attribute = 1.0
              string_list_attribute = ["one", "two", "three"]
              int_list_attribute = [1, 2, 3]
              string_map_attribute = {
                a = "one",
                b = "two",
                c = "three"
              }
              int_map_attribute = {
                a = 1,
                b = 2,
                c = 3
              }
              object_1 = {
                required_string = "value"
                nested_int = 1
              }
              object_2 = {
                required_string = "value"
              }
            }
            """);

        var plan = await terraform.PlanWithOutputAsync();

        Assert.That(plan.ResourceChanges, Has.Count.EqualTo(1));
        Assert.That(plan.ResourceChanges.Single().Change.Actions.Single(), Is.EqualTo("create"));
        Assert.That(plan.ResourceChanges.Single().Change.Before.ValueKind, Is.EqualTo(JsonValueKind.Null));

        var after = JsonSerializer.Serialize(plan.ResourceChanges.Single().Change.After, new JsonSerializerOptions() { WriteIndented = true });
        var expected = """
            {
              "boolean_attribute": true,
              "double_attribute": 1,
              "float_attribute": 1,
              "int_attribute": 1,
              "int_list_attribute": [
                1,
                2,
                3
              ],
              "int_map_attribute": {
                "a": 1,
                "b": 2,
                "c": 3
              },
              "object_1": {
                "nested_int": 1,
                "required_string": "value"
              },
              "object_2": {
                "required_string": "value"
              },
              "required_int": 1,
              "required_string": "value",
              "string_list_attribute": [
                "one",
                "two",
                "three"
              ],
              "string_map_attribute": {
                "a": "one",
                "b": "two",
                "c": "three"
              }
            }
            """;

        Assert.That(after, Is.EqualTo(expected));
    }

    [Test]
    public async Task TestPlanCreateOnlyRequiredFields()
    {
        using var terraform = await _host.CreateTerraformTestInstanceAsync(ProviderName);

        var resourcePath = Path.Combine(terraform.WorkDir, "file.tf");

        await File.WriteAllTextAsync(resourcePath, """
            resource "test_resource" "test" {
              required_string = "value"
              required_int = 1
              object_1 = {
                required_string = "test"
              }
              object_2 = {
                required_string = "test"
              }
            }
            """);

        var plan = await terraform.PlanWithOutputAsync();

        Assert.That(plan.ResourceChanges, Has.Count.EqualTo(1));
        Assert.That(plan.ResourceChanges.Single().Change.Actions.Single(), Is.EqualTo("create"));
        Assert.That(plan.ResourceChanges.Single().Change.Before.ValueKind, Is.EqualTo(JsonValueKind.Null));

        var after = JsonSerializer.Serialize(plan.ResourceChanges.Single().Change.After, new JsonSerializerOptions() { WriteIndented = true });
        var expected = """
            {
              "boolean_attribute": null,
              "double_attribute": null,
              "float_attribute": null,
              "int_attribute": null,
              "int_list_attribute": null,
              "int_map_attribute": null,
              "object_1": {
                "nested_int": null,
                "required_string": "test"
              },
              "object_2": {
                "required_string": "test"
              },
              "required_int": 1,
              "required_string": "value",
              "string_list_attribute": null,
              "string_map_attribute": null
            }
            """;

        Assert.That(after, Is.EqualTo(expected));
    }
}
