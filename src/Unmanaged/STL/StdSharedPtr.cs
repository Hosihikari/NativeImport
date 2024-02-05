using Hosihikari.NativeInterop.Generation;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Hosihikari.NativeInterop.Unmanaged.STL;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct StdSharedPtr : ITypeReferenceProvider
{
    public void* ptr;
    public void* ctr;

    [GeneratedRegex("^class std::shared_ptr<(?<class_type>.*)>")]
    private static partial Regex WinStdSharedPtrRegex();

    private static Regex StdSharedPtrRegex()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? WinStdSharedPtrRegex()
            : throw new NotImplementedException();
    }

    public static Regex Regex => StdSharedPtrRegex();

    public static Type Matched(Match match)
    {
        return typeof(StdSharedPtr);
    }

    public readonly void* Target<T>() where T : class, ICppInstance<T>
    {
        return T.ConstructInstance((nint)ptr, false, true);
    }
}

//public class StdSharedPtr<T>
//    where T : class, ICppInstance<T>
//{
//}