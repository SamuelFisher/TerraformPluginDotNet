using System.Security.Cryptography.X509Certificates;

namespace TerraformPluginDotNet;

public record PluginHostCertificate(X509Certificate2 Certificate);
