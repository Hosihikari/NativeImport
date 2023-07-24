using Hosihikari.NativeInterop.LibLoader;

namespace Hosihikari.NativeInterop.Hook;

public class NativeFunc
{
    public class HookInstance
    {
        internal unsafe HookInstance(FuncHookT* instance, void* original)
        {
            _instance = instance;
            _original = original;
        }

        private unsafe FuncHookT* _instance;
        private unsafe void* _original;
        internal unsafe FuncHookT* Instance
        {
            get
            {
                CheckActive();
                return _instance;
            }
        }
        public unsafe void* Original
        {
            get
            {
                CheckActive();
                return _original;
            }
        }

        public HookResult Uninstall()
        {
            unsafe
            {
                CheckActive();
                var result = LibHook.Unhook(ref _original, ref _instance);
                if (result == HookResult.Success)
                {
                    _instance = null;
                    _original = null;
                }
                return result;
            }
        }

        private void CheckActive()
        {
            unsafe
            {
                if (_original is null || _instance is null)
                {
                    throw new InvalidOperationException(
                        "This hook is not active. Maybe already uninstalled."
                    );
                }
            }
        }
    }

    /// <summary>
    /// Hook a function by its address.
    /// </summary>
    /// <param name="address">The address of the function.</param>
    /// <param name="hook">The hook function you created.</param>
    /// <param name="org">The original function.</param>
    /// <param name="instance"> The instance of current hook </param>
    /// <returns>hook result (0 if succeed)</returns>
    public static unsafe HookResult Hook(
        void* address,
        void* hook,
        out void* org,
        out HookInstance instance
    )
    {
        var result = LibHook.Hook(address, hook, out org, out var t);
        instance = new HookInstance(t, org);
        return result;
    }

    /// <summary>
    /// Hook a function by its offset.
    /// </summary>
    /// <param name="offset">The offset of the function.</param>
    /// <param name="hook">The hook function you created.</param>
    /// <param name="org">The original function.</param>
    /// <param name="instance"></param>
    /// <returns>hook result (0 if succeed)</returns>
    internal static unsafe HookResult Hook(
        int offset,
        void* hook,
        out void* org,
        out HookInstance instance
    )
    {
        return Hook(
            (HandleHelper.MainHandleHandle + offset).ToPointer(),
            hook,
            out org,
            out instance
        );
    }

    /// <summary>
    /// Hook a function by its offset.
    /// </summary>
    /// <param name="address">The address of the function.</param>
    /// <param name="hook">The hook function you created.</param>
    /// <param name="org">The original function.</param>
    /// <returns>hook result (0 if succeed)</returns>

    public static HookResult Hook(nint address, nint hook, out nint org, out HookInstance instance)
    {
        if (address == 0)
            throw new NullReferenceException(nameof(address));
        if (hook == 0)
            throw new NullReferenceException(nameof(hook));
        unsafe
        {
            var result = Hook((void*)address, (void*)hook, out var orgPtr, out instance);
            org = (nint)orgPtr;
            return result;
        }
    }

    /// <summary>
    /// Hook a function by its offset.
    /// </summary>
    /// <param name="offset">The offset of the function.</param>
    /// <param name="hook">The hook function you created.</param>
    /// <param name="org">The original function.</param>
    /// <returns>hook result (0 if succeed)</returns>

    public static HookResult Hook(int offset, nint hook, out nint org, out HookInstance instance)
    {
        return Hook(HandleHelper.MainHandleHandle + offset, hook, out org, out instance);
    }
}
