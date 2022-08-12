using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TerraformPluginDotNet.ResourceProvider;

namespace SchemaUpgrade;

public class UpgradableResourceProvider : IResourceProvider<UpgradableResourceV2>
{
    public Task<UpgradableResourceV2> PlanAsync(UpgradableResourceV2 prior, UpgradableResourceV2 proposed)
    {
        return Task.FromResult(proposed);
    }

    public Task<UpgradableResourceV2> CreateAsync(UpgradableResourceV2 planned)
    {
        planned.Id = Guid.NewGuid().ToString();

        // Do nothing
        return Task.FromResult(planned);
    }

    public Task DeleteAsync(UpgradableResourceV2 resource)
    {
        // Do nothing
        return Task.CompletedTask;
    }

    public Task<UpgradableResourceV2> ReadAsync(UpgradableResourceV2 resource)
    {
        // Do nothing
        return Task.FromResult(resource);
    }

    public Task<UpgradableResourceV2> UpdateAsync(UpgradableResourceV2 prior, UpgradableResourceV2 planned)
    {
        // Do nothing
        return Task.FromResult(planned);
    }

    public Task<IList<UpgradableResourceV2>> ImportAsync(string id)
    {
        throw new NotSupportedException();
    }
}
