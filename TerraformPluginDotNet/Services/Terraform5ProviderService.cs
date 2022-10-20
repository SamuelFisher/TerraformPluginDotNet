using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TerraformPluginDotNet.ProviderConfig;
using TerraformPluginDotNet.ResourceProvider;
using Tfplugin5;

namespace TerraformPluginDotNet.Services;

class Terraform5ProviderService : Provider.ProviderBase
{
    private readonly ILogger<Terraform5ProviderService> _logger;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ResourceRegistry _resourceRegistry;
    private readonly IServiceProvider _serviceProvider;
    private readonly ProviderConfigurationRegistry _providerConfiguration;

    public Terraform5ProviderService(
        ILogger<Terraform5ProviderService> logger,
        IHostApplicationLifetime lifetime,
        ResourceRegistry resourceRegistry,
        IServiceProvider serviceProvider,
        ProviderConfigurationRegistry providerConfiguration = null)
    {
        _logger = logger;
        _lifetime = lifetime;
        _resourceRegistry = resourceRegistry;
        _serviceProvider = serviceProvider;
        _providerConfiguration = providerConfiguration;
    }

    public override async Task<Configure.Types.Response> Configure(Configure.Types.Request request, ServerCallContext context)
    {
        if (_providerConfiguration == null)
        {
            return new Configure.Types.Response { };
        }

        var configurationHostType = typeof(ProviderConfigurationHost<>).MakeGenericType(_providerConfiguration.ConfigurationType);
        var configurationHost = _serviceProvider.GetService(configurationHostType);
        await (Task)configurationHostType.GetMethod(nameof(ProviderConfigurationHost<object>.ConfigureAsync))
            .Invoke(configurationHost, new[] { request });
        return new Configure.Types.Response { };
    }

    public override Task<GetProviderSchema.Types.Response> GetSchema(GetProviderSchema.Types.Request request, ServerCallContext context)
    {
        var res = new GetProviderSchema.Types.Response
        {
            Provider = _providerConfiguration?.ConfigurationSchema ?? new Schema { Block = new Schema.Types.Block { } },
        };

        foreach (var schema in _resourceRegistry.Schemas)
        {
            res.ResourceSchemas.Add(schema.Key, schema.Value);
        }

        foreach (var schema in _resourceRegistry.DataSchemas)
        {
            res.DataSourceSchemas.Add(schema.Key, schema.Value);
        }

        return Task.FromResult(res);
    }

    public override Task<PlanResourceChange.Types.Response> PlanResourceChange(PlanResourceChange.Types.Request request, ServerCallContext context)
    {
        if (!_resourceRegistry.Types.TryGetValue(request.TypeName, out var resourceType))
        {
            return Task.FromResult(new Tfplugin5.PlanResourceChange.Types.Response
            {
                Diagnostics =
                    {
                        new Diagnostic { Detail = "Unknown type name." },
                    },
            });
        }

        var providerHostType = typeof(ResourceProviderHost<>).MakeGenericType(resourceType);
        var provider = _serviceProvider.GetService(providerHostType);
        return (Task<PlanResourceChange.Types.Response>)providerHostType.GetMethod(nameof(ResourceProviderHost<object>.PlanResourceChange))
            .Invoke(provider, new[] { request });
    }

    public override Task<ApplyResourceChange.Types.Response> ApplyResourceChange(ApplyResourceChange.Types.Request request, ServerCallContext context)
    {
        if (!_resourceRegistry.Types.TryGetValue(request.TypeName, out var resourceType))
        {
            return Task.FromResult(new ApplyResourceChange.Types.Response
            {
                Diagnostics =
                    {
                        new Diagnostic { Detail = "Unknown type name." },
                    },
            });
        }

        var providerHostType = typeof(ResourceProviderHost<>).MakeGenericType(resourceType);
        var provider = _serviceProvider.GetService(providerHostType);
        return (Task<ApplyResourceChange.Types.Response>)providerHostType.GetMethod(nameof(ResourceProviderHost<object>.ApplyResourceChange))
            .Invoke(provider, new[] { request });
    }

