using System.Threading.Tasks;

namespace TerraformPluginDotNet.Testing;

public static class TerraformTestInstanceExtensions
{
    public static Task<string> InitAsync(this ITerraformTestInstance terraform)
    {
        return terraform.RunCommandAsync("init -no-color");
    }

    public static Task<string> PlanAsync(this ITerraformTestInstance terraform)
    {
        return terraform.RunCommandAsync("plan -no-color");
    }

    public static Task<string> ApplyAsync(this ITerraformTestInstance terraform)
    {
        return terraform.RunCommandAsync("apply -no-color -input=false -auto-approve=true");
    }
}
