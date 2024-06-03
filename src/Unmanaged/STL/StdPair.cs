namespace Hosihikari.NativeInterop.Unmanaged.STL;

public struct StdPair<TFirst, TSecond>(in TFirst first, in TSecond second)
    where TFirst : unmanaged
    where TSecond : unmanaged
{
    public TFirst First = first;
    public TSecond Second = second;
}
