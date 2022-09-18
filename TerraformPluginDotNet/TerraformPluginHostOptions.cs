using System.ComponentModel.DataAnnotations;

namespace TerraformPluginDotNet;

public class TerraformPluginHostOptions
{
    /// <summary>
    /// The full provider name. For example, `example.com/example/sampleprovider`.
    /// </summary>
    [Required]
    public string FullProviderName { get; set; }

    /// <summary>
    /// Configures debug mode which listens on H2C instead of TLS.
    /// </summary>
    public bool DebugMode { get; set; }
}
