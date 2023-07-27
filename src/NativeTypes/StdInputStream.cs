using System.Runtime.InteropServices;
using System.Text;
using Hosihikari.NativeInterop.LibLoader;

namespace Hosihikari.NativeInterop.NativeTypes;

public class StdInputStream
{
    private readonly unsafe void* _pointer;

    //flag to determine if the pointer is created by this lib and should be deleted in the destructor
    private readonly bool _isOwner;

    public StdInputStream(ReadOnlySpan<byte> data)
    {
        unsafe
        {
            fixed (byte* dataPtr = data)
            {
                _pointer = LibLoader.LibNative.std_istream_new(dataPtr, data.Length);
                _isOwner = true;
            }
        }
    }

    public unsafe StdInputStream(void* ptr)
    {
        _pointer = ptr;
        _isOwner = false;
    }

    public byte[] ReadToEnd()
    {
        MemoryStream ms = new();
        StreamWriter writer = new(ms);
        unsafe
        {
            const int bufferSize = 128;
            var buffer = stackalloc byte[bufferSize];
            while (LibNative.std_istream_read(_pointer, buffer, bufferSize))
            {
                writer.Write(new ReadOnlySpan<byte>(buffer, bufferSize).ToArray());
                NativeMemory.Clear(buffer, bufferSize);
            }

            var data = new ReadOnlySpan<byte>(buffer, bufferSize).ToArray();
            //seek first \0 and trim end
            var end = data.AsSpan().IndexOf((byte)0);
            if (end != -1)
            {
                data = data.AsSpan(0, end).ToArray();
            }

            writer.Write(data);
            writer.Flush();
            return ms.ToArray();
        }
    }

    ~StdInputStream()
    {
        unsafe
        {
            if (_isOwner)
            {
                LibLoader.LibNative.std_istream_delete(_pointer);
            }
        }
    }
}