namespace Hosihikari.NativeInterop.Generation;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
public sealed class PredefinedTypeAttribute : Attribute
{
    public required string TypeName { get; set; }

    public bool IgnoreTemplateArgs { get; set; } = false;
}