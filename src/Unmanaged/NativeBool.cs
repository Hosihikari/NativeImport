using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.Unmanaged;

[StructLayout(LayoutKind.Sequential)]
internal readonly ref struct NativeBool
{
    private readonly byte value;

    private NativeBool(bool val) => value = val ? (byte)1 : (byte)0;

    public static implicit operator bool(NativeBool val) => val.value is not byte.MinValue;
    public static implicit operator NativeBool(bool val) => new(val);
}
