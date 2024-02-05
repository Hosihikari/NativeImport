namespace Hosihikari.NativeInterop.Unmanaged.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
public sealed class FlagBitsAttribute(int value) : Attribute
{
    public int FlagBits { get; private set; } = value;
}