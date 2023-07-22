using NativeInterop.LibLoader;

namespace NativeInterop.Hook;

public class NativeFunc
{
    /// <summary>
    /// Hook a function by its address.
    /// </summary>
    /// <param name="address">The address of the function.</param>
    /// <param name="hook">The hook function you created.</param>
    /// <param name="org">The original function.</param>
    /// <returns>hook result (0 if succeed)</returns>
    public static unsafe int Hook(void* address, void* hook, out void* org)
    {
        return LibHook.Hook(address, hook, out org);
    }

    /// <summary>
    /// Hook a function by its offset.
    /// </summary>
    /// <param name="offset">The offset of the function.</param>
    /// <param name="hook">The hook function you created.</param>
    /// <param name="org">The original function.</param>
    /// <returns>hook result (0 if succeed)</returns>
    public static unsafe int Hook(int offset, void* hook, out void* org)
    {
        return LibHook.Hook((HandleHelper.MainHandleHandle + offset).ToPointer(), hook, out org);
    }

    /// <summary>
    /// Hook a function by its offset.
    /// </summary>
    /// <param name="address">The address of the function.</param>
    /// <param name="hook">The hook function you created.</param>
    /// <param name="org">The original function.</param>
    /// <returns>hook result (0 if succeed)</returns>

    public static int Hook(nint address, nint hook, out nint org)
    {
        if (address == 0)
            throw new NullReferenceException(nameof(address));
        if (hook == 0)
            throw new NullReferenceException(nameof(hook));
        unsafe
        {
            var result = LibHook.Hook((void*)address, (void*)hook, out var orgPtr);
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

    public static int Hook(int offset, nint hook, out nint org)
    {
        return Hook(HandleHelper.MainHandleHandle + offset, hook, out org);
    }
}
