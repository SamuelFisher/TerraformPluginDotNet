using System;
using Tfplugin5;

namespace TerraformPluginDotNet.ProviderConfig
{
    record ProviderConfigurationRegistry
    {
        public Schema ConfigurationSchema { get; init; }

        public Type ConfigurationType { get; init; }
    }
}
