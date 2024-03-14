using Hosihikari.NativeInterop.Import;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace Hosihikari.NativeInterop.Utils;

[UnsupportedOSPlatform("windows")]
public static class LinkUtils
{
    private const int Eintr = 4;
    private const int Eexist = 17;

    private const int BufferSize = 4097; // PATH_MAX is (usually?) 4096.

    public static void CreateFileSymlink(string symlink, string pointingTo)
    {
        if (!File.Exists(pointingTo))
        {
            throw new FileNotFoundException(default, pointingTo);
        }

        CreateSymlinkInternal(symlink, pointingTo);
    }

    public static void CreateDirectorySymlink(string symlink, string pointingTo)
    {
        if (!Directory.Exists(pointingTo))
        {
            throw new DirectoryNotFoundException();
        }

        CreateSymlinkInternal(symlink, pointingTo);
    }

    private static void CreateSymlinkInternal(string symlink, string target)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(symlink);
        ArgumentException.ThrowIfNullOrWhiteSpace(target);
        if (symlink == target)
        {
            throw new ArgumentException("Source and Target are the same");
        }

        while (LibC.Symlink(symlink, target) != nint.Zero)
        {
            int errno = Marshal.GetLastPInvokeError();
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

    private static int ParseHResult(int errno)
    {
        return (errno & 0x80000000) is 0x80000000
            ? errno
            : (errno & 0x0000FFFF) | unchecked((int)0x80070000);
    }

    public static string ReadLink(string symlinkPath)
    {
        int symlinkSize = Encoding.UTF8.GetByteCount(symlinkPath);
        byte[] symlink = ArrayPool<byte>.Shared.Rent(symlinkSize + 1);
        byte[] buffer = ArrayPool<byte>.Shared.Rent(BufferSize);
        try
        {
            Encoding.UTF8.GetBytes(symlinkPath, 0, symlinkPath.Length, symlink, 0);
            symlink[symlinkSize] = 0;
            long size = LibC.Readlink(symlink, buffer, BufferSize);
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
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        while (LibC.Unlink(path) != nint.Zero)
        {
            int errno = Marshal.GetLastPInvokeError();
            if (errno is Eintr)
            {
                continue; //retry
            }

            int hResult = ParseHResult(errno);
            Marshal.ThrowExceptionForHR(hResult);
        }
    }
}