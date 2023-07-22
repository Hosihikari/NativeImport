using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NativeInterop;

public static class LibPreloader
{
    const string libname = "libpreloader";

    [DllImport(libname, EntryPoint = "hook", ExactSpelling = true)]
    public static extern unsafe int Hook(void* rva, void* hook, out void* org);

    public static unsafe int Hook(int offset, void* hook, out void* org)
    {
        return Hook((MainHandleHandle + offset).ToPointer(), hook, out org);
    }

    public static int Hook(nint rva, nint hook, out nint org)
    {
        if (rva == 0)
            throw new NullReferenceException(nameof(rva));
        if (hook == 0)
            throw new NullReferenceException(nameof(hook));
        unsafe
        {
            var result = Hook((void*)rva, (void*)hook, out var orgPtr);
            org = (nint)orgPtr;
            return result;
        }
    }

    public static int Hook(int offset, nint hook, out nint org)
    {
        return Hook(MainHandleHandle + offset, hook, out org);
    }

    private static readonly Lazy<nint> _handle =
        new(() =>
        {
            var cp = Process.GetCurrentProcess();
            return cp.MainModule!.BaseAddress;
        });
    public static nint MainHandleHandle => _handle.Value;
}
