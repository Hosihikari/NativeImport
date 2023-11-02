using System.Collections;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Hosihikari.NativeInterop.Generation;
using Hosihikari.NativeInterop.Layer;
using Hosihikari.NativeInterop.Utils;
using static Hosihikari.NativeInterop.Generation.ITypeReferenceProvider;

namespace Hosihikari.NativeInterop.Unmanaged.STL;

[PredefinedType(
    NativeTypeName = "basic_string<char, struct std::char_traits<char>, class std::allocator<char>>",
    NativeTypeNamespace = "std")]
public unsafe partial class StdString :
    IDisposable,
    ICppInstance<StdString>,
    IMoveableCppInstance<StdString>,
    ICopyableCppInstance<StdString>,
    IEnumerable<byte>
{
    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public readonly struct StdStringFiller : INativeTypeFiller<StdStringFiller, StdString>
    {
        static StdStringFiller()
        {
            if (sizeof(StdStringFiller) != 32) throw new InvalidOperationException();
        }

        [FieldOffset(0)]
        private readonly long _alignment_member;

        public static void Destruct(StdStringFiller* @this) => DestructInstance(new(@this));

        public void Destruct()
        {
            fixed (StdStringFiller* ptr = &this)
            {
                Destruct(ptr);
            }
        }

        public static implicit operator StdString(in StdStringFiller filler)
        {
            fixed (void* ptr = &filler)
            {
                return new StdString(ptr);
            }
        }
    }

    public static ulong ClassSize => LibNative.std_string_get_class_size();

    public bool IsOwner { get => _isOwner; set => _isOwner = value; }
    public nint Pointer { get => new(_pointer); set => _pointer = value.ToPointer(); }

    public bool IsTempStackValue { get; set; }


    public static StdString ConstructInstance(nint ptr, bool owns, bool isTempStackValue) => new(ptr, owns, isTempStackValue);

    public static void DestructInstance(nint ptr) => LibNative.std_string_destructor(ptr.ToPointer());

    static object ICppInstanceNonGeneric.ConstructInstance(nint ptr, bool owns, bool isTempStackValue) => ConstructInstance(ptr, owns, isTempStackValue);

    public static StdString ConstructInstanceByCopy(StdString right) => new(right);
    public static StdString ConstructInstanceByMove(MoveHandle<StdString> right) => new(right);


    public static implicit operator nint(StdString str) => new(str._pointer);

    public static implicit operator void*(StdString str) => str._pointer;

    public void Destruct() => DestructInstance(this);

    protected virtual void Dispose(bool disposing)
    {
        if (disposedValue) return;
        if (_isOwner) Destruct();
        if (IsTempStackValue is false) LibNative.operator_delete(this);

        disposedValue = true;
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
        IsTempStackValue = false;
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
        {
            throw new NullReferenceException(nameof(str._pointer));
        }

        _pointer = LibNative.operator_new(ClassSize);
        LibNative.std_string_placement_new_copy(_pointer, str);
    }

    public StdString(MoveHandle<StdString> str)
    {
        if (str.Target._pointer is null)
        {
            throw new NullReferenceException(nameof(str.Target._pointer));
        }

        _pointer = LibNative.operator_new(ClassSize);
        LibNative.std_string_placement_new_move(_pointer, str.Target);
    }

    public StdString(nint pointer, bool isOwner = false, bool isTempStackValue = true)
    {
        _pointer = pointer.ToPointer();
        _isOwner = isOwner;
        IsTempStackValue = isTempStackValue;
    }

    public StdString(void* pointer)
    {
        _pointer = pointer;
        _isOwner = false;
        IsTempStackValue = true;
    }

    public ReadOnlySpan<byte> Data
    {
        get
        {
            byte* ptr = LibNative.std_string_data(_pointer);
            ulong len = LibNative.std_string_length(_pointer);
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

        public readonly byte Current
        {
            get
            {
                if (currentOffset >= length)
                {
                    throw new IndexOutOfRangeException();
                }
                return data[currentOffset];
            }
        }

        readonly object IEnumerator.Current => Current;

        public readonly void Dispose()
        {
        }

        public bool MoveNext() => ++currentOffset < length;

        public void Reset() => currentOffset = ulong.MaxValue;
    }
}
