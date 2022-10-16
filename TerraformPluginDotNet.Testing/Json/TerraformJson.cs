using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TerraformPluginDotNet.Testing.Json;

// See https://www.terraform.io/internals/json-format#plan-representation
public record TerraformJsonPlan
{
    [JsonPropertyName("format_version")]
    public string FormatVersion { get; init; }

    [JsonPropertyName("terraform_version")]
    public string TerraformVersion { get; init; }

    [JsonPropertyName("resource_changes")]
    public ImmutableList<TerraformJsonResourceChange> ResourceChanges { get; init; }

    [JsonPropertyName("prior_state")]
    public PriorState PriorState { get; init; }
}

public record TerraformJsonResourceChange
{
    [JsonPropertyName("address")]
    public string Address { get; init; }

    [JsonPropertyName("previous_address")]
    public string PreviousAddress { get; init; }

    [JsonPropertyName("module_address")]
    public string ModuleAddress { get; init; }

    [JsonPropertyName("mode")]
    public string Mode { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("index")]
    public int Index { get; init; }

    [JsonPropertyName("deposed")]
    public string Deposed { get; init; }

    [JsonPropertyName("change")]
    public TerraformJsonChange Change { get; init; }

    [JsonPropertyName("action_reason")]
    public string ActionReason { get; init; }
}

// See https://www.terraform.io/internals/json-format#change-representation
public record TerraformJsonChange
{
    [JsonPropertyName("actions")]
    public ImmutableList<string> Actions { get; init; }

    [JsonPropertyName("before")]
    public JsonElement Before { get; init; }

    [JsonPropertyName("after")]
    public JsonElement After { get; init; }
}

public record PriorState
{
    [JsonPropertyName("format_version")]
    public string FormatVersion { get; init; }

    [JsonPropertyName("terraform_version")]
    public string TerraformVersion { get; init; }

    [JsonPropertyName("values")]
    public StateValues Values { get; init; }
}

public class StateValues
{
    [JsonPropertyName("root_module")]
    public Module RootModule { get; init; }
}

public class Module
{
    [JsonPropertyName("resources")]
    public ImmutableList<Resource> Resources { get; init; }
}

public class Resource
{
    [JsonPropertyName("address")]
    public string Address { get; init; }

    [JsonPropertyName("mode")]
    public string Mode { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("provider_name")]
    public string ProviderName { get; init; }

    [JsonPropertyName("schema_version")]
    public int SchemaVersion { get; init; }

    [JsonPropertyName("values")]
    public JsonElement Values { get; init; }

    [JsonPropertyName("sensitive_values")]
    public JsonElement SensitiveValues { get; init; }
}
