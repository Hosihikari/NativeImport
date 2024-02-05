namespace Hosihikari.NativeInterop.Hook.ObjectOriented;

public sealed class EasyHook<TDelegate>(string rva, TDelegate func)
    : HookBase<TDelegate>(rva) where TDelegate : Delegate
{
    public EasyHook(TDelegate oldFunc, TDelegate newFunc)
        : this(SymbolHelper.QuerySymbol(oldFunc), newFunc)
    {
    }

    public override TDelegate HookedFunc { get; } = func;
}