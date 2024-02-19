using Hosihikari.NativeInterop.Import;
using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.Hook.ObjectOriented;

public abstract class HookBase<TDelegate> : IHook
    where TDelegate : Delegate
{
    private readonly nint _address;
    private readonly Lazy<TDelegate> _hookDelegateInstance;

    private readonly Lazy<TDelegate> _originalMethodGroupInstance;

    //alloc handle for delegate
    private GCHandle? _handle;
    private nint _hookedDelegatePointer;
    private HookInstance? _instance;
    private nint _originalMethodGroupPointer;

    protected HookBase(Delegate methodGroup)
        : this(SymbolHelper.QuerySymbol(methodGroup))
    {
    }

    protected HookBase(string symbol)
        : this(SymbolHelper.Dlsym(symbol))
    {
    }

    protected HookBase(nint address)
        : this()
    {
        _address = address;
    }

    private HookBase()
    {
        _originalMethodGroupInstance = new(() => _originalMethodGroupPointer == nint.Zero
            ? throw new NullReferenceException("The original pointer is null. Has the hook been installed?")
            : Marshal.GetDelegateForFunctionPointer<TDelegate>(_originalMethodGroupPointer));
        _hookDelegateInstance = new(() => HookDelegate ?? throw new NullReferenceException("Hook method is null."));
    }

    protected TDelegate Original => _originalMethodGroupInstance.Value;

    protected abstract TDelegate HookDelegate { get; }

    public bool HasInstalled => _originalMethodGroupPointer != nint.Zero;

    public void Install()
    {
        lock (this)
        {
            if (_address == nint.Zero)
            {
                throw new NullReferenceException("Address is null or symbol is invalid.");
            }

            if ((_originalMethodGroupPointer != nint.Zero) || _instance is not null || _handle is not null)
            {
                throw new HookAlreadyInstalledException();
            }

            _handle = GCHandle.Alloc(_hookDelegateInstance.Value);
            _hookedDelegatePointer = Marshal.GetFunctionPointerForDelegate(_hookDelegateInstance.Value);
            HookResult result = Function.Hook(_address, _hookedDelegatePointer, out _originalMethodGroupPointer,
                out _instance);
            if (result is not HookResult.Success)
            {
                throw new HookInstalledFailedException(result);
            }
        }
    }

    public bool TryInstall()
    {
        try
        {
            Install();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Uninstall()
    {
        lock (this)
        {
            if ((_originalMethodGroupPointer == nint.Zero) || _instance is null || _handle is null)
            {
                throw new HookNotInstalledException();
            }

            HookResult result = _instance.Uninstall();
            if (result is not HookResult.Success)
            {
                throw new HookUninstalledFailedException(result);
            }

            _originalMethodGroupPointer = nint.Zero;
            _instance = default;
            _handle?.Free();
            _handle = default;
        }
    }

    public bool TryUninstall()
    {
        try
        {
            Uninstall();
            return true;
        }
        catch
        {
            return false;
        }
    }
}