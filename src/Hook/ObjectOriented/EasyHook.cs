namespace Hosihikari.NativeInterop.Hook.ObjectOriented;

public class EasyHook<TDelegate> : HookBase<TDelegate>
    where TDelegate : Delegate
{
    public EasyHook(string rva, TDelegate func)
        : base(rva)
    {
        HookedFunc = func;
    }

    public EasyHook(TDelegate oldFunc, TDelegate newFunc)
        : base(SymbolHelper.QuerySymbol(oldFunc))
    {
        HookedFunc = newFunc;
    }

    public override TDelegate HookedFunc { get; }
}
