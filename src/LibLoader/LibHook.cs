using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.LibLoader;

public enum HookResult : int
{
    InternalError = -1,
    Success = 0,
    ErrorOutOfMemory = 1,
    ErrorAlreadyInstalled = 2,
    ErrorDisassembly = 3,
    ErrorIpRelativeOffset = 4,
    ErrorCannotFixIpRelative = 5,
    ErrorFoundBackJump = 6,
    ErrorTooShortInstructions = 7,

    /// <summary>
    /// memory allocation error
    /// </summary>
    ErrorMemoryAllocation = 8,

    /// <summary>
    ///  other memory function errors
    /// </summary>
    ErrorMemoryFunction = 9,
    ErrorNotInstalled = 10,
    ErrorNoAvailableRegisters = 11,
    ErrorNoSpaceNearTargetAddr = 12,
}

internal struct FuncHookT { }

internal static class LibHook
{
    const string LibName = LibNative.LibName; //"libhook";

    [DllImport(LibName, EntryPoint = "hook", ExactSpelling = true)]
    internal static extern unsafe HookResult Hook(
        void* rva,
        void* hook,
        out void* org,
        out FuncHookT* instance
    );

    [DllImport(LibName, EntryPoint = "unhook", ExactSpelling = true)]
    internal static extern unsafe HookResult Unhook(ref void* org, ref FuncHookT* instance);
}
