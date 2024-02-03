using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.Unmanaged;

[StructLayout(LayoutKind.Explicit, Size = 8)]
public struct UnknownResult;

public struct Result<T> where T : class, ICppInstance<T>
{
    public T GetInstance()
    {
        if (Value == nint.Zero)
        {
            throw new InvalidOperationException("Null pointer.");
        }

        T ret = T.ConstructInstance(Value, true, true);
        Value = nint.Zero;
        return ret;
    }

    public readonly void Drop()
    {
        if (Value == nint.Zero)
        {
            return;
        }

        T.DestructInstance(Value);
    }

    public nint Value { get; private set; }
}