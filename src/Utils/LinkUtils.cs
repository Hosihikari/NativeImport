using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Hosihikari.NativeInterop.LibLoader;

namespace Hosihikari.NativeInterop.Utils;

public static class LinkUtils
{
    private const int Eintr = 4;
    private const int Eexist = 17;

    public static void CreateFileSymlink(string symlink, string pointingTo)
    {
        if (symlink == null)
            throw new ArgumentNullException(nameof(symlink));
        if (pointingTo == null)
            throw new ArgumentNullException(nameof(pointingTo));

        if (!File.Exists(pointingTo))
            throw new FileNotFoundException(new FileNotFoundException().Message, pointingTo);

        CreateSymlinkInternal(symlink, target: pointingTo);
    }

    public static void CreateDirectorySymlink(string symlink, string pointingTo)
    {
        if (symlink == null)
            throw new ArgumentNullException(nameof(symlink));
        if (pointingTo == null)
            throw new ArgumentNullException(nameof(pointingTo));

        if (!Directory.Exists(pointingTo))
            throw new DirectoryNotFoundException();

        CreateSymlinkInternal(symlink, target: pointingTo);
    }

    static void CreateSymlinkInternal(string symlink, string target)
    {
        if (symlink == target)
            throw new ArgumentException("Source and Target are the same");
        while (true)
        {
            if (LibcHelper.symlink(symlink: symlink, target: target) == 0)
                return;
            int errno = Marshal.GetLastWin32Error();
            switch (errno)
            {
                case Eintr:
                    //retry
                    continue;
                case Eexist:
                    throw new IOException("File already exists");
            }
            int hResult = ParseHResult(errno);
            Marshal.ThrowExceptionForHR(hResult);
        }
    }

    static int ParseHResult(int errno) =>
        (errno & 0x80000000) == 0x80000000
            ? errno
            : (errno & 0x0000FFFF) | unchecked((int)0x80070000);

    private const int BufferSize = 4097; // PATH_MAX is (usually?) 4096.

    public static string ReadLink(string symlinkPath)
    {
        var symlinkSize = Encoding.UTF8.GetByteCount(symlinkPath);
        var symlink = ArrayPool<byte>.Shared.Rent(symlinkSize + 1);
        var buffer = ArrayPool<byte>.Shared.Rent(BufferSize);
        try
        {
            Encoding.UTF8.GetBytes(symlinkPath, 0, symlinkPath.Length, symlink, 0);
            symlink[symlinkSize] = 0;

            var size = LibcHelper.readlink(symlink, buffer, BufferSize);
            return Encoding.UTF8.GetString(buffer, 0, (int)size);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(symlink);
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public static bool IsLink(string path)
    {
        var attributes = File.GetAttributes(path);
        return (attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint;
    }

    public static void Unlink(string path)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path));

        while (true)
        {
            if (LibcHelper.unlink(path) == 0)
                return;
            int errno = Marshal.GetLastWin32Error();
            if (errno == Eintr)
                continue; //retry
            int hResult = ParseHResult(errno);
            Marshal.ThrowExceptionForHR(hResult);
        }
    }
}
