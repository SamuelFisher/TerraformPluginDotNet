using Tfplugin5;

namespace TerraformPluginDotNet.ProviderConfig;

record ProviderConfigurationRegistry(
    Schema ConfigurationSchema,
    Type ConfigurationType);
