using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraformPluginDotNet.ResourceProvider;

public interface IResourceUpgrader<T>
{
    Task<T> UpgradeResourceStateAsync(long schemaVersion, ReadOnlyMemory<byte> json);
}
