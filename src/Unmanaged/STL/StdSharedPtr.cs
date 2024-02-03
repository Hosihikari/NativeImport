using Hosihikari.NativeInterop.Generation;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Hosihikari.NativeInterop.Unmanaged.STL;

[StructLayout(LayoutKind.Sequential)]
public unsafe
#if WINDOWS
    partial
#endif
    struct CxxSharedPtr : ITypeReferenceProvider
{
    public void* ptr;
    public void* ctr;
#if WINDOWS
    [GeneratedRegex("^class std::shared_ptr<(?<class_type>.*)>")]
    internal static partial Regex StdSharedPtrRegex();

    public static Regex Regex => StdSharedPtrRegex();
#else
    internal static Regex StdVectorRegex()
    {
        throw new NotImplementedException();
    }

    public static Regex Regex => StdVectorRegex();
#endif
    public static Type Matched(Match match)
    {
        return typeof(CxxSharedPtr);
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