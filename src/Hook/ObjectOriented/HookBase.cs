using Hosihikari.NativeInterop.Import;
using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.Hook.ObjectOriented;

public abstract class HookBase<TDelegate> : IHook
    where TDelegate : Delegate
{
    private readonly nint _address;
    private readonly Lazy<TDelegate> _hookedMethodInstance;

    private readonly Lazy<TDelegate> _originalMethodInstance;

    //alloc handle for delegate
    private GCHandle? _handle;
    private nint _hookedMethodPointer;
    private HookInstance? _instance;
    private nint _orgIntPtr;

    protected HookBase(Delegate method)
        : this(SymbolHelper.QuerySymbol(method))
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
        _originalMethodInstance = new(() => _orgIntPtr == nint.Zero
            ? throw new NullReferenceException("OrgIntPtr is zero, has the hook installed?")
            : Marshal.GetDelegateForFunctionPointer<TDelegate>(new(_orgIntPtr)));
        _hookedMethodInstance = new(() =>
        {
            TDelegate hookedMethod = HookMethod;
            return HookMethod is null ? throw new NullReferenceException("HookedMethod") : hookedMethod;
        });
    }

    protected TDelegate OriginalMethod => _originalMethodInstance.Value;
    public abstract TDelegate HookMethod { get; }

    public bool HasInstalled => _orgIntPtr != nint.Zero;

    public void Install()
    {
        lock (this)
        {
            if (_address == nint.Zero)
            {
                throw new NullReferenceException("Address or symbol is null.");
            }

            if ((_orgIntPtr != nint.Zero) || _handle is not null)
            {
                throw new HookAlreadyInstalledException();
            }

            //save handle for delegate, prevent gc
            _handle = GCHandle.Alloc(_hookedMethodInstance.Value);
            //get pointer of delegate
            _hookedMethodPointer = Marshal.GetFunctionPointerForDelegate(_hookedMethodInstance.Value);
            //hook and check if success, otherwise throw exception
            if (
                Function.Hook(_address, _hookedMethodPointer, out _orgIntPtr, out _instance)
                is not HookResult.Success
                and var errCode
            )
            {
                throw new HookInstalledFailedException(errCode);
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
            if ((_orgIntPtr == nint.Zero) || _instance is null)
            {
                throw new HookNotInstalledException();
            }

            //free delegate handle
            _handle?.Free();
            _handle = null;
            //unhook and check if success, otherwise throw exception
            if (_instance.Uninstall() is not HookResult.Success and var errCode)
            {
                throw new HookUninstalledFailedException(errCode);
            }

            _orgIntPtr = nint.Zero;
            _instance = null;
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