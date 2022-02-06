using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TerraformPluginDotNet;

public record PluginHostCertificate
{
    public X509Certificate2 Certificate { get; init; }
}
