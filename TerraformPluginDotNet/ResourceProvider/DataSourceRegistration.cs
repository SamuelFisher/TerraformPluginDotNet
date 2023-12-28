using Tfplugin5;

namespace TerraformPluginDotNet.ResourceProvider;

public record DataSourceRegistration(string ResourceName, Type Type, Schema Schema);
