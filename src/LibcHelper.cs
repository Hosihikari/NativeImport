using System.Runtime.InteropServices;

namespace NativeInterop;

public static class LibcHelper
{
    public const int RTLD_NOW = 0x002;
    const string libc = "libc";

    [DllImport(libc)]
    public static extern IntPtr dlsym(IntPtr handle, string symbol);

    [DllImport(libc)]
    public static extern IntPtr dlopen(string filename, int flags);

    [DllImport(libc)]
    public static extern int dlclose(IntPtr handle);
}
