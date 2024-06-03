using System.Numerics;
using System.Runtime.CompilerServices;

namespace Hosihikari.NativeInterop.Unmanaged.STL;


public readonly ref struct StdLess<T>
    where T : unmanaged, IComparisonOperators<T, T, bool>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Compare(in T left, in T right) => left < right;
}

public readonly ref struct StdLess
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Compare<T>(in T left, in T right)
    where T : unmanaged, IComparisonOperators<T, T, bool>
        => left < right;
}
