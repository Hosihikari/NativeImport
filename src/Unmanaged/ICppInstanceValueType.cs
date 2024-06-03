namespace Hosihikari.NativeInterop.Unmanaged;

public interface ICppInstanceValueType<TSelf>
    where TSelf : unmanaged
{
}

public interface ICopyableCppInstanceValueType<TSelf> : ICppInstanceValueType<TSelf>
    where TSelf : unmanaged
{
    TSelf Copy();
}
