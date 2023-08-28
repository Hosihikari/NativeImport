using System.Buffers;
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
        if (symlink is null)
        {
            throw new ArgumentNullException(nameof(symlink));
        }
        if (pointingTo is null)
        {
            throw new ArgumentNullException(nameof(pointingTo));
        }

        if (!File.Exists(pointingTo))
        {
            throw new FileNotFoundException(new FileNotFoundException().Message, pointingTo);
        }

        CreateSymlinkInternal(symlink, pointingTo);
    }

    public static void CreateDirectorySymlink(string symlink, string pointingTo)
    {
        if (symlink is null)
        {
            throw new ArgumentNullException(nameof(symlink));
        }
        if (pointingTo is null)
        {
            throw new ArgumentNullException(nameof(pointingTo));
        }

        if (!Directory.Exists(pointingTo))
        {
            throw new DirectoryNotFoundException();
        }

        CreateSymlinkInternal(symlink, pointingTo);
    }

    static void CreateSymlinkInternal(string symlink, string target)
    {
        if (symlink == target)
        {
            throw new ArgumentException("Source and Target are the same");
        }
        while (LibcHelper.Symlink(symlink, target) != nint.Zero)
        {
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
        (errno & 0x80000000) is 0x80000000
            ? errno
            : (errno & 0x0000FFFF) | unchecked((int)0x80070000);

    private const int BufferSize = 4097; // PATH_MAX is (usually?) 4096.

    public static string ReadLink(string symlinkPath)
    {
        int symlinkSize = Encoding.UTF8.GetByteCount(symlinkPath);
        byte[] symlink = ArrayPool<byte>.Shared.Rent(symlinkSize + 1);
        byte[] buffer = ArrayPool<byte>.Shared.Rent(BufferSize);
        try
        {
            Encoding.UTF8.GetBytes(symlinkPath, 0, symlinkPath.Length, symlink, 0);
            symlink[symlinkSize] = 0;

            long size = LibcHelper.Readlink(symlink, buffer, BufferSize);
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
        FileAttributes attributes = File.GetAttributes(path);
        return (attributes & FileAttributes.ReparsePoint) is FileAttributes.ReparsePoint;
    }

    public static void Unlink(string path)
    {
        if (path is null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        while (LibcHelper.Unlink(path) != nint.Zero)
        {
            int errno = Marshal.GetLastWin32Error();
            if (errno is Eintr)
            {
                continue; //retry
            }
            int hResult = ParseHResult(errno);
            Marshal.ThrowExceptionForHR(hResult);
        }
    }
}
