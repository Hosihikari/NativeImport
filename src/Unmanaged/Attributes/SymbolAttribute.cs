namespace Hosihikari.NativeInterop.Unmanaged.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
public class SymbolAttribute(string symbol) : Attribute
{
    public string Symbol { get; private set; } = symbol;
}
