namespace Hosihikari.NativeInterop.Unmanaged.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
public class RVAAttribute : Attribute
{
    public ulong RVA { get; private set; }

    public RVAAttribute(ulong rva) => RVA = rva;
}
