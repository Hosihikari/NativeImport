using Hosihikari.NativeInterop.Import;

namespace Hosihikari.NativeInterop.Hook;

public sealed class HookInstance
{
    private nint _address;
    private nint _original;

    internal HookInstance(nint address, nint original)
    {
        _address = address;
        _original = original;
    }

    public nint OriginalMethod
    {
        get
        {
            CheckActive();
            return _original;
        }
    }

    public HookResult Uninstall()
    {
        CheckActive();
        HookResult result = LibHook.Unhook(_address);
        if (result is not HookResult.Success)
        {
            return result;
        }

        _address = nint.Zero;
        _original = nint.Zero;
        return result;
    }

    private void CheckActive()
    {
        if ((_address == nint.Zero) || (_original == nint.Zero))
        {
            throw new InvalidOperationException(
                "This hook is not active. Maybe already uninstalled."
            );
        }
    }
}