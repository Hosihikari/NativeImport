namespace Hosihikari.NativeInterop.Unmanaged;

public readonly unsafe ref struct MoveHandle<T>
{
    internal MoveHandle(in T val)
    {
#pragma warning disable CS8500
        fixed (T* ptr = &val)
        {
            Target = ptr;
        }
    }

    public T* Target { get; }
#pragma warning restore CS8500
}