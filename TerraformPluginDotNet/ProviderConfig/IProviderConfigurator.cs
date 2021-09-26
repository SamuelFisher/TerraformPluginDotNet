using System.Text;
using System.Threading.Tasks;

namespace TerraformPluginDotNet.ProviderConfig
{
    public interface IProviderConfigurator<T>
    {
        Task ConfigureAsync(T config);
    }
}
