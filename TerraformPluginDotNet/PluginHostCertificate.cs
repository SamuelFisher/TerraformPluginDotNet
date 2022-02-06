using System.Security.Cryptography.X509Certificates;

namespace TerraformPluginDotNet;

public record PluginHostCertificate
{
    public X509Certificate2 Certificate { get; init; }
}
