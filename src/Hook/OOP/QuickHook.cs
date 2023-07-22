namespace NativeInterop.Hook.OOP;

public class QuickHook<TDelegate> : HookBase<TDelegate>
    where TDelegate : Delegate
{
    public QuickHook(string rva, TDelegate func)
        : base(rva)
    {
        HookedFunc = func;
    }

    public override TDelegate HookedFunc { get; }
}
