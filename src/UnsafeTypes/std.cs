namespace Hosihikari.NativeInterop.UnsafeTypes;

#pragma warning disable CS8981
#pragma warning disable IDE1006 
#pragma warning disable CS0618

public static class std
{
    public static move_handle<T> move<T>(T _Right) where T : class, IDisposable, ICppInstance<T> => new(_Right);
}

#pragma warning restore CS0618
#pragma warning restore IDE1006 
#pragma warning restore CS8981 
