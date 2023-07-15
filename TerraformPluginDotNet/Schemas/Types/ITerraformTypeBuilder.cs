namespace TerraformPluginDotNet.Schemas.Types;

public interface ITerraformTypeBuilder
{
    TerraformType GetTerraformType(Type t);
}
