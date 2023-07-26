using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.UnsafeTypes;

#pragma warning disable CS8981
#pragma warning disable IDE1006 

public readonly unsafe struct pointer<T> where T : class, ICppInstance<T>
{
    private readonly nint ptr;

    public readonly T Target => T.ConstructInstance(ptr, false);

    private pointer(nint ptr) => this.ptr = ptr;

    public static implicit operator pointer<T>(T ins) => new(ins.Pointer);

    public static explicit operator T(pointer<T> ptr) => ptr.Target;

    public static explicit operator pointer<T>(nint ptr) => new(ptr);

    public static implicit operator nint(pointer<T> ptr) => ptr.ptr;

    public readonly void Delete()
    {
        if (ptr is not 0)
        {
            T.DestructInstance(ptr);
            NativeMemory.Free((void*)ptr);
        }
    }
}

#pragma warning restore IDE1006 
#pragma warning restore CS8981 
