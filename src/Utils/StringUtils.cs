﻿using System.Text;
using Hosihikari.NativeInterop.LibLoader;

namespace Hosihikari.NativeInterop.Utils;

public static class StringUtils
{
    public static unsafe string MarshalStdString(void* strPtr)
    {
        byte* dataPtr = LibNative.std_string_data(strPtr);
        if (dataPtr is null)
        {
            return string.Empty;
        }
        ulong len = LibNative.std_string_length(strPtr);
        if (len <= 0)
        {
            return string.Empty;
        }
        return Encoding.UTF8.GetString(dataPtr, (int)len);
    }

    /// <summary>
    /// Converts a string to a null-terminated UTF-8 byte array.
    /// </summary>
    /// <param name="s"> string data </param>
    /// <returns>  null-terminated UTF-8 byte array </returns>
    public static byte[] StringToManagedUtf8(string s) =>
        StringToManagedUtf8(s, out _);

    /// <summary>
    /// Converts a string to a null-terminated UTF-8 byte array.
    /// </summary>
    /// <param name="s"> string data </param>
    /// <param name="length"> byte length </param>
    /// <returns> null-terminated UTF-8 byte array </returns>
    public static unsafe byte[] StringToManagedUtf8(string? s, out int length)
    {
        if (string.IsNullOrEmpty(s))
        {
            length = 0;
            return new byte[] { 0 };
        }
        Encoding utf8 = Encoding.UTF8;
        fixed (char* ptr = s)
        {
            length = utf8.GetByteCount(ptr, s.Length);
            byte[] buffer = new byte[length + 1];
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
        if (data.Length <= 0)
        {
            return string.Empty;
        }
        return Encoding.UTF8.GetString(data);
    }
}
