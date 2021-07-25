using System;
using System.Text;

namespace TerraformPluginDotNet.Testing
{
    public class TerraformCommandException : Exception
    {
        internal TerraformCommandException(string command, int exitCode, string output)
        {
            Command = command;
            ExitCode = exitCode;
            Output = output;
        }

        internal TerraformCommandException(string command, TimeSpan? timedOutAfter, string output)
        {
            Command = command;
            TimedOutAfter = timedOutAfter;
            Output = output;
        }

        public string Command { get; private set; }

        public int ExitCode { get; private set; }

        public TimeSpan? TimedOutAfter { get; private set; }

        public string Output { get; private set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (TimedOutAfter.HasValue)
            {
                sb.AppendLine($"Terraform timed out after {TimedOutAfter.Value.TotalSeconds:N2}s.");
            }
            else
            {
                sb.AppendLine($"Terraform exited with code {ExitCode}.");
            }

            sb.AppendLine();
            sb.AppendLine($"$ terraform {Command}");
            sb.Append(Output);

            return sb.ToString();
        }
    }
}
