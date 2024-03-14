using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.Unmanaged;

[StructLayout(LayoutKind.Sequential)]
public readonly struct NativeBoolean
{
    private readonly byte value;

    private NativeBoolean(bool val)
    {
        value = val ? (byte)1 : (byte)0;
    }

    public static implicit operator bool(NativeBoolean val)
    {
        return val.value is not byte.MinValue;
    }

    public static implicit operator NativeBoolean(bool val)
    {
        return new(val);
    }
}