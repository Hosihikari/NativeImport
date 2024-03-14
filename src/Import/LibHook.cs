using System.Runtime.InteropServices;
using System.Runtime.Versioning;

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
    public static partial HookResult Hook(
        nint address,
        nint replaceFunc,
        out nint originFunc
    );

    [LibraryImport(LibName, EntryPoint = "DobbyDestroy")]
    public static partial HookResult Unhook(nint address);

    [SupportedOSPlatform("windows")]
    [LibraryImport(LibName, EntryPoint = "resolveSymbol", StringMarshalling = StringMarshalling.Utf8)]
    public static partial nint ResolveSymbol(string symbol);
}