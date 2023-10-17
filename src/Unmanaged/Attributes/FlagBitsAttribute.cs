namespace Hosihikari.NativeInterop.Unmanaged.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
public class FlagBitsAttribute : Attribute
{
    public int FlagBits { get; private set; }
    public FlagBitsAttribute(int value) => FlagBits = value;
}
