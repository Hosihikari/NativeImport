namespace Hosihikari.NativeInterop.Unmanaged;

public struct UnexploredType<T> where T : class, ICppInstance<T>
{
    static UnexploredType() => throw new InvalidOperationException();
}