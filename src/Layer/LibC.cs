using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.Layer;

public static partial class LibC
{
#if !WINDOWS
    private const string LibName = "c";

    [LibraryImport(LibName, SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial int Symlink(string target, string symlink);

    [LibraryImport(LibName, SetLastError = true)]
    public static partial long Readlink(
        [MarshalAs(UnmanagedType.LPArray)] byte[] filename,
        [MarshalAs(UnmanagedType.LPArray)] byte[] buffer,
        long len
    );

    [LibraryImport(LibName, SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial int Unlink(string path);
#endif
}
