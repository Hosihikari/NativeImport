namespace Hosihikari.NativeInterop.Hook.ObjectOriented;

public sealed class EasyHook<TDelegate>(string symbol, TDelegate hookDelegate)
    : HookBase<TDelegate>(symbol) where TDelegate : Delegate
{
    public EasyHook(TDelegate originalMethodGroup, TDelegate hookDelegate)
        : this(SymbolHelper.QuerySymbol(originalMethodGroup), hookDelegate)
    {
    }

    protected override TDelegate HookDelegate { get; } = hookDelegate;
}