namespace Hosihikari.NativeInterop.UnsafeTypes;

public interface ICppInstanceNonGeneric
{
    public nint Pointer { get; set; }
    public bool IsOwner { get; set; }

    public void Destruct();



    public static abstract ulong ClassSize { get; }

    public static abstract void DestructInstance(nint ptr);

    public static abstract object ConstructInstance(nint ptr, bool owns);
}

public interface ICppInstance<TSelf> : ICppInstanceNonGeneric
    where TSelf : class, ICppInstance<TSelf>
{
    public new static abstract TSelf ConstructInstance(nint ptr, bool owns);
}

public static class Utils
{
    public static U As<T, U>(this ICppInstance<T> @this, bool releaseSrc = false)
        where T : class, ICppInstance<T>
        where U : class, ICppInstance<U>
    {
        if (releaseSrc)
        {
            var temp = U.ConstructInstance(@this.Pointer, @this.IsOwner);
            @this.Pointer = 0;
            @this.IsOwner = false;

            if (@this is IDisposable ins)
                ins.Dispose();

            return temp;
        }
        else
        {
            return U.ConstructInstance(@this.Pointer, false);
        }
    }
}
