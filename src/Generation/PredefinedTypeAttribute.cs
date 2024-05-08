namespace Hosihikari.NativeInterop.Generation;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
public sealed class PredefinedTypeAttribute : Attribute
{
    public string TypeName { get; set; } = string.Empty;

    public bool IgnoreTemplateArgs { get; set; } = false;
}