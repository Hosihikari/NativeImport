using Hosihikari.NativeInterop.Generation;
using Hosihikari.NativeInterop.Utils;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Hosihikari.NativeInterop.Unmanaged.STL;

[SupportedOSPlatform("windows")]
[PredefinedType(
    NativeTypeName = "basic_string<char, struct std::char_traits<char>, class std::allocator<char>>",
    NativeTypeNamespace = "std")]
[StructLayout(LayoutKind.Sequential)]
public unsafe struct StdString : IDisposable
{
    private const int BufferSize = 16;

    [StructLayout(LayoutKind.Explicit)]
    public struct Storage
    {
        [FieldOffset(0)] public fixed byte buffer[BufferSize];
        [FieldOffset(0)] public byte* ptr;
    }

    //0
    public Storage storage;

    //16
    public ulong size;

    //24
    public ulong res;

    public void Clear()
    {
        if (res > BufferSize)
        {
            HeapAlloc.Delete(storage.ptr);
        }

        size = res = 0;
        storage.ptr = null;
    }

    public override string ToString()
    {
        fixed (byte* buf = storage.buffer)
        {
            return new(res > BufferSize ? (sbyte*)storage.ptr : (sbyte*)buf, 0, (int)size);
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
            {
                Unsafe.CopyBlock(p, ptr, (uint)size * sizeof(byte));
            }

            @this.res = BufferSize;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly ulong CalculateGrowth(ulong newSize)
    {
        ulong num = res;
        const ulong num2 = int.MaxValue / sizeof(byte);
        ulong num3 = num >> 1;
        if (num > (num2 - num3))
        {
            return num2;
        }

        ulong num4 = num3 + num;
        return num4 < newSize ? newSize : num4;
    }

    private void EmplaceReallocate(ulong index, byte* ptr, ulong size)
    {
        fixed (byte* buf = storage.buffer)
        {
            ulong oldCap = res;
            ulong newSize = size + size;
            ulong newCap = CalculateGrowth(newSize);
            byte* temp = stackalloc byte[BufferSize];
            byte* newStr = newCap > BufferSize ? (byte*)HeapAlloc.New(newCap) : temp;
            byte* oldStr = oldCap > BufferSize ? storage.ptr : buf;
            byte* constructed = newStr;
            Unsafe.CopyBlock(constructed, oldStr, (uint)index * sizeof(byte));
            constructed += index;
            Unsafe.CopyBlock(constructed, ptr, (uint)size * sizeof(byte));
            constructed += size;
            Unsafe.CopyBlock(constructed, oldStr + index, (uint)(size - index) * sizeof(byte));
            if (newCap > BufferSize)
            {
                storage.ptr = newStr;
                this.size = newSize;
                res = newCap;
            }
            else
            {
                Unsafe.CopyBlock(buf, newStr, (uint)newSize * sizeof(byte));
                this.size = newSize;
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
            byte* str = res > BufferSize ? storage.ptr : buf;
            byte* temp = stackalloc byte[BufferSize];
            byte* buffer = size > BufferSize ? (byte*)HeapAlloc.New(size) : temp;
            Unsafe.CopyBlock(buffer, str + index, (uint)(size - index) * sizeof(byte));
            Unsafe.CopyBlock(str + index + size, buffer, (uint)size * sizeof(byte));
            Unsafe.CopyBlock(str + index, ptr, (uint)size * sizeof(byte));
            if (size > BufferSize)
            {
                HeapAlloc.Delete(buffer);
            }

            this.size += size;
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
        byte[] bytes = StringUtils.StringToManagedUtf8(str);
        res = size = (ulong)bytes.Length;
        fixed (byte* ptr = bytes)
        {
            AllocateByPtr(ref this, ptr, size);
        }
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
        {
            AllocateByPtr(ref this, ptr, str.size);
        }
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
        fixed (byte* buffer = str.storage.buffer)
        {
            if (str.size > (res - size))
            {
                EmplaceReallocate(index, str.size > BufferSize ? str.storage.ptr : buffer, str.size);
            }
            else
            {
                EmplaceWithUnusedCapacity(index, str.size > BufferSize ? str.storage.ptr : buffer, str.size);
            }
        }
    }

    public void EmplaceBack(in StdString str)
    {
        Emplace(Size - 1, str);
    }

    public Span<byte> Data
    {
        get
        {
            if (size > BufferSize)
            {
                return new(storage.ptr, (int)size);
            }

            fixed (byte* ptr = storage.buffer)
            {
                return new(ptr, (int)size);
            }
        }
    }

    public readonly ulong Length => size;

    public void Append(in StdString str)
    {
        EmplaceBack(str);
    }

    public void Append(string str)
    {
        EmplaceBack(new(str));
    }

    public void Dispose()
    {
        Clear();
    }
}