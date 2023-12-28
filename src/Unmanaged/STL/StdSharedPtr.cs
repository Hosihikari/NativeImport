using Hosihikari.NativeInterop.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hosihikari.NativeInterop.Unmanaged.STL;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CxxSharedPtr : ITypeReferenceProvider
{
    public void* ptr;

    public void* ctr;

#if WINDOWS
    [GeneratedRegex("^class std::shared_ptr<(?<class_type>.*)>")]
    internal static partial Regex StdSharedPtrRegex();
#else
    internal static Regex StdVectorRegex() => throw new NotImplementedException();
#endif

    public static Regex Regex => StdSharedPtrRegex();

    public static Type? Matched(Match match) => typeof(CxxSharedPtr);

    public readonly void* Target<T>() where T : class, ICppInstance<T>
        => T.ConstructInstance((nint)ptr, false, true);
}

//public class StdSharedPtr<T>
//    where T : class, ICppInstance<T>
//{

//}
