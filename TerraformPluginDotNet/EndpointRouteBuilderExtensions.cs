using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using TerraformPluginDotNet.Services;

namespace TerraformPluginDotNet;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapTerraformPlugin(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGrpcService<Terraform5ProviderService>();
        return endpointRouteBuilder;
    }
}
