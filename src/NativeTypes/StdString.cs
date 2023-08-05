using ELFSharp.MachO;
using Hosihikari.NativeInterop.LibLoader;
using Hosihikari.NativeInterop.UnsafeTypes;
using Hosihikari.NativeInterop.Utils;
using System.Collections;
using System.Collections.Specialized;

namespace Hosihikari.NativeInterop.NativeTypes;

public unsafe class StdString :
    IDisposable,
    ICppInstance<StdString>,
    IMoveableCppInstance<StdString>,
    ICopyableCppInstance<StdString>,
    IEnumerable<byte>
{
    public static ulong ClassSize => LibNative.std_string_get_class_size();

    public bool IsOwner { get => _isOwner; set => _isOwner = value; }
    public nint Pointer { get => (nint)_pointer; set => _pointer = (void*)value; }

    public static StdString ConstructInstance(nint ptr, bool owns) => new(ptr, owns);

    public static void DestructInstance(nint ptr) => LibNative.std_string_destructor((void*)ptr);

    static object ICppInstanceNonGeneric.ConstructInstance(nint ptr, bool owns) => ConstructInstance(ptr, owns);

    public static StdString ConstructInstanceByCopy(StdString right) => new(right);
    public static StdString ConstructInstanceByMove(move_handle<StdString> right) => new(right);


    public static implicit operator nint(StdString str) => (nint)str._pointer;

    public static implicit operator void*(StdString str) => str._pointer;

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

    ~StdString() => Dispose(disposing: false);

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }


    private void* _pointer;
    //flag to determine if the pointer is created by this lib and should be deleted in the destructor
    private bool _isOwner;
    private bool disposedValue;

    public StdString()
    {
        _pointer = LibNative.std_string_new();
        _isOwner = true;
    }

    public StdString(string str)
    {
        fixed (byte* data = StringUtils.StringToManagedUtf8(str))
        {
            _pointer = LibNative.std_string_new_str(data);
            _isOwner = true;
        }
    }

    public StdString(StdString str)
    {
        if (str._pointer is null)
            throw new NullReferenceException(nameof(str._pointer));

        _pointer = LibNative.operator_new(ClassSize);
        LibNative.std_string_placement_new_copy(_pointer, str);
    }

    public StdString(move_handle<StdString> str)
    {
        if (str.Target._pointer is null)
            throw new NullReferenceException(nameof(str.Target._pointer));

        _pointer = LibNative.operator_new(ClassSize);
        LibNative.std_string_placement_new_move(_pointer, str.Target);
    }

    public StdString(nint pointer, bool isOwner = false)
    {
        _pointer = pointer.ToPointer();
        _isOwner = isOwner;
    }

    public StdString(void* pointer)
    {
        _pointer = pointer;
        _isOwner = false;
    }

    public ReadOnlySpan<byte> Data
    {
        get
        {
            var ptr = LibNative.std_string_data(_pointer);
            var len = LibNative.std_string_length(_pointer);
            if (ptr is null || len <= 0)
            {
                return ReadOnlySpan<byte>.Empty;
            }
            return new ReadOnlySpan<byte>(ptr, (int)len);
        }
    }

    public ulong Length
    {
        get
        {
            return LibNative.std_string_length(_pointer);
        }
    }

    public void Append(string str)
    {
        fixed (byte* data = StringUtils.StringToManagedUtf8(str))
        {
            LibNative.std_string_append(_pointer, data);
        }
    }

    public void Append(StdString str)
    {
        LibNative.std_string_append_std_string(_pointer, str._pointer);
    }

    public void Clear()
    {
        LibNative.std_string_clear(_pointer);
    }

    public override string ToString()
    {
        return StringUtils.Utf8ToString(Data);
    }

    public IEnumerator<byte> GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => new StdStringEnumerator();

    private struct StdStringEnumerator : IEnumerator<byte>
    {
        private ulong currentOffset;
        private readonly ulong length;
        private readonly byte* data;

        public StdStringEnumerator(StdString str)
        {
            data = LibNative.std_string_data(str);
            currentOffset = ulong.MaxValue;
            length = str.Length;
        }

        public byte Current
        {
            get
            {
                if (currentOffset >= length)
                    throw new IndexOutOfRangeException();
                return data[currentOffset];
            }
        }

        object IEnumerator.Current => Current;

        public readonly void Dispose()
        {
        }

        public bool MoveNext() => ++currentOffset < length;

        public void Reset() => currentOffset = ulong.MaxValue;
    }

    //public ref StdStringStruct GetPinnableReference()
    //{
    //    
    //    {
    //        return ref *(StdStringStruct*)_pointer;
    //    }
    //}
}
//public struct StdStringStruct { }
