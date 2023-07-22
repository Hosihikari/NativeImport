using System.Runtime.InteropServices;

namespace Loader;

public static class LibPreloader
{
    const string libname = "libpreloader";

    [DllImport(libname, EntryPoint = "hook", ExactSpelling = true)]
    public static extern unsafe int Hook(void* rva, void* hook, out void* org);

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
}
