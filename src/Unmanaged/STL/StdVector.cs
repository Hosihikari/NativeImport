using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Hosihikari.NativeInterop.Generation;
using Hosihikari.NativeInterop.Layer;
using size_t = ulong;

namespace Hosihikari.NativeInterop.Unmanaged.STL;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CxxVector : ITypeReferenceProvider
{
    public void* begin;

    public void* end;

    //compressed_pair<pointer,allocator<T>>
    public void* end_cap;

#if WINDOWS
    [GeneratedRegex("^class std::vector<(?<class_type>.*), class std::allocator<(\\k<class_type>)>>")]
    internal static partial Regex StdVectorRegex();
#else
    internal static Regex StdVectorRegex() => throw new NotImplementedException();
#endif

    public static Regex Regex => StdVectorRegex();

    public static Type Matched(Match match) =>
        typeof(CxxVector);
}

public struct Unknown { }

public unsafe partial class StdVector<T> :
    IDisposable,
    ICppInstance<StdVector<T>>,
    IMoveableCppInstance<StdVector<T>>,
    ICopyableCppInstance<StdVector<T>>
    where T : unmanaged
{
    public struct StdVectorFiller : INativeTypeFiller<StdVectorFiller, StdVector<T>>
    {
        public CxxVector cxxVector;

        static StdVectorFiller()
        {
            if (sizeof(StdVectorFiller) != 24)
            {
                throw new InvalidOperationException();
            }
        }


        public static void Destruct(StdVectorFiller* @this) => DestructInstance(new(@this));

        public static implicit operator StdVector<T>(in StdVectorFiller filler)
        {
            fixed (void* ptr = &filler)
            {
                return new(ptr);
            }
        }
    }

    private static readonly bool IsFiller;
    private static readonly delegate* managed<T*, void> DtorFptr;
    static StdVector() => IsFiller = NativeTypeFillerHelper.TryGetDestructorFunctionPointer(out DtorFptr);

    public static size_t ClassSize => 24ul;

    public bool IsOwner { get; set; }

    public bool IsTempStackValue { get; set; }

    public nint Pointer { get => new(_pointer); set => _pointer = (CxxVector*)value.ToPointer(); }

    public static StdVector<T> ConstructInstance(nint ptr, bool owns, bool isTempStackValue) => new(ptr, owns, isTempStackValue);

    public static void DestructInstance(nint ptr)
    {
        CxxVector* p = (CxxVector*)ptr.ToPointer();

        if (IsFiller)
        {
            using StdVector<T> vector = new(ptr);
            size_t size = vector.Size();
            for (size_t i = 0; i < size; ++i)
            {
                fixed (T* currentPtr = &vector[i])
                {
                    DtorFptr(currentPtr);
                }
            }
        }

        if (p is not null)
        {
            LibNative.operator_delete(p);
        }

        p->begin = null;
        p->end = null;
        p->end_cap = null;
    }

    static object ICppInstanceNonGeneric.ConstructInstance(nint ptr, bool owns, bool isTempStackValue) => ConstructInstance(ptr, owns, isTempStackValue);

    public static StdVector<T> ConstructInstanceByCopy(StdVector<T> right) => new(right);
    public static StdVector<T> ConstructInstanceByMove(MoveHandle<StdVector<T>> right) => new(right.Target);


    public static implicit operator nint(StdVector<T> vec) => new(vec._pointer);

    public static implicit operator void*(StdVector<T> vec) => vec._pointer;

    public void Destruct() => DestructInstance(this);

    protected virtual void Dispose(bool disposing)
    {
        if (disposedValue)
        {
            return;
        }

        if (IsOwner)
        {
            Destruct();
            LibNative.operator_delete(this);
        }

        disposedValue = true;
    }

    ~StdVector() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private CxxVector* _pointer;
    private bool disposedValue;


    public StdVector(nint ptr, bool isOwner = false, bool isTempStackValue = true)
    {
        _pointer = (CxxVector*)ptr.ToPointer();
        IsOwner = isOwner;
    }

    public StdVector(StdVector<T> vec)
    {
        CxxVector* ptr = vec._pointer;
        if (ptr is null)
        {
            throw new NullReferenceException(nameof(vec._pointer));
        }

        size_t size = vec.Size();
        _pointer = HeapAlloc<CxxVector>.New(default);
        First = HeapAlloc<T>.NewArray(size);
        Unsafe.CopyBlock(First, vec.First, (uint)size * (uint)sizeof(T));
        Last = First + size;
        End = Last + vec.Capacity();

        IsOwner = true;
        IsTempStackValue = false;
    }

    public StdVector(MoveHandle<StdVector<T>> vec)
    {
        throw new NotImplementedException();

        //CxxVectorDesc* ptr = vec.Target._pointer;
        //if (ptr is null)
        //{
        //    throw new NullReferenceException(nameof(vec.Target._pointer));
        //}

        //Destruct();
        //First = vec.Target.First;
        //Last = vec.Target.Last;
        //End = vec.Target.End;

        //vec.Target.First = null;
        //vec.Target.Last = null;
        //vec.Target.End = null;
    }



    private T* First { get => (T*)_pointer->begin; set => _pointer->begin = value; }
    private T* Last { get => (T*)_pointer->end; set => _pointer->end = value; }
    private T* End { get => (T*)_pointer->end_cap; set => _pointer->end_cap = value; }

    private static readonly size_t max_size = size_t.MaxValue / (size_t)sizeof(T);

    public StdVector(nint pointer)
    {
        _pointer = (CxxVector*)pointer.ToPointer();
        IsOwner = false;
        IsTempStackValue = true;
    }

    public StdVector(void* pointer)
    {
        _pointer = (CxxVector*)pointer;
        IsOwner = false;
        IsTempStackValue = true;
    }

    public StdVector()
    {
        _pointer = (CxxVector*)Marshal.AllocHGlobal(sizeof(CxxVector)).ToPointer();
        IsOwner = true;
        Unsafe.InitBlock(_pointer, 0, (uint)sizeof(CxxVector));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public size_t Size() => (size_t)((Last - First) / sizeof(T));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public size_t Capacity() => (size_t)((End - First) / sizeof(T));

    public ref T this[size_t index]
    {
        get
        {
            if (index > Capacity() - 1)
            {
                throw new IndexOutOfRangeException("Index is greater than Capacity minus one");
            }

            return ref First[index];
        }
    }

    private size_t CalculateGrowth(size_t newSize)
    {
        size_t oldCapacity = Capacity();
        if (oldCapacity > max_size - (oldCapacity / 2))
        {
            return max_size;
        }

        size_t geometric = oldCapacity + (oldCapacity / 2);

        return geometric >= newSize ? geometric : newSize;
    }

    public ref T EmplaceBack(in T val)
    {
        if (Last != End)
        {
            T* last = Last;
            Unsafe.Write(last, val);
            ++Last;
            return ref Unsafe.AsRef<T>(last);
        }
        size_t newCapacity = CalculateGrowth(Size() + 1);
        size_t oldSize = Size();
        T* newVec = (T*)Marshal.AllocHGlobal((int)newCapacity * sizeof(T)).ToPointer();
        Unsafe.CopyBlock(newVec, First, (uint)(((int)oldSize) * sizeof(T)));
        Unsafe.Write(newVec + oldSize, val);

        if (First is not null)
        {
            Marshal.FreeHGlobal(new(First));
        }

        First = newVec;
        Last = newVec + oldSize + 1;
        End = newVec + newCapacity;
        return ref Unsafe.AsRef<T>(newVec + oldSize);
    }
}
