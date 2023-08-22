namespace TerraformPluginDotNet.ResourceProvider;

public interface IDataSourceSchemaProvider
{
    IEnumerable<DataSourceRegistration> GetSchemas();
}
