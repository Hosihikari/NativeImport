using Hosihikari.NativeInterop.Generation;
using Hosihikari.NativeInterop.Import;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace Hosihikari.NativeInterop.Unmanaged.STL;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct StdVector : ITypeReferenceProvider
{
    public void* begin;

    public void* end;

    //compressed_pair<pointer,allocator<T>>
    public void* end_cap;

    [SupportedOSPlatform("windows")]
    [GeneratedRegex("^class std::vector<(?<class_type>.*), class std::allocator<(\\k<class_type>)>>")]
    private static partial Regex StdVectorRegex();

    [SupportedOSPlatform("windows")] public static Regex Regex => StdVectorRegex();

    public static Type Matched(Match match)
    {
        return typeof(StdVector);
    }
}

public struct Unknown;

public sealed unsafe class StdVector<T> :
    ICppInstance<StdVector<T>>,
    IMoveableCppInstance<StdVector<T>>,
    ICopyableCppInstance<StdVector<T>>
    where T : unmanaged
{
    private static readonly bool IsFiller;
    private static readonly delegate* managed<T*, void> DtorFptr;
    private static readonly ulong s_maxSize = ulong.MaxValue / (ulong)sizeof(T);
    private bool _disposedValue;
    private StdVector* _pointer;

    static StdVector()
    {
        IsFiller = NativeTypeFillerHelper.TryGetDestructorFunctionPointer(out DtorFptr);
    }

    public StdVector(nint ptr, bool isOwner = false, bool isTempStackValue = true)
    {
        _pointer = (StdVector*)ptr.ToPointer();
        OwnsInstance = isOwner;
    }

    public StdVector(StdVector<T> vec)
    {
        StdVector* ptr = vec._pointer;
        if (ptr is null)
        {
            throw new NullReferenceException(nameof(vec._pointer));
        }

        ulong size = vec.Size();
        _pointer = HeapAlloc<StdVector>.New(default);
        First = HeapAlloc<T>.NewArray(size);
        Unsafe.CopyBlock(First, vec.First, (uint)size * (uint)sizeof(T));
        Last = First + size;
        End = Last + vec.Capacity();
        OwnsInstance = true;
        OwnsMemory = false;
    }

    // public StdVector(MoveHandle<StdVector<T>> vec)
    // {
    //     StdVectorDesc* ptr = vec.Target._pointer;
    //     if (ptr is null)
    //     {
    //         throw new NullReferenceException(nameof(vec.Target._pointer));
    //     }
    //
    //     Destruct();
    //     First = vec.Target.First;
    //     Last = vec.Target.Last;
    //     End = vec.Target.End;
    //
    //     vec.Target.First = null;
    //     vec.Target.Last = null;
    //     vec.Target.End = null;
    // }

    public StdVector(nint pointer)
    {
        _pointer = (StdVector*)pointer.ToPointer();
        OwnsInstance = false;
        OwnsMemory = true;
    }

    public StdVector(void* pointer)
    {
        _pointer = (StdVector*)pointer;
        OwnsInstance = false;
        OwnsMemory = true;
    }

    public StdVector()
    {
        _pointer = (StdVector*)Marshal.AllocHGlobal(sizeof(StdVector)).ToPointer();
        OwnsInstance = true;
        Unsafe.InitBlock(_pointer, 0, (uint)sizeof(StdVector));
    }

    private T* First
    {
        get => (T*)_pointer->begin;
        set => _pointer->begin = value;
    }

    private T* Last
    {
        get => (T*)_pointer->end;
        set => _pointer->end = value;
    }

    private T* End
    {
        get => (T*)_pointer->end_cap;
        set => _pointer->end_cap = value;
    }

    public ref T this[ulong index]
    {
        get
        {
            if (index > (Capacity() - 1))
            {
                throw new IndexOutOfRangeException("Index is greater than Capacity minus one");
            }

            return ref First[index];
        }
    }

    public static StdVector<T> ConstructInstanceByCopy(StdVector<T> right)
    {
        return new(right);
    }

    public static ulong ClassSize => 24ul;
    public bool OwnsInstance { get; set; }
    public bool OwnsMemory { get; set; }

    public nint Pointer
    {
        get => new(_pointer);
        set => _pointer = (StdVector*)value.ToPointer();
    }

    public static StdVector<T> ConstructInstance(nint ptr, bool owns, bool isTempStackValue)
    {
        return new(ptr, owns, isTempStackValue);
    }

    public static void DestructInstance(nint ptr)
    {
        StdVector* p = (StdVector*)ptr.ToPointer();
        if (IsFiller)
        {
            using StdVector<T> vector = new(ptr);
            ulong size = vector.Size();
            for (ulong i = 0; i < size; ++i)
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

    static object ICppInstanceNonGeneric.ConstructInstance(nint ptr, bool owns, bool isTempStackValue)
    {
        return ConstructInstance(ptr, owns, isTempStackValue);
    }

    public static implicit operator nint(StdVector<T> vec)
    {
        return new(vec._pointer);
    }

    public static implicit operator void*(StdVector<T> vec)
    {
        return vec._pointer;
    }

    public void Destruct()
    {
        DestructInstance(this);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public static StdVector<T> ConstructInstanceByMove(MoveHandle<StdVector<T>> right)
    {
        return new(right.Target);
    }

    private void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (OwnsInstance)
        {
            Destruct();
            LibNative.operator_delete(this);
        }

        _disposedValue = true;
    }

    ~StdVector()
    {
        Dispose(false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong Size()
    {
        return (ulong)((Last - First) / sizeof(T));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong Capacity()
    {
        return (ulong)((End - First) / sizeof(T));
    }

    private ulong CalculateGrowth(ulong newSize)
    {
        ulong oldCapacity = Capacity();
        if (oldCapacity > (s_maxSize - (oldCapacity / 2)))
        {
            return s_maxSize;
        }

        ulong geometric = oldCapacity + (oldCapacity / 2);
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

        ulong newCapacity = CalculateGrowth(Size() + 1);
        ulong oldSize = Size();
        T* newVec = (T*)Marshal.AllocHGlobal((int)newCapacity * sizeof(T)).ToPointer();
        Unsafe.CopyBlock(newVec, First, (uint)((int)oldSize * sizeof(T)));
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

    public struct StdVectorFiller : INativeTypeFiller<StdVectorFiller, StdVector<T>>
    {
        public StdVector StdVector;

        static StdVectorFiller()
        {
            if (sizeof(StdVectorFiller) is not 24)
            {
                throw new InvalidOperationException();
            }
        }

        public static void Destruct(StdVectorFiller* @this)
        {
            DestructInstance(new(@this));
        }

        public static implicit operator StdVector<T>(in StdVectorFiller filler)
        {
            fixed (void* ptr = &filler)
            {
                return new(ptr);
            }
        }
    }
}