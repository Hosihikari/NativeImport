namespace Hosihikari.NativeInterop.Unmanaged;

public interface ICppInstanceNonGeneric : IDisposable
{
    /// <summary>
    /// noexcept
    /// </summary>
    public nint Pointer { get; set; }

    /// <summary>
    /// noexcept
    /// </summary>
    public bool IsOwner { get; set; }

    /// <summary>
    /// noexcept
    /// </summary>
    public bool IsTempStackValue { get; set; }

    public void Destruct();


    /// <summary>
    /// noexcept
    /// </summary>
    public static abstract ulong ClassSize { get; }

    public static abstract void DestructInstance(nint ptr);

    public static abstract object ConstructInstance(nint ptr, bool owns, bool isTempStackValue);
}

public interface ICppInstance<TSelf> : ICppInstanceNonGeneric
    where TSelf : class, ICppInstance<TSelf>
{
    public new static abstract TSelf ConstructInstance(nint ptr, bool owns, bool isTempStackValue);

    /// <summary>
    /// noexcept
    /// </summary>
    /// <param name="ins"></param>
    public static abstract implicit operator nint(TSelf ins);

    /// <summary>
    /// noexcept
    /// </summary>
    /// <param name="ins"></param>
    public static unsafe abstract implicit operator void*(TSelf ins);
}

public static class Utils
{
    public static T As<T>(this ICppInstanceNonGeneric @this, bool releaseSrc = false)
        where T : class, IDisposable, ICppInstance<T>
    {
        if (releaseSrc)
        {
            T temp = T.ConstructInstance(@this.Pointer, @this.IsOwner, @this.IsTempStackValue);
            @this.Pointer = 0;
            @this.IsOwner = false;
            @this.IsTempStackValue = false;

            @this.Dispose();

            return temp;
        }
        return T.ConstructInstance(@this.Pointer, false, true);
    }
}
