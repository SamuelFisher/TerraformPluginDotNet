using System.Threading.Tasks;

namespace TerraformPluginDotNet.Testing
{
    public static class TerraformTestInstanceExtensions
    {
        public static Task InitAsync(this ITerraformTestInstance terraform)
        {
            return terraform.RunCommandAsync("init -no-color");
        }

        public static Task PlanAsync(this ITerraformTestInstance terraform)
        {
            return terraform.RunCommandAsync("plan -no-color");
        }

        public static Task ApplyAsync(this ITerraformTestInstance terraform)
        {
            return terraform.RunCommandAsync("apply -no-color -input=false -auto-approve=true");
        }
    }
}
