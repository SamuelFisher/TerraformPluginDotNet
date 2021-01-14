using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraformPluginDotNet.Serialization;

namespace TerraformPluginDotNet.ResourceProvider
{
    class DefaultResourceUpgrader<T> : IResourceUpgrader<T>
    {
        private readonly IDynamicValueSerializer _serializer;

        public DefaultResourceUpgrader(IDynamicValueSerializer serializer)
        {
            _serializer = serializer;
        }

        public Task<T> UpgradeResourceStateAsync(long schemaVersion, ReadOnlyMemory<byte> json)
        {
            return Task.FromResult(_serializer.DeserializeJson<T>(json));
        }
    }
}
