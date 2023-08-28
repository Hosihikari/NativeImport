using System.Runtime.CompilerServices;

namespace Hosihikari.NativeInterop.Unmanaged;

public readonly unsafe struct ValuePointer<T> where T : unmanaged
{
    private readonly nint _ptr;

    public readonly ref T Target => ref Unsafe.AsRef<T>(_ptr.ToPointer());

    private ValuePointer(nint ptr) => _ptr = ptr;

    public static explicit operator ValuePointer<T>(nint ptr) => new(ptr);

    public static implicit operator nint(ValuePointer<T> ptr) => ptr._ptr;
}
