using Hosihikari.NativeInterop.LibLoader;

namespace Hosihikari.NativeInterop.Hook;

public class NativeFunc
{
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
        HookResult result = LibHook.Hook(address, hook, out org);
        instance = new HookInstance(address, org);
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
    ) => Hook(
            (HandleHelper.MainHandleHandle + offset).ToPointer(),
            hook,
            out org,
            out instance
        );

    /// <summary>
    /// Hook a function by its offset.
    /// </summary>
    /// <param name="address">The address of the function.</param>
    /// <param name="hook">The hook function you created.</param>
    /// <param name="org">The original function.</param>
    /// <returns>hook result (0 if succeed)</returns>

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
        unsafe
        {
            HookResult result = Hook(address.ToPointer(), hook.ToPointer(), out void* orgPtr, out instance);
            org = new(orgPtr);
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

    public static HookResult Hook(int offset, nint hook, out nint org, out HookInstance instance) =>
        Hook(HandleHelper.MainHandleHandle + offset, hook, out org, out instance);
}
