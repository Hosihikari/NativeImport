namespace Hosihikari.NativeInterop.Unmanaged;

public static class Std
{
    public static MoveHandle<T> Move<T>(T _Right)
        where T : class, IDisposable, ICppInstance<T> =>
        new(_Right);
}
