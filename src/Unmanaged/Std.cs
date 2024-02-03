namespace Hosihikari.NativeInterop.Unmanaged;

public static class Std
{
    public static MoveHandle<T> Move<T>(in T _Right)
        where T : class, IDisposable, ICppInstance<T>
    {
        return new(_Right);
    }
}