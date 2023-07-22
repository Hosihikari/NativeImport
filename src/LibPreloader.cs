using System.Runtime.InteropServices;

namespace Loader;

internal static class LibPreloader
{
    const string libname = "libpreloader";

    [DllImport(libname)]
    internal static extern unsafe int hook(nint rva, void* hook, out void* org);
}
