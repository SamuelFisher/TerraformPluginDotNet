using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraformPluginDotNet
{
    public class TerraformPluginHostOptions
    {
        /// <summary>
        /// The full provider name. For example, `example.com/example/dotnetsample`.
        /// </summary>
        [Required]
        public string FullProviderName { get; set; }

        /// <summary>
        /// Configures debug mode which listens on H2C instead of TLS.
        /// </summary>
        public bool DebugMode { get; set; }
    }
}
