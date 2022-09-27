namespace TerraformPluginDotNet.Resources;

/// <summary>
/// Indicates that a value is required.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class RequiredAttribute : Attribute
{
}
