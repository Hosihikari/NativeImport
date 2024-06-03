
using System.Numerics;

namespace Hosihikari.NativeInterop.Unmanaged.STL.Interface;

public interface IStdIterator<TIterator, T> :
    IEqualityOperators<TIterator, TIterator, bool>,
    IIncrementOperators<TIterator>,
    IDecrementOperators<TIterator>
    where TIterator : unmanaged, IStdIterator<TIterator, T>
    where T : unmanaged
{
    public ref T Target { get; }
}
