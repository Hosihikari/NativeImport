using System.Runtime.InteropServices;
using Hosihikari.NativeInterop.LibLoader;

namespace Hosihikari.NativeInterop.Unmanaged.STL;

/// <summary>
/// std::istream wrapper
/// </summary>
public class StdInputStream
{
    private readonly unsafe void* _pointer;
    private readonly unsafe byte* _buffer;
    private readonly unsafe void* _nativebuffer;

    //flag to determine if the pointer is created by this lib and should be deleted in the destructor
    private readonly bool _isOwner;
    public unsafe void* Pointer => _pointer;

    public StdInputStream(ReadOnlySpan<byte> data)
    {
        unsafe
        {
            _buffer = (byte*)NativeMemory.Alloc((nuint)data.Length);
            data.CopyTo(new Span<byte>(_buffer, data.Length));
            LibNative.std_istream_new(
                _buffer,
                data.Length,
                out _pointer,
                out _nativebuffer
            );
            _isOwner = true;
        }
    }

    public unsafe StdInputStream(void* ptr)
    {
        _pointer = ptr;
        _isOwner = false;
    }

    ~StdInputStream()
    {
        unsafe
        {
            if (_isOwner)
            {
                if (_buffer is not null)
                {
                    NativeMemory.Free(_buffer);
                }
                LibNative.std_istream_delete(_pointer, _nativebuffer);
            }
        }
    }

    public byte[] ReadToEnd()
    {
        MemoryStream ms = new();
        BinaryWriter writer = new(ms);
        unsafe
        {
            const int bufferSize = 128;
            byte* buffer = stackalloc byte[bufferSize];
            while (LibNative.std_istream_read(_pointer, buffer, bufferSize))
            {
                writer.Write(new ReadOnlySpan<byte>(buffer, bufferSize).ToArray());
                NativeMemory.Clear(buffer, bufferSize);
            }

            byte[] data = new ReadOnlySpan<byte>(buffer, bufferSize).ToArray();
            int end = data.AsSpan().IndexOf((byte)0);
            if (end != -1)
            {
                data = data.AsSpan(0, end).ToArray();
            }
            writer.Write(data);
            writer.Flush();
            return ms.ToArray();
        }
    }

    public static unsafe implicit operator void*(StdInputStream stream) => stream.Pointer;
}
