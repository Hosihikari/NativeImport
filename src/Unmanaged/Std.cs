namespace Hosihikari.NativeInterop.Unmanaged;

public static class Std
{
    public static MoveHandle<T> Move<T>(in T right)
        where T : class, IDisposable, ICppInstance<T>
    {
        return new(right);
    }
}