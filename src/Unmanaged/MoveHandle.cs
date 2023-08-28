namespace Hosihikari.NativeInterop.Unmanaged;

public readonly ref struct MoveHandle<T> where T : class, ICppInstance<T>
{
    private readonly T _instance;

    internal MoveHandle(T val)
    {
        _instance = val;
    }

    public T Target => _instance;
}
