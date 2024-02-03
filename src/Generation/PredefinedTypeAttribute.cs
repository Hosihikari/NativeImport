namespace Hosihikari.NativeInterop.Generation;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
public class PredefinedTypeAttribute : Attribute
{
    public string NativeTypeName { get; set; } = string.Empty;
    public string NativeTypeNamespace { get; set; } = string.Empty;
}