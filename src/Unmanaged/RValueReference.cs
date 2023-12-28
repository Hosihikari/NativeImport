namespace Hosihikari.NativeInterop.Unmanaged;

public readonly unsafe ref struct RValueReference<T> where T : class, ICppInstance<T>
{
    private readonly nint _ptr;

    public T Target => T.ConstructInstance(_ptr, false, false);

    private RValueReference(nint ptr) => _ptr = ptr;

    public static implicit operator RValueReference<T>(in MoveHandle<T> handle) => new(handle.Target->Pointer);
}
