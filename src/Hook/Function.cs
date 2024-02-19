using Hosihikari.NativeInterop.Import;

namespace Hosihikari.NativeInterop.Hook;

public static class Function
{
    /// <summary>
    ///     Hook a function by its address.
    /// </summary>
    /// <param name="address">The address of the function.</param>
    /// <param name="hook">The hook function you created.</param>
    /// <param name="org">The original function.</param>
    /// <param name="instance">The instance of current hook.</param>
    /// <returns>Result of hooking.</returns>
    public static unsafe HookResult Hook(
        void* address,
        void* hook,
        out void* org,
        out HookInstance instance
    )
    {
        HookResult result = Hook((nint)address, (nint)hook, out nint orgPtr, out instance);
        org = orgPtr.ToPointer();
        return result;
    }

    /// <summary>
    ///     Hook a function by its offset.
    /// </summary>
    /// <param name="offset">The offset of the function.</param>
    /// <param name="hook">The hook function you created.</param>
    /// <param name="org">The original function.</param>
    /// <param name="instance">The instance of current hook.</param>
    /// <returns>Result of hooking.</returns>
    public static unsafe HookResult Hook(
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
    ///     Hook a function by its offset.
    /// </summary>
    /// <param name="address">The address of the function.</param>
    /// <param name="hook">The hook function you created.</param>
    /// <param name="org">The original function.</param>
    /// <param name="instance">The instance of current hook.</param>
    /// <returns>Result of hooking.</returns>
    public static HookResult Hook(nint address, nint hook, out nint org, out HookInstance instance)
    {
        if (address == nint.Zero)
        {
            throw new NullReferenceException(nameof(address));
        }

        if (hook == nint.Zero)
        {
            throw new NullReferenceException(nameof(hook));
        }

        HookResult result = LibHook.Hook(address, hook, out org);
        instance = new(address, org);
        return result;
    }

    /// <summary>
    ///     Hook a function by its offset.
    /// </summary>
    /// <param name="offset">The offset of the function.</param>
    /// <param name="hook">The hook function you created.</param>
    /// <param name="org">The original function.</param>
    /// <param name="instance">The instance of current hook.</param>
    /// <returns>Result of hooking.</returns>
    public static HookResult Hook(int offset, nint hook, out nint org, out HookInstance instance)
    {
        return Hook(HandleHelper.MainHandleHandle + offset, hook, out org, out instance);
    }
}