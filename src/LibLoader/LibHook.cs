using System.Runtime.InteropServices;

namespace NativeInterop.LibLoader;

internal static class LibHook
{
    const string libname = "libhook";

    [DllImport(libname, EntryPoint = "hook", ExactSpelling = true)]
    internal static extern unsafe int Hook(void* rva, void* hook, out void* org);
}
