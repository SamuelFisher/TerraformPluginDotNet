using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TerraformPluginDotNet.ResourceProvider;

namespace TerraformPluginDotNet.Test.Functional;

class TestResourceProvider : IResourceProvider<TestResource>
{
    public Task<TestResource> PlanAsync(TestResource prior, TestResource proposed)
    {
        return Task.FromResult(proposed);
    }

    public Task<TestResource> CreateAsync(TestResource planned)
    {
        planned.Id = Guid.NewGuid().ToString();
        return Task.FromResult(planned);
    }

    public Task DeleteAsync(TestResource resource)
    {
        return Task.CompletedTask;
    }

    public Task<TestResource> ReadAsync(TestResource resource)
    {
        return Task.FromResult(resource);
    }

    public Task<TestResource> UpdateAsync(TestResource prior, TestResource planned)
    {
        return Task.FromResult(planned);
    }

    public Task<IList<TestResource>> ImportAsync(string id)
    {
        throw new NotSupportedException();
    }
}
