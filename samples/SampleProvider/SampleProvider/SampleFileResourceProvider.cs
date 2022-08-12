using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TerraformPluginDotNet.ResourceProvider;

namespace SampleProvider;

public class SampleFileResourceProvider : IResourceProvider<SampleFileResource>
{
    private readonly SampleConfigurator _configurator;

    public SampleFileResourceProvider(SampleConfigurator configurator)
    {
        _configurator = configurator;
    }

    public Task<SampleFileResource> PlanAsync(SampleFileResource prior, SampleFileResource proposed)
    {
        return Task.FromResult(proposed);
    }

    public async Task<SampleFileResource> CreateAsync(SampleFileResource planned)
    {
        planned.Id = Guid.NewGuid().ToString();
        await File.WriteAllTextAsync(planned.Path, BuildContent(planned.Content));
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
        await File.WriteAllTextAsync(planned.Path, BuildContent(planned.Content));
        return planned;
    }

    public async Task<IList<SampleFileResource>> ImportAsync(string id)
    {
        // Id is not SampleFileResource.Id, it's the "import ID" supplied by Terraform
        // and in this provider, is defined to be the file name.

        if (!File.Exists(id))
        {
            throw new TerraformResourceProviderException($"File '{id}' does not exist.");
        }

        var content = await File.ReadAllTextAsync(id);

        return new[]
        {
            new SampleFileResource
            {
                Id = Guid.NewGuid().ToString(),
                Path = id,
                Content = content,
            },
        };
    }

    private string BuildContent(string content)
    {
        if (_configurator.Config?.FileHeader is not string header)
        {
            return content;
        }

        var sb = new StringBuilder();
        sb.AppendLine(header);
        sb.Append(content);
        return sb.ToString();
    }
}
