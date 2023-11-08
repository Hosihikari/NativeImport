using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.Layer;

public enum HookResult : int
{
    InternalError = -1,
    Success = 0
}

internal static partial class LibHook
{

#if LINUX
    private const string LibName = "libdobby";
#else
    private const string LibName = "Hosihikari.Preload";
#endif

#if LINUX
    [LibraryImport(LibName, EntryPoint = "DobbyHook")]
#else
    [LibraryImport(LibName, EntryPoint = "hook_function")]
#endif
    internal static unsafe partial HookResult Hook(
        void* address,
        void* replace_func,
        out void* origin_func
    );


#if LINUX
    [LibraryImport(LibName, EntryPoint = "DobbyDestroy")]
#else
    [LibraryImport(LibName, EntryPoint = "unhook_function")]
#endif
    internal static unsafe partial HookResult Unhook(void* address);
}
