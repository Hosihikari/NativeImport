namespace Hosihikari.NativeInterop.Unmanaged;

internal interface ICopyableCppInstance<TSelf> where TSelf : class, ICppInstance<TSelf>
{
    static abstract TSelf ConstructInstanceByCopy(TSelf right);
}
