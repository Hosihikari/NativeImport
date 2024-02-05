using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.Import;

public enum HookResult
{
    InternalError = -1,
    Success = 0
}

internal static partial class LibHook
{
    private const string LibName = "dobby";

    [LibraryImport(LibName, EntryPoint = "DobbyHook")]
    public static unsafe partial HookResult Hook(
        void* address,
        void* replaceFunc,
        out void* originFunc
    );

    [LibraryImport(LibName, EntryPoint = "DobbyDestroy")]
    public static unsafe partial HookResult Unhook(void* address);
}