    public override Task<UpgradeResourceState.Types.Response> UpgradeResourceState(UpgradeResourceState.Types.Request request, ServerCallContext context)
    {
        if (!_resourceRegistry.Types.TryGetValue(request.TypeName, out var resourceType))
        {
            return Task.FromResult(new UpgradeResourceState.Types.Response
            {
                Diagnostics =
                    {
                        new Diagnostic { Detail = "Unknown type name." },
                    },
            });
        }

        var providerHostType = typeof(ResourceProviderHost<>).MakeGenericType(resourceType);
        var provider = _serviceProvider.GetService(providerHostType);
        return (Task<UpgradeResourceState.Types.Response>)providerHostType.GetMethod(nameof(ResourceProviderHost<object>.UpgradeResourceState))
            .Invoke(provider, new[] { request });
    }

    public override Task<ReadResource.Types.Response> ReadResource(ReadResource.Types.Request request, ServerCallContext context)
    {
        if (!_resourceRegistry.Types.TryGetValue(request.TypeName, out var resourceType))
        {
            return Task.FromResult(new ReadResource.Types.Response
            {
                Diagnostics =
                    {
                        new Diagnostic { Detail = "Unknown type name." },
                    },
            });
        }

        var providerHostType = typeof(ResourceProviderHost<>).MakeGenericType(resourceType);
        var provider = _serviceProvider.GetService(providerHostType);
        return (Task<ReadResource.Types.Response>)providerHostType.GetMethod(nameof(ResourceProviderHost<object>.ReadResource))
            .Invoke(provider, new[] { request });
    }

    public override Task<ImportResourceState.Types.Response> ImportResourceState(ImportResourceState.Types.Request request, ServerCallContext context)
    {
        if (!_resourceRegistry.Types.TryGetValue(request.TypeName, out var resourceType))
        {
            return Task.FromResult(new ImportResourceState.Types.Response
            {
                Diagnostics =
                    {
                        new Diagnostic { Detail = "Unknown type name." },
                    },
            });
        }

        var providerHostType = typeof(ResourceProviderHost<>).MakeGenericType(resourceType);
        var provider = _serviceProvider.GetService(providerHostType);
        return (Task<ImportResourceState.Types.Response>)providerHostType.GetMethod(nameof(ResourceProviderHost<object>.ImportResourceState))
            .Invoke(provider, new[] { request });
    }

    public override Task<PrepareProviderConfig.Types.Response> PrepareProviderConfig(PrepareProviderConfig.Types.Request request, ServerCallContext context)
    {
        return Task.FromResult(new PrepareProviderConfig.Types.Response());
    }

    public override Task<Stop.Types.Response> Stop(Stop.Types.Request request, ServerCallContext context)
    {
        _lifetime.StopApplication();
        _lifetime.ApplicationStopped.WaitHandle.WaitOne();
        return Task.FromResult(new Stop.Types.Response());
    }

    public override Task<ValidateDataSourceConfig.Types.Response> ValidateDataSourceConfig(ValidateDataSourceConfig.Types.Request request, ServerCallContext context)
    {
        return Task.FromResult(new ValidateDataSourceConfig.Types.Response());
    }

    public override Task<ValidateResourceTypeConfig.Types.Response> ValidateResourceTypeConfig(ValidateResourceTypeConfig.Types.Request request, ServerCallContext context)
    {
        return Task.FromResult(new ValidateResourceTypeConfig.Types.Response());
    }

    public override Task<ReadDataSource.Types.Response> ReadDataSource(ReadDataSource.Types.Request request, ServerCallContext context)
    {
        if (!_resourceRegistry.DataTypes.TryGetValue(request.TypeName, out var resourceType))
        {
            return Task.FromResult(new ReadDataSource.Types.Response
            {
                Diagnostics =
                    {
                        new Diagnostic { Detail = "Unknown type name." },
                    },
            });
        }

        var providerHostType = typeof(DataSourceProviderHost<>).MakeGenericType(resourceType);
        var provider = _serviceProvider.GetService(providerHostType);
        return (Task<ReadDataSource.Types.Response>)providerHostType.GetMethod(nameof(DataSourceProviderHost<object>.ReadDataSource))!
            .Invoke(provider, new[] { request })!;
    }
}
