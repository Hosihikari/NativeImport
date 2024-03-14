namespace Hosihikari.NativeInterop.Unmanaged;

public readonly unsafe struct Reference<T> where T : class, ICppInstance<T>
{
    private readonly nint _ptr;
    public readonly T Target => T.ConstructInstance(_ptr, false, false);

    private Reference(nint ptr)
    {
        _ptr = ptr;
    }

    public static implicit operator Reference<T>(T ins)
    {
        return new(ins.Pointer);
    }

    public static implicit operator T(Reference<T> ptr)
    {
        return ptr.Target;
    }

    public static explicit operator Reference<T>(nint ptr)
    {
        return new(ptr);
    }

    public static implicit operator nint(Reference<T> ptr)
    {
        return ptr._ptr;
    }

    public Reference<U> As<U>() where U : class, ICppInstance<U>
    {
        return (Reference<U>)_ptr;
    }
}