using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using size_t = System.UInt64;

namespace Hosihikari.NativeInterop.NativeTypes;

[StructLayout(LayoutKind.Sequential)]
internal unsafe ref struct CxxVectorDesc
{
    public void* begin;

    public void* end;

    //compressed_pair<pointer,allocator<T>>
    public void* end_cap;
}

public unsafe class StdVector<T> where T : unmanaged
{
    private readonly CxxVectorDesc* _pointer;
    private readonly bool _isOwner;

    private T* First { get => (T*)_pointer->begin; set => _pointer->begin = value; }
    private T* Last { get => (T*)_pointer->end; set => _pointer->end = value; }
    private T* End { get => (T*)_pointer->end_cap; set => _pointer->end_cap = value; }

    private static readonly size_t max_size = size_t.MaxValue / (size_t)sizeof(T);

    public StdVector(nint pointer)
    {
        _pointer = (CxxVectorDesc*)pointer.ToPointer();
        _isOwner = false;
    }

    public StdVector(void* pointer)
    {
        _pointer = (CxxVectorDesc*)pointer;
        _isOwner = false;
    }

    public StdVector()
    {
        _pointer = (CxxVectorDesc*)Marshal.AllocHGlobal(sizeof(CxxVectorDesc));
        _isOwner = true;
        Unsafe.InitBlock(_pointer, 0, (uint)sizeof(CxxVectorDesc));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public size_t Size() => (ulong)((Last - First) / sizeof(T));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public size_t Capacity() => (ulong)((End - First) / sizeof(T));

    public ref T this[size_t index]
    {
        get
        {
            if (index > Capacity() - 1)
                throw new IndexOutOfRangeException("index > Capacity() - 1");

            return ref First[index];
        }
    }

    private size_t CalculateGrowth(size_t newSize)
    {
        var oldCapacity = Capacity();
        if (oldCapacity > max_size - oldCapacity / 2)
            return max_size;

        var geometric = oldCapacity + oldCapacity / 2;

        if (geometric < newSize)
            return newSize;

        return geometric;
    }

    public ref T EmplaceBack(in T val)
    {
        if (Last != End)
        {
            var last = Last;
            Unsafe.Write(last, val);
            ++Last;
            return ref Unsafe.AsRef<T>(last);
        }
        else
        {
            var newCapacity = CalculateGrowth(Size() + 1);
            var oldSize = Size();
            var newVec = (T*)Marshal.AllocHGlobal((int)newCapacity * sizeof(T));
            Unsafe.CopyBlock(newVec, First, (uint)(((int)oldSize) * sizeof(T)));
            Unsafe.Write(newVec + oldSize, val);

            if (First is not null)
                Marshal.FreeHGlobal((nint)First);

            First = newVec;
            Last = newVec + oldSize + 1;
            End = newVec + newCapacity;
            return ref Unsafe.AsRef<T>(newVec + oldSize);
        }
    }
}
