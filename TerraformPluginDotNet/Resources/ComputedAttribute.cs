namespace TerraformPluginDotNet.Resources;

/// <summary>
/// Indicates that a value is "known after apply".
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ComputedAttribute : Attribute
{
}
