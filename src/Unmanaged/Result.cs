using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.Unmanaged;

public unsafe struct Result<T> where T : class, ICppInstance<T>
{
    private nint _ptr;

    public T GetInstance()
    {
        if (_ptr == IntPtr.Zero)
            throw new InvalidOperationException("Null pointer.");

        var ret =T.ConstructInstance(_ptr, true, true);
        _ptr = IntPtr.Zero;
        return ret;
    }

    public readonly void Drop()
    {
        if (_ptr == nint.Zero)
        {
            return;
        }
        T.DestructInstance(_ptr);
    }
}

