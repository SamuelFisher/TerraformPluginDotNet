using System;
using System.Threading.Tasks;

namespace TerraformPluginDotNet.Testing
{
    public interface ITerraformTestInstance : IDisposable
    {
        TimeSpan DefaultCommandTimeout { get; }

        public string WorkDir { get; }

        /// <summary>
        /// Runs a Terraform command. Fails if the Terraform process does not exit with code 0 within the specified timeout.
        /// </summary>
        /// <param name="command">The command to run.</param>
        /// <param name="timeout">Defines how long to wait for Terraform to exit. Defaults to 1 second if not specified.</param>
        Task RunCommandAsync(string command, TimeSpan? timeout = default);
    }
}
