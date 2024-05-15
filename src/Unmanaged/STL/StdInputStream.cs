using Hosihikari.NativeInterop.Import;
using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.Unmanaged.STL;

/// <summary>
///     std::istream wrapper
/// </summary>
public sealed class StdInputStream
{
    private readonly unsafe byte* _buffer;

    //flag to determine if the pointer is created by this lib and should be deleted in the destructor
    private readonly bool _isOwner;
    private readonly unsafe void* _nativeBuffer;
    private readonly unsafe void* _pointer;

    public StdInputStream(ReadOnlySpan<byte> data)
    {
        unsafe
        {
            _buffer = (byte*)NativeMemory.Alloc((nuint)data.Length);
            data.CopyTo(new(_buffer, data.Length));
            LibNative.std_istream_new(
                _buffer,
                data.Length,
                out _pointer,
                out _nativeBuffer
            );
            _isOwner = true;
        }
    }

    public unsafe StdInputStream(void* ptr)
    {
        _pointer = ptr;
        _isOwner = false;
    }

    public unsafe void* Pointer => _pointer;

    ~StdInputStream()
    {
        unsafe
        {
            if (!_isOwner)
            {
                throw new NullReferenceException();
            }

            if (_buffer is not null)
            {
                NativeMemory.Free(_buffer);
            }

            LibNative.std_istream_delete(_pointer, _nativeBuffer);
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
                writer.Write([..new ReadOnlySpan<byte>(buffer, bufferSize)]);
                NativeMemory.Clear(buffer, bufferSize);
            }

            byte[] data = [..new ReadOnlySpan<byte>(buffer, bufferSize)];
            int end = data.AsSpan().IndexOf((byte)0);
            if (end > 0)
            {
                data = [..data.AsSpan(0, end)];
            }

            writer.Write(data);
            writer.Flush();
            return ms.ToArray();
        }
    }

    public static unsafe implicit operator void*(StdInputStream stream)
    {
        return stream.Pointer;
    }
}