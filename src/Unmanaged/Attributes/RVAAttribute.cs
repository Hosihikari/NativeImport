namespace Hosihikari.NativeInterop.Unmanaged.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
public sealed class RVAAttribute(ulong rva) : Attribute
{
    public ulong RVA { get; private set; } = rva;
}