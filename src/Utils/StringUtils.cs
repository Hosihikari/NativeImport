using System.Text;

namespace NativeInterop.Utils;

public static class StringUtils
{
    public static byte[] StringToManagedUtf8(string s)
    {
        return StringToManagedUtf8(s, out _);
    }

    public static unsafe byte[] StringToManagedUtf8(string? s, out int length)
    {
        if (string.IsNullOrEmpty(s))
        {
            length = 0;
            return new byte[] { 0 };
        }
        var utf8 = Encoding.UTF8;
        fixed (char* ptr = s)
        {
            length = utf8.GetByteCount(ptr, s.Length);
            var buffer = new byte[length + 1];
            fixed (byte* buf = buffer)
            {
                utf8.GetBytes(ptr, s.Length, buf, length);
                buf[length] = 0; // null-terminated
            }
            return buffer;
        }
    }

    public static string Utf8ToString(ReadOnlySpan<byte> data)
    {
        return Encoding.UTF8.GetString(data);
    }
}
