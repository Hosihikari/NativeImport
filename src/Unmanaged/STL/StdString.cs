using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Hosihikari.NativeInterop.Generation;
using Hosihikari.NativeInterop.Utils;

namespace Hosihikari.NativeInterop.Unmanaged.STL;

[PredefinedType(
    NativeTypeName = "basic_string<char, struct std::char_traits<char>, class std::allocator<char>>",
    NativeTypeNamespace = "std")]

[StructLayout(LayoutKind.Sequential)]
public unsafe struct StdString : IDisposable
{
#if WINDOWS

    private const int BufferSize = 16;

    [StructLayout(LayoutKind.Explicit)]
    public struct Storage
    {
        [FieldOffset(0)]
        public fixed byte buffer[BufferSize];

        [FieldOffset(0)]
        public byte* ptr;
    }

    //0
    public Storage storage;
    //16
    public ulong size;
    //24
    public ulong res;

    public void Clear()
    {
        if (res > BufferSize) HeapAlloc.Delete(storage.ptr);
        size = res = 0;
        storage.ptr = null;
    }

    public override string ToString()
    {
        fixed (byte* buf = storage.buffer)
        {
            return new string(res > BufferSize ? (sbyte*)storage.ptr : (sbyte*)buf, 0, (int)size);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AllocateByPtr(ref StdString @this, byte* ptr, ulong size)
    {
        if (size > BufferSize)
        {
            @this.storage.ptr = (byte*)HeapAlloc.New(size);
            Unsafe.CopyBlock(@this.storage.ptr, ptr, (uint)size * sizeof(byte));
        }
        else
        {
            fixed (byte* p = @this.storage.buffer)
                Unsafe.CopyBlock(p, ptr, (uint)size * sizeof(byte));
            @this.res = BufferSize;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly ulong CalculateGrowth(ulong newSize)
    {
        ulong num = res;

        ulong num2 = int.MaxValue / sizeof(byte);
        ulong num3 = num >> 1;
        if (num > num2 - num3)
        {
            return num2;
        }
        ulong num4 = num3 + num;
        return (num4 < newSize) ? newSize : num4;
    }

    private void EmplaceReallocate(ulong index, byte* ptr, ulong size)
    {
        fixed (byte* buf = storage.buffer)
        {
            var oldSize = size;
            var oldCap = res;
            var newSize = oldSize + size;
            var newCap = CalculateGrowth(newSize);

            byte* newStr = null;
            byte* oldStr = null;
            byte* temp = stackalloc byte[BufferSize];

            newStr = newCap > BufferSize ? (byte*)HeapAlloc.New(newCap) : temp;
            oldStr = oldCap > BufferSize ? storage.ptr : buf;

            byte* constructed = newStr;

            var indexLength = index;
            Unsafe.CopyBlock(constructed, oldStr, (uint)indexLength * sizeof(byte));
            constructed += indexLength;
            Unsafe.CopyBlock(constructed, ptr, (uint)size * sizeof(byte));
            constructed += size;
            Unsafe.CopyBlock(constructed, oldStr + indexLength, (uint)(oldSize - indexLength) * sizeof(byte));

            if (newCap > BufferSize)
            {
                storage.ptr = newStr;
                size = newSize;
                res = newCap;
            }
            else
            {
                Unsafe.CopyBlock(buf, newStr, (uint)newSize * sizeof(byte));
                size = newSize;
                res = BufferSize;
            }

            if (oldCap > BufferSize)
            {
                HeapAlloc.Delete(oldStr);
            }
        }
    }

    private void EmplaceWithUnusedCapacity(ulong index, byte* ptr, ulong size)
    {
        fixed (byte* buf = storage.buffer)
        {
            byte* str = null;
            var indexLength = index;
            str = res > BufferSize ? storage.ptr : buf;
            byte* temp = stackalloc byte[BufferSize];
            byte* buffer = size > BufferSize ? (byte*)HeapAlloc.New(size) : temp;

            Unsafe.CopyBlock(buffer, str + indexLength, (uint)(size - indexLength) * sizeof(byte));
            Unsafe.CopyBlock(str + indexLength + size, buffer, (uint)size * sizeof(byte));
            Unsafe.CopyBlock(str + indexLength, ptr, (uint)size * sizeof(byte));

            if (size > BufferSize) HeapAlloc.Delete(buffer);

            size += size;
        }
    }

    public StdString(byte* ptr, ulong size)
    {
        this.size = size;
        res = size;
        AllocateByPtr(ref this, ptr, size);
    }

    public StdString(string str)
    {
        var bytes = StringUtils.StringToManagedUtf8(str);
        res = size = (ulong)bytes.Length;

        fixed (byte* ptr = bytes)
            AllocateByPtr(ref this, ptr, size);
    }

    public StdString(ReadOnlySpan<byte> span)
    {
        res = size = (ulong)span.Length;
        fixed (byte* p = span)
        {
            AllocateByPtr(ref this, p, size);
        }
    }

    public StdString(in StdString str)
    {
        res = size = str.size;
        fixed (byte* ptr = str.Data)
            AllocateByPtr(ref this, ptr, str.size);
    }

    public StdString(MoveHandle<StdString> str)
    {
        Clear();

        StdString* target = str.Target;

        size = target->size;
        res = target->res;
        storage = target->storage;

        target->size = 0;
        target->res = 0;
        Unsafe.InitBlock(target->storage.buffer, 0, BufferSize);
    }

    public readonly ulong Size => size;

    public readonly ulong Capacity => res;

    public void Emplace(ulong index, in StdString str)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, size - 1);

        fixed (byte* _buffer = str.storage.buffer)
        {
            if (str.size > res - size)
                EmplaceReallocate(index, str.size > BufferSize ? str.storage.ptr : _buffer, str.size);
            else
                EmplaceWithUnusedCapacity(index, str.size > BufferSize ? str.storage.ptr : _buffer, str.size);
        }
    }

    public void EmplaceBack(in StdString str) => Emplace(Size - 1, str);

    public Span<byte> Data
    {
        get
        {
            if (size > BufferSize)
                return new(storage.ptr, (int)size);
            else
                fixed (byte* ptr = storage.buffer)
                    return new(ptr, (int)size);
        }
    }

    public readonly ulong Length => size;

    public void Append(in StdString str) => EmplaceBack(str);

    public void Append(string str) => EmplaceBack(new(str));

    public void Dispose() => Clear();

#else

    //0
    public byte* data;
    //8
    public ulong length;
    //16
    public StdString* unknownStringPtr;
    //24
    public void* unknownPtr;

    public void Dispose()
    {
        fixed (StdString* ptr = &this)
            LibNative.std_string_destructor(ptr);
    }

    public StdString()
    {
        fixed (StdString* ptr = &this)
        {
            LibNative.std_string_placement_new_default(ptr);
        }
    }

    public StdString(string str)
    {
        fixed (StdString* ptr = &this)
        {
            byte* data = Utf8StringMarshaller.ConvertToUnmanaged(str);
            LibNative.std_string_placement_new_c_style_str(ptr, data);
            Utf8StringMarshaller.Free(data);
        }
    }

    public StdString(in StdString str)
    {
        fixed (StdString* ptr = &this)
        fixed (StdString* right = &str)
        {
            LibNative.std_string_placement_new_copy(ptr, right);
        }
    }

    public StdString(MoveHandle<StdString> str)
    {
        fixed (StdString* ptr = &this)
            LibNative.std_string_placement_new_move(ptr, str.Target);
    }

    public ReadOnlySpan<byte> Data
    {
        get
        {
            fixed (StdString* ptr = &this)
            {
                byte* data = LibNative.std_string_data(ptr);
                ulong len = LibNative.std_string_length(ptr);
                if (data is null || len <= 0)
                {
                    return ReadOnlySpan<byte>.Empty;
                }
                return new ReadOnlySpan<byte>(ptr, (int)len);
            }
        }
    }

    public ulong Length
    {
        get
        {
            fixed (StdString* ptr = &this)
            {
                return LibNative.std_string_length(ptr);
            }
        }
    }

    public void Append(string str)
    {
        fixed (StdString* ptr = &this)
        {
            byte* data = Utf8StringMarshaller.ConvertToUnmanaged(str);
            LibNative.std_string_append(ptr, data);
            Utf8StringMarshaller.Free(data);
        }
    }

    public void Append(in StdString str)
    {
        fixed (StdString* ptr = &this)
        fixed (StdString* right = &str)
        {
            LibNative.std_string_append_std_string(ptr, right);
        }
    }

    public void Clear()
    {
        fixed (StdString* ptr = &this)
        {
            LibNative.std_string_clear(ptr);
        }
    }

    public override string ToString()
    {
        fixed (byte* data = Data)
            return Utf8StringMarshaller.ConvertToManaged(data) ?? string.Empty;
    }
#endif
}

//public unsafe partial class StdString :
//    IDisposable,
//    ICppInstance<StdString>,
//    IMoveableCppInstance<StdString>,
//    ICopyableCppInstance<StdString>,
//    IEnumerable<byte>
//{
//    [StructLayout(LayoutKind.Explicit, Size = 32)]
//    public readonly struct StdStringFiller : INativeTypeFiller<StdStringFiller, StdString>
//    {
//        static StdStringFiller()
//        {
//            if (sizeof(StdStringFiller) != 32) throw new InvalidOperationException();
//        }

//        [FieldOffset(0)]
//        private readonly long _alignment_member;

//        public static void Destruct(StdStringFiller* @this) => DestructInstance(new(@this));

//        public void Destruct()
//        {
//            fixed (StdStringFiller* ptr = &this)
//            {
//                Destruct(ptr);
//            }
//        }

//        public static implicit operator StdString(in StdStringFiller filler)
//        {
//            fixed (void* ptr = &filler)
//            {
//                return new StdString(ptr);
//            }
//        }
//    }

//    public static ulong ClassSize => LibNative.std_string_get_class_size();

//    public bool IsOwner { get => _isOwner; set => _isOwner = value; }
//    public nint Pointer { get => new(_pointer); set => _pointer = value.ToPointer(); }

//    public bool IsTempStackValue { get; set; }


//    public static StdString ConstructInstance(nint ptr, bool owns, bool isTempStackValue) => new(ptr, owns, isTempStackValue);

//    public static void DestructInstance(nint ptr) => LibNative.std_string_destructor(ptr.ToPointer());

//    static object ICppInstanceNonGeneric.ConstructInstance(nint ptr, bool owns, bool isTempStackValue) => ConstructInstance(ptr, owns, isTempStackValue);

//    public static StdString ConstructInstanceByCopy(StdString right) => new(right);
//    public static StdString ConstructInstanceByMove(MoveHandle<StdString> right) => new(right);


//    public static implicit operator nint(StdString str) => new(str._pointer);

//    public static implicit operator void*(StdString str) => str._pointer;

//    public void Destruct() => DestructInstance(this);

//    protected virtual void Dispose(bool disposing)
//    {
//        if (disposedValue) return;
//        if (_isOwner)
//        {
//            Destruct();
//            if (IsTempStackValue is false) LibNative.operator_delete(this);
//        }


//        disposedValue = true;
//    }

//    ~StdString() => Dispose(disposing: false);

//    public void Dispose()
//    {
//        Dispose(disposing: true);
//        GC.SuppressFinalize(this);
//    }


//    private void* _pointer;
//    //flag to determine if the pointer is created by this lib and should be deleted in the destructor
//    private bool _isOwner;
//    private bool disposedValue;

//    public StdString()
//    {
//        _pointer = LibNative.std_string_new();
//        _isOwner = true;
//        IsTempStackValue = false;
//    }

//    public StdString(string str)
//    {
//        fixed (byte* data = StringUtils.StringToManagedUtf8(str))
//        {
//            _pointer = LibNative.std_string_new_str(data);
//            _isOwner = true;
//        }
//    }

//    public StdString(StdString str)
//    {
//        if (str._pointer is null)
//        {
//            throw new NullReferenceException(nameof(str._pointer));
//        }

//        _pointer = LibNative.operator_new(ClassSize);
//        LibNative.std_string_placement_new_copy(_pointer, str);
//    }

//    public StdString(MoveHandle<StdString> str)
//    {
//        if (str.Target._pointer is null)
//        {
//            throw new NullReferenceException(nameof(str.Target._pointer));
//        }

//        _pointer = LibNative.operator_new(ClassSize);
//        LibNative.std_string_placement_new_move(_pointer, str.Target);
//    }

//    public StdString(nint pointer, bool isOwner = false, bool isTempStackValue = true)
//    {
//        _pointer = pointer.ToPointer();
//        _isOwner = isOwner;
//        IsTempStackValue = isTempStackValue;
//    }

//    public StdString(void* pointer)
//    {
//        _pointer = pointer;
//        _isOwner = false;
//        IsTempStackValue = true;
//    }

//    public ReadOnlySpan<byte> Data
//    {
//        get
//        {
//            byte* ptr = LibNative.std_string_data(_pointer);
//            ulong len = LibNative.std_string_length(_pointer);
//            if (ptr is null || len <= 0)
//            {
//                return ReadOnlySpan<byte>.Empty;
//            }
//            return new ReadOnlySpan<byte>(ptr, (int)len);
//        }
//    }

//    public ulong Length
//    {
//        get
//        {
//            return LibNative.std_string_length(_pointer);
//        }
//    }

//    public void Append(string str)
//    {
//        fixed (byte* data = StringUtils.StringToManagedUtf8(str))
//        {
//            LibNative.std_string_append(_pointer, data);
//        }
//    }

//    public void Append(StdString str)
//    {
//        LibNative.std_string_append_std_string(_pointer, str._pointer);
//    }

//    public void Clear()
//    {
//        LibNative.std_string_clear(_pointer);
//    }

//    public override string ToString()
//    {
//        return StringUtils.Utf8ToString(Data);
//    }

//    public IEnumerator<byte> GetEnumerator() => GetEnumerator();

//    IEnumerator IEnumerable.GetEnumerator() => new StdStringEnumerator();

//    private struct StdStringEnumerator : IEnumerator<byte>
//    {
//        private ulong currentOffset;
//        private readonly ulong length;
//        private readonly byte* data;

//        public StdStringEnumerator(StdString str)
//        {
//            data = LibNative.std_string_data(str);
//            currentOffset = ulong.MaxValue;
//            length = str.Length;
//        }

//        public readonly byte Current
//        {
//            get
//            {
//                if (currentOffset >= length)
//                {
//                    throw new IndexOutOfRangeException();
//                }
//                return data[currentOffset];
//            }
//        }

//        readonly object IEnumerator.Current => Current;

//        public readonly void Dispose()
//        {
//        }

//        public bool MoveNext() => ++currentOffset < length;

//        public void Reset() => currentOffset = ulong.MaxValue;
//    }
//}
