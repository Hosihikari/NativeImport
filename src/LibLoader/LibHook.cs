using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.LibLoader;

public enum HookResult : int
{
    InternalError = -1,
    Success = 0
}

internal struct FuncHookT { }

internal static class LibHook
{
    private const string LibName = "libdobby.so";

    [DllImport(LibName, EntryPoint = "DobbyHook", ExactSpelling = true)]
    internal static extern unsafe HookResult Hook(
        void* address,
        void* replace_func,
        out void* origin_func
    );

    [DllImport(LibName, EntryPoint = "DobbyDestroy", ExactSpelling = true)]
    internal static extern unsafe HookResult Unhook(void* address);
}
