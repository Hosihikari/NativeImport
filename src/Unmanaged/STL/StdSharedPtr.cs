using Hosihikari.NativeInterop.Generation;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace Hosihikari.NativeInterop.Unmanaged.STL;

[PredefinedType(TypeName = "class std::shared_ptr", IgnoreTemplateArgs = true)]
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct StdSharedPtr
{
    public void* ptr;
    public void* ctr;

    public readonly void* Target<T>() where T : class, ICppInstance<T>
    {
        return T.ConstructInstance((nint)ptr, false, false);
    }
}

//public class StdSharedPtr<T>
//    where T : class, ICppInstance<T>
//{
//}