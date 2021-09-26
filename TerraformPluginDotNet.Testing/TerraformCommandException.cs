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
            Message = $"Terraform exited with code {ExitCode}.";
        }

        internal TerraformCommandException(string command, TimeSpan? timedOutAfter, string output)
        {
            Command = command;
            TimedOutAfter = timedOutAfter;
            Output = output;
            Message = $"Terraform timed out after {TimedOutAfter.Value.TotalSeconds:N2}s.";
        }

        public override string Message { get; }

        public string Command { get; }

        public int ExitCode { get; }

        public TimeSpan? TimedOutAfter { get; }

        public string Output { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Message);
            sb.AppendLine();
            sb.AppendLine($"$ terraform {Command}");
            sb.Append(Output);
            return sb.ToString();
        }
    }
}
