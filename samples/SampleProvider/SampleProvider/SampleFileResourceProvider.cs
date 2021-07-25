using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraformPluginDotNet.ResourceProvider;

namespace SampleProvider
{
    public class SampleFileResourceProvider : IResourceProvider<SampleFileResource>
    {
        public Task<SampleFileResource> PlanAsync(SampleFileResource prior, SampleFileResource proposed)
        {
            return Task.FromResult(proposed);
        }

        public async Task<SampleFileResource> CreateAsync(SampleFileResource planned)
        {
            planned.Id = Guid.NewGuid().ToString();
            await File.WriteAllTextAsync(planned.Path, planned.Content);
            return planned;
        }

        public Task DeleteAsync(SampleFileResource resource)
        {
            File.Delete(resource.Path);
            return Task.CompletedTask;
        }

        public async Task<SampleFileResource> ReadAsync(SampleFileResource resource)
        {
            var content = await File.ReadAllTextAsync(resource.Path);
            resource.Content = content;
            return resource;
        }

        public async Task<SampleFileResource> UpdateAsync(SampleFileResource prior, SampleFileResource planned)
        {
            await File.WriteAllTextAsync(planned.Path, planned.Content);
            return planned;
        }
    }
}
