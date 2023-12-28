namespace Hosihikari.NativeInterop.Unmanaged;

#pragma warning disable CS8500
public readonly unsafe ref struct MoveHandle<T>
{
    private readonly T* _instance;

    internal MoveHandle(in T val)
    {
        fixed (T* ptr = &val)
            _instance = ptr;
    }

    public T* Target => _instance;
}
