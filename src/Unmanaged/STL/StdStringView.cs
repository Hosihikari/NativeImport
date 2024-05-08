using Hosihikari.NativeInterop.Generation;
using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.Unmanaged.STL;

[PredefinedType(TypeName = "class basic_string_view<char, struct std::char_traits<char>>")]
[StructLayout(LayoutKind.Sequential)]
public unsafe struct StdStringView
{
    public byte* ptr;
    public ulong length;
}