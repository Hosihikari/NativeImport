using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Hosihikari.NativeInterop.Import;

[UnsupportedOSPlatform("windows")]
public static partial class LibC
{
    private const string LibName = "c";

    [LibraryImport(LibName, EntryPoint = "symlink", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial int Symlink(string target, string symlink);

    [LibraryImport(LibName, EntryPoint = "readlink", SetLastError = true)]
    public static partial long Readlink(
        [MarshalAs(UnmanagedType.LPArray)] byte[] filename,
        [MarshalAs(UnmanagedType.LPArray)] byte[] buffer,
        long len
    );

    [LibraryImport(LibName, EntryPoint = "unlink", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial int Unlink(string path);
}