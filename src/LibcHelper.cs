using System.Runtime.InteropServices;

namespace Loader;

internal static class LibcHelper
{
    internal const int RTLD_NOW = 0x002;
    const string libc = "libc";

    [DllImport(libc)]
    internal static extern IntPtr dlsym(IntPtr handle, string symbol);

    [DllImport(libc)]
    internal static extern IntPtr dlopen(string filename, int flags);

    [DllImport(libc)]
    internal static extern int dlclose(IntPtr handle);
}
