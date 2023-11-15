using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.Layer;

public enum HookResult : int
{
    InternalError = -1,
    Success = 0
}

internal static partial class LibHook
{

#if WINDOWS
    private const string LibName = "Hosihikari.Preload";
#else
    private const string LibName = "dobby";
#endif

#if WINDOWS
    [LibraryImport(LibName, EntryPoint = "hook_function")]
#else
    [LibraryImport(LibName, EntryPoint = "DobbyHook")]
#endif
    internal static unsafe partial HookResult Hook(
        void* address,
        void* replace_func,
        out void* origin_func
    );


#if WINDOWS
    [LibraryImport(LibName, EntryPoint = "unhook_function")]
#else
    [LibraryImport(LibName, EntryPoint = "DobbyDestroy")]
#endif
    internal static unsafe partial HookResult Unhook(void* address);
}
