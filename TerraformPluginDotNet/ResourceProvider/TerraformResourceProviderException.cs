namespace TerraformPluginDotNet.ResourceProvider;

#nullable enable

public class TerraformResourceProviderException : Exception
{
    public TerraformResourceProviderException(string message, Exception? innerException = default)
        : base(message, innerException)
    {
    }
}
