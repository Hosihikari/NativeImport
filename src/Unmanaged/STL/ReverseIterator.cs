using Hosihikari.NativeInterop.Unmanaged.STL.Interface;
using System.Numerics;

namespace Hosihikari.NativeInterop.Unmanaged.STL;

public struct ReverseIterator<TIterator, T>(TIterator current) :
    IStdIterator<ReverseIterator<TIterator, T>, T>,
    IIncrementOperators<ReverseIterator<TIterator, T>>,
    IDecrementOperators<ReverseIterator<TIterator, T>>,
    IEqualityOperators<ReverseIterator<TIterator, T>, ReverseIterator<TIterator, T>, bool>
    where T : unmanaged
    where TIterator :
    unmanaged,
    IStdIterator<TIterator, T>,
    IIncrementOperators<TIterator>,
    IDecrementOperators<TIterator>,
    IEqualityOperators<TIterator, TIterator, bool>
{
    private TIterator current = current;

    public readonly TIterator Base
    {
        get => current;
    }

    public ref T Target => ref current.Target;

    public readonly override bool Equals(object? obj)
    {
        return obj is ReverseIterator<TIterator, T> iterator &&
               EqualityComparer<TIterator>.Default.Equals(current, iterator.current);
    }

    public readonly override int GetHashCode()
    {
        return HashCode.Combine(current);
    }

    public static ReverseIterator<TIterator, T> operator ++(ReverseIterator<TIterator, T> value) => new(--value.current);

    public static ReverseIterator<TIterator, T> operator --(ReverseIterator<TIterator, T> value) => new(++value.current);

    public static bool operator ==(ReverseIterator<TIterator, T> left, ReverseIterator<TIterator, T> right) => left.current == right.current;

    public static bool operator !=(ReverseIterator<TIterator, T> left, ReverseIterator<TIterator, T> right) => left.current != right.current;
}
