using System.Runtime.CompilerServices;

namespace Hosihikari.NativeInterop.UnsafeTypes;

#pragma warning disable IDE1006 

public readonly unsafe struct value_pointer<T> where T : unmanaged
{
    private readonly nint ptr;

    public readonly ref T Target => ref Unsafe.AsRef<T>((void*)ptr);

    private value_pointer(nint ptr) => this.ptr = ptr;

    public static explicit operator value_pointer<T>(nint ptr) => new(ptr);

    public static implicit operator nint(value_pointer<T> ptr) => ptr.ptr;
}

#pragma warning restore IDE1006 
