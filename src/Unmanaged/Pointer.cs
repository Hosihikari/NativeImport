using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.Unmanaged;

public readonly unsafe struct Pointer<T> where T : class, ICppInstance<T>
{
    private readonly nint _ptr;

    public readonly T Target => T.ConstructInstance(_ptr, false);

    private Pointer(nint ptr) => _ptr = ptr;

    public static implicit operator Pointer<T>(T ins) => new(ins.Pointer);

    public static explicit operator T(Pointer<T> ptr) => ptr.Target;

    public static explicit operator Pointer<T>(nint ptr) => new(ptr);

    public static implicit operator nint(Pointer<T> ptr) => ptr._ptr;

    public readonly void Delete()
    {
        if (_ptr is not 0)
        {
            T.DestructInstance(_ptr);
            NativeMemory.Free((void*)_ptr);
        }
    }
}
