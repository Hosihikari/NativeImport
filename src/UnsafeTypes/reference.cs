namespace Hosihikari.NativeInterop.UnsafeTypes;

#pragma warning disable CS8981
#pragma warning disable IDE1006 

public readonly unsafe struct reference<T> where T : class, ICppInstance<T>
{
    private readonly nint ptr;

    public readonly T Target => T.ConstructInstance(ptr, false);

    private reference(nint ptr) => this.ptr = ptr;

    public static implicit operator reference<T>(T ins) => new(ins.Pointer);

    public static implicit operator T(reference<T> ptr) => ptr.Target;

    public static explicit operator reference<T>(nint ptr) => new(ptr);

    public static implicit operator nint(reference<T> ptr) => ptr.ptr;
}

#pragma warning restore IDE1006 
#pragma warning restore CS8981 
