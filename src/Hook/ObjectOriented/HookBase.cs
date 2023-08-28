using System.Runtime.InteropServices;
using Hosihikari.NativeInterop.LibLoader;

namespace Hosihikari.NativeInterop.Hook.ObjectOriented;

public abstract class HookBase<TDelegate> : IHook
    where TDelegate : Delegate
{
    protected HookBase(string symbol)
        : this(SymbolHelper.Dlsym(symbol)) { }

    protected HookBase(nint address)
        : this()
    {
        unsafe
        {
            _address = address.ToPointer();
        }
    }

    private HookBase()
    {
        _originalFuncInstance = new Lazy<TDelegate>(() =>
        {
            unsafe
            {
                return _orgIntPtr is null
                    ? throw new NullReferenceException("OrgIntPtr is zero, has the hook installed ?")
                    : Marshal.GetDelegateForFunctionPointer<TDelegate>(new(_orgIntPtr));
            }
        });
        _hookedFuncInstance = new Lazy<TDelegate>(() =>
        {
            TDelegate hookedFunc = HookedFunc;
            return HookedFunc is null ? throw new NullReferenceException("HookedFunc") : hookedFunc;
        });
    }

    protected TDelegate Original => _originalFuncInstance.Value;
    public abstract TDelegate HookedFunc { get; }
    public bool HasInstalled
    {
        get
        {
            unsafe
            {
                return _orgIntPtr is not null;
            }
        }
    }

    private readonly unsafe void* _address;
    private unsafe void* _orgIntPtr;
    private readonly Lazy<TDelegate> _hookedFuncInstance;
    private readonly Lazy<TDelegate> _originalFuncInstance;
    private unsafe void* _hookedFuncPointer;

    //alloc handle for delegate
    private GCHandle? _handle = null;

    private HookInstance? _instance = null;

    public void Install()
    {
        lock (this)
        {
            unsafe
            {
                if (_address is null)
                {
                    throw new NullReferenceException("Address or symbol is null.");
                }
                if (_orgIntPtr is not null || _handle is not null)
                {
                    throw new HookAlreadyInstalledException();
                }
                //save handle for delegate, prevent gc
                _handle = GCHandle.Alloc(_hookedFuncInstance.Value);
                //get pointer of delegate
                _hookedFuncPointer = Marshal
                    .GetFunctionPointerForDelegate(_hookedFuncInstance.Value)
                    .ToPointer();
                //hook and check if success, otherwise throw exception
                if (
                    NativeFunc.Hook(_address, _hookedFuncPointer, out _orgIntPtr, out _instance)
                    is not HookResult.Success
                        and HookResult errCode
                )
                {
                    throw new HookInstalledFailedException(errCode);
                }
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
            unsafe
            {
                if (_orgIntPtr is null || _instance is null)
                {
                    throw new HookNotInstalledException();
                }
                //free delegate handle
                _handle?.Free();
                _handle = null;
                //unhook and check if success, otherwise throw exception
                if (_instance.Uninstall() is not HookResult.Success and HookResult errCode)
                {
                    throw new HookUninstalledFailedException(errCode);
                }
                _orgIntPtr = null;
                _instance = null;
            }
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
