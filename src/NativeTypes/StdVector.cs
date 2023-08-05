using Hosihikari.NativeInterop.LibLoader;
using Hosihikari.NativeInterop.UnsafeTypes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using size_t = System.UInt64;

namespace Hosihikari.NativeInterop.NativeTypes;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct CxxVectorDesc
{
    public void* begin;

    public void* end;

    //compressed_pair<pointer,allocator<T>>
    public void* end_cap;
}

public unsafe class StdVector<T> :
    IDisposable,
    ICppInstance<StdVector<T>>,
    IMoveableCppInstance<StdVector<T>>,
    ICopyableCppInstance<StdVector<T>>/*,*/
    //IEnumerable<T>
    where T : unmanaged
{
    public static ulong ClassSize => 24ul;

    public bool IsOwner { get => _isOwner; set => _isOwner = value; }
    public nint Pointer { get => (nint)_pointer; set => _pointer = (CxxVectorDesc*)value; }

    public static StdVector<T> ConstructInstance(nint ptr, bool owns) => new(ptr, owns);

    public static void DestructInstance(nint ptr)
    {
        var p = (CxxVectorDesc*)ptr;
        if (p is not null)
            LibNative.operator_delete(p);

        p->begin = null;
        p->end = null;
        p->end_cap = null;
    }

    static object ICppInstanceNonGeneric.ConstructInstance(nint ptr, bool owns) => ConstructInstance(ptr, owns);

    public static StdVector<T> ConstructInstanceByCopy(StdVector<T> right) => new(right);
    public static StdVector<T> ConstructInstanceByMove(move_handle<StdVector<T>> right) => new(right);


    public static implicit operator nint(StdVector<T> vec) => (nint)vec._pointer;

    public static implicit operator void*(StdVector<T> vec) => vec._pointer;

    public void Destruct() => DestructInstance(this);

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {

            if (_isOwner)
            {
                Destruct();
                LibNative.operator_delete(this);
            }

            disposedValue = true;
        }
    }

    ~StdVector() => Dispose(disposing: false);

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private CxxVectorDesc* _pointer;
    private bool _isOwner;
    private bool disposedValue;


    public StdVector(nint ptr, bool isOwner = false)
    {
        _pointer = (CxxVectorDesc*)ptr;
        _isOwner = isOwner;
    }

    public StdVector(StdVector<T> vec)
    {
        var ptr = vec._pointer;
        if (ptr is null)
            throw new NullReferenceException(nameof(vec._pointer));

        var size = vec.Size();
        _pointer = heap_alloc<CxxVectorDesc>.New(default);
        First = heap_alloc<T>.NewArray(size);
        Unsafe.CopyBlock(First, vec.First, (uint)size * (uint)sizeof(T));
        Last = First + size;
        End = Last + vec.Capacity();
    }

    public StdVector(move_handle<StdVector<T>> vec)
    {
        var ptr = vec.Target._pointer;
        if (ptr is null)
            throw new NullReferenceException(nameof(vec.Target._pointer));

        Destruct();
        First = vec.Target.First;
        Last = vec.Target.Last;
        End = vec.Target.End;

        vec.Target.First = null;
        vec.Target.Last = null;
        vec.Target.End = null;
    }



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
