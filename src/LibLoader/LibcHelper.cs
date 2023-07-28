using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.LibLoader;

public static class LibcHelper
{
    public const int RTLD_NOW = 0x002;
    const string libc = "libc";

    [DllImport(libc)]
    public static extern nint dlsym(nint handle, string symbol);

    [DllImport(libc)]
    public static extern nint dlopen(string filename, int flags);

    [DllImport(libc)]
    public static extern int dlclose(nint handle);

    [DllImport(libc, SetLastError = true)]
    public static extern int symlink(string target, string symlink);

    [DllImport(libc)]
    public static extern IntPtr strerror(int errnum);

    [DllImport(libc, SetLastError = true)]
    public static extern long readlink(
        [MarshalAs(UnmanagedType.LPArray)] byte[] filename,
        [MarshalAs(UnmanagedType.LPArray)] byte[] buffer,
        long len
    );

    [DllImport(libc, SetLastError = true)]
    public static extern int unlink(string path);
}
