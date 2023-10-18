namespace Hosihikari.NativeInterop.Unmanaged;

public readonly unsafe struct Reference<T> where T : class, ICppInstance<T>
{
    private readonly nint _ptr;

    public readonly T Target => T.ConstructInstance(_ptr, false, false);

    private Reference(nint ptr) => _ptr = ptr;

    public static implicit operator Reference<T>(T ins) => new(ins.Pointer);

    public static implicit operator T(Reference<T> ptr) => ptr.Target;

    public static explicit operator Reference<T>(nint ptr) => new(ptr);

    public static implicit operator nint(Reference<T> ptr) => ptr._ptr;
}
