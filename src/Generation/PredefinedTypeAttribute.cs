namespace Hosihikari.NativeInterop.Generation;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
public sealed class PredefinedTypeAttribute : Attribute
{
    public string? NativeTypeName { get; set; }
    public string? NativeTypeNamespace { get; set; }
}