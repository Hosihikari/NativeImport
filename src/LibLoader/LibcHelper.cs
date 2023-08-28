using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.LibLoader;

public static class LibcHelper
{
    public const int RTLD_NOW = 0x002;
    const string libc = "libc";

    [DllImport(libc)]
    public static extern nint Dlsym(nint handle, string symbol);

    [DllImport(libc)]
    public static extern nint Dlopen(string filename, int flags);

    [DllImport(libc)]
    public static extern int Dlclose(nint handle);

    [DllImport(libc, SetLastError = true)]
    public static extern int Symlink(string target, string symlink);

    [DllImport(libc)]
    public static extern IntPtr Strerror(int errnum);

    [DllImport(libc, SetLastError = true)]
    public static extern long Readlink(
        [MarshalAs(UnmanagedType.LPArray)] byte[] filename,
        [MarshalAs(UnmanagedType.LPArray)] byte[] buffer,
        long len
    );

    [DllImport(libc, SetLastError = true)]
    public static extern int Unlink(string path);
}
