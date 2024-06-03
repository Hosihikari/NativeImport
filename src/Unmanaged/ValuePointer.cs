using System.Runtime.CompilerServices;

namespace Hosihikari.NativeInterop.Unmanaged;

public readonly unsafe struct ValuePointer<T> where T : unmanaged
{
    private readonly nint _ptr;
    public ref T Target => ref Unsafe.AsRef<T>(_ptr.ToPointer());
    public T* Pointer => (T*)_ptr;

    private ValuePointer(nint ptr)
    {
        _ptr = ptr;
    }

    public ValuePointer(T* ptr)
    {
        _ptr = (nint)ptr;
    }

    public static explicit operator ValuePointer<T>(nint ptr)
    {
        return new(ptr);
    }

    public static implicit operator nint(ValuePointer<T> ptr)
    {
        return ptr._ptr;
    }

    public static implicit operator ValuePointer<T>(T* ptr)
    {
        return new(ptr);
    }

    public static implicit operator T*(ValuePointer<T> ptr)
    {
        return ptr.Pointer;
    }

    public bool IsNull
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _ptr is 0;
    }
}