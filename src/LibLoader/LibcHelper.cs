using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.LibLoader;

public static partial class LibcHelper
{
    public const int RTLD_NOW = 0x002;
    private const string LibName = "libc";

    [LibraryImport(LibName, StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint Dlsym(nint handle, string symbol);

    [LibraryImport(LibName, StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint Dlopen(string filename, int flags);

    [LibraryImport(LibName)]
    public static partial int Dlclose(nint handle);

    [LibraryImport(LibName, SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial int Symlink(string target, string symlink);

    [LibraryImport(LibName)]
    public static partial IntPtr Strerror(int errnum);

    [LibraryImport(LibName, SetLastError = true)]
    public static partial long Readlink(
        [MarshalAs(UnmanagedType.LPArray)] byte[] filename,
        [MarshalAs(UnmanagedType.LPArray)] byte[] buffer,
        long len
    );

    [LibraryImport(LibName, SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial int Unlink(string path);
}
