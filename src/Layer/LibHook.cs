using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.Layer;

public enum HookResult : int
{
    InternalError = -1,
    Success = 0
}

internal static partial class LibHook
{
    private const string LibName = "libdobby";

    [LibraryImport(LibName, EntryPoint = "DobbyHook")]
    internal static unsafe partial HookResult Hook(
        void* address,
        void* replace_func,
        out void* origin_func
    );

    [LibraryImport(LibName, EntryPoint = "DobbyDestroy")]
    internal static unsafe partial HookResult Unhook(void* address);
}
