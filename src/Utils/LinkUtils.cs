using System.Runtime.InteropServices;

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
            if (LibLoader.LibcHelper.symlink(symlink: symlink, target: target) == 0)
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
}
