
namespace Hosihikari.NativeInterop.Unmanaged.STL;

public unsafe static class StdTypeTraits_Char
{
    public const ulong _FNV_offset_basis = 14695981039346656037UL;
    public const ulong _FNV_prime = 1099511628211UL;
    public static ulong _Fnv1a_append_bytes(ulong _Val, byte* _First, ulong _Count)
    { // accumulate range [_First, _First + _Count) into partial FNV-1a hash _Val
        for (ulong _Idx = 0; _Idx < _Count; ++_Idx)
        {
            _Val ^= (ulong)(_First[_Idx]);
            _Val *= _FNV_prime;
        }

        return _Val;
    }

    public static ulong _Hash_array_representation<_Kty>(_Kty* _First, ulong _Count)
        where _Kty : unmanaged
    { // bitwise hashes the representation of an array
        return _Fnv1a_append_bytes(_FNV_offset_basis, (byte*)_First, _Count * (ulong)sizeof(_Kty));
    }

    public static int Compare(byte* _First1, byte* _First2, ulong _Count) => Memory.Memcmp(_First1, _First2, _Count);

    public static int _Traits_compare(byte* _Left, ulong _Left_size, byte* _Right, ulong _Right_size)
    {
        var _Ans = Compare(_Left, _Right, Math.Min(_Left_size, _Right_size));
        if (_Ans is not 0)
            return _Ans;

        if(_Left_size < _Right_size)
            return -1;

        if (_Left_size > _Right_size)
            return 1;

        return 0;
    }
}
