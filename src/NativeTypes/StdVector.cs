using System.Runtime.CompilerServices;

namespace Hosihikari.NativeInterop.NativeTypes;

public class StdVector<TStruct> : StdVectorBase
    where TStruct : struct
{
    private static int GetItemSize() => Unsafe.SizeOf<TStruct>();

    public StdVector()
        : base(GetItemSize()) { }

    public StdVector(IntPtr pointer)
        : base(pointer, GetItemSize()) { }

    public unsafe StdVector(void* pointer)
        : base(pointer, GetItemSize()) { }
}

public class StdVectorBase
{
    private readonly unsafe void* _pointer;

    // size of each item in the vector
    private readonly int _itemSize;

    //flag to determine if the pointer is created by this lib and should be deleted in the destructor
    private readonly bool _isOwner;

    public StdVectorBase(int itemSize)
    {
        unsafe
        {
            _pointer = LibLoader.LibNative.std_vector_new();
            _isOwner = true;
        }
        _itemSize = itemSize;
    }

    public unsafe StdVectorBase(nint pointer, int itemSize)
    {
        _pointer = pointer.ToPointer();
        _isOwner = false;
        _itemSize = itemSize;
    }

    public unsafe StdVectorBase(void* pointer, int itemSize)
    {
        _pointer = pointer;
        _isOwner = false;
        _itemSize = itemSize;
    }

    ~StdVectorBase()
    {
        unsafe
        {
            if (_isOwner)
            {
                LibLoader.LibNative.std_vector_delete(_pointer);
            }
        }
    }

    public void RawPushBack(ReadOnlySpan<byte> data)
    {
        unsafe
        {
            fixed (byte* dataPtr = data)
            {
                LibLoader.LibNative.std_vector_push_back(_pointer, dataPtr, data.Length);
            }
        }
    }

    public ReadOnlySpan<byte> RawData
    {
        get
        {
            unsafe
            {
                return new ReadOnlySpan<byte>(
                    LibLoader.LibNative.std_vector_data(_pointer),
                    LibLoader.LibNative.std_vector_size(_pointer)
                );
            }
        }
    }

    public int RawSize
    {
        get
        {
            unsafe
            {
                return LibLoader.LibNative.std_vector_size(_pointer);
            }
        }
    }

    public unsafe ReadOnlySpan<byte> RawAt(int index)
    {
        return new ReadOnlySpan<byte>(
            LibLoader.LibNative.std_vector_at(_pointer, index),
            _itemSize
        );
    }

    protected unsafe void* InternalRawFront => LibLoader.LibNative.std_vector_front(_pointer);
    protected unsafe void* InternalRawBack => LibLoader.LibNative.std_vector_back(_pointer);
    //public unsafe ReadOnlySpan<byte> RawFront =>
    //    new(LibLoader.LibNative.std_vector_front(_pointer), _itemSize);

    //public unsafe ReadOnlySpan<byte> RawBack =>
    //    new(LibLoader.LibNative.std_vector_back(_pointer), _itemSize);
}
