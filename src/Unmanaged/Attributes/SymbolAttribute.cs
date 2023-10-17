namespace Hosihikari.NativeInterop.Unmanaged.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
public class SymbolAttribute : Attribute
{
    public string Symbol { get; private set; }

    public SymbolAttribute(string symbol) => Symbol = symbol;
}
