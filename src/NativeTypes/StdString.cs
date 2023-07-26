using Hosihikari.NativeInterop.Utils;

namespace Hosihikari.NativeInterop.NativeTypes;

public class StdString
{
    private readonly unsafe void* _pointer;

    //flag to determine if the pointer is created by this lib and should be deleted in the destructor
    private readonly bool _isOwner;

    public StdString()
    {
        unsafe
        {
            _pointer = LibLoader.LibNative.std_string_new();
            _isOwner = true;
        }
    }

    public unsafe StdString(string str)
    {
        fixed (byte* data = StringUtils.StringToManagedUtf8(str))
        {
            _pointer = LibLoader.LibNative.std_string_new_str(data);
            _isOwner = true;
        }
    }

    public unsafe StdString(nint pointer)
    {
        _pointer = pointer.ToPointer();
        _isOwner = false;
    }

    public unsafe StdString(void* pointer)
    {
        _pointer = pointer;
        _isOwner = false;
    }

    ~StdString()
    {
        unsafe
        {
            if (_isOwner)
            {
                LibLoader.LibNative.std_string_delete(_pointer);
            }
        }
    }

    public unsafe void* Pointer => _pointer;
    public ReadOnlySpan<byte> Data
    {
        get
        {
            unsafe
            {
                var ptr = LibLoader.LibNative.std_string_data(_pointer);
                var len = LibLoader.LibNative.std_string_length(_pointer);
                if (ptr is null || len <= 0)
                {
                    return ReadOnlySpan<byte>.Empty;
                }
                return new ReadOnlySpan<byte>(ptr, len);
            }
        }
    }

    public int Length
    {
        get
        {
            unsafe
            {
                return LibLoader.LibNative.std_string_length(_pointer);
            }
        }
    }

    public void Append(string str)
    {
        unsafe
        {
            fixed (byte* data = StringUtils.StringToManagedUtf8(str))
            {
                LibLoader.LibNative.std_string_append(_pointer, data);
            }
        }
    }

    public void Append(StdString str)
    {
        unsafe
        {
            LibLoader.LibNative.std_string_append_std_string(_pointer, str._pointer);
        }
    }

    public void Clear()
    {
        unsafe
        {
            LibLoader.LibNative.std_string_clear(_pointer);
        }
    }

    public override string ToString()
    {
        return StringUtils.Utf8ToString(Data);
    }

    public static implicit operator nint(StdString str)
    {
        unsafe
        {
            return (nint)str._pointer;
        }
    }

    public static unsafe implicit operator void*(StdString str)
    {
        return str._pointer;
    }

    //public ref StdStringStruct GetPinnableReference()
    //{
    //    unsafe
    //    {
    //        return ref *(StdStringStruct*)_pointer;
    //    }
    //}
}
//public struct StdStringStruct { }
