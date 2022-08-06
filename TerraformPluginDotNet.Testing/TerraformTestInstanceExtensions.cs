using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TerraformPluginDotNet.Testing.Json;

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

    public static async Task<TerraformJsonPlan> PlanWithOutputAsync(this ITerraformTestInstance terraform)
    {
        var tmp = Path.GetTempFileName();

        try
        {
            await terraform.RunCommandAsync($"plan -no-color -out=\"{tmp}\"");
            var jsonPlan = await terraform.RunCommandAsync($"show -json \"{tmp}\"");
            return JsonSerializer.Deserialize<TerraformJsonPlan>(jsonPlan);
        }
        finally
        {
            File.Delete(tmp);
        }
    }

    public static Task<string> ApplyAsync(this ITerraformTestInstance terraform)
    {
        return terraform.RunCommandAsync("apply -no-color -input=false -auto-approve=true");
    }
}
