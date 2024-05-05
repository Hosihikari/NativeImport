namespace Hosihikari.NativeInterop.Unmanaged;

public interface ICppInstanceNonGeneric : IDisposable
{
    /// <summary>
    ///     noexcept
    /// </summary>
    public nint Pointer { get; set; }

    /// <summary>
    ///     noexcept
    /// </summary>
    public bool OwnsInstance { get; set; }

    /// <summary>
    ///     noexcept
    /// </summary>
    public bool OwnsMemory { get; set; }

    /// <summary>
    ///     noexcept
    /// </summary>
    public static abstract ulong ClassSize { get; }

    public void Destruct();
    public static abstract void DestructInstance(nint ptr);
    public static abstract object ConstructInstance(nint ptr, bool owns, bool isTempStackValue);
}

public interface ICppInstance<TSelf> : ICppInstanceNonGeneric
    where TSelf : class, ICppInstance<TSelf>
{
    public new static abstract TSelf ConstructInstance(nint ptr, bool owns, bool isTempStackValue);

    /// <summary>
    ///     noexcept
    /// </summary>
    /// <param name="ins"></param>
    public static abstract implicit operator nint(TSelf ins);

    /// <summary>
    ///     noexcept
    /// </summary>
    /// <param name="ins"></param>
    public static abstract unsafe implicit operator void*(TSelf ins);
}