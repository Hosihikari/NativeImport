using Hosihikari.NativeInterop.Import;

namespace Hosihikari.NativeInterop.Unmanaged;

public readonly unsafe ref struct HeapAlloc<T> where T : unmanaged
{
    private static readonly ulong s_size = (ulong)sizeof(T);

    public static T* New(in T val)
    {
        T* ptr = (T*)LibNative.operator_new(s_size);
        *ptr = val;
        return ptr;
    }

    public static T* NewArray(ulong count, in T defaultVal = default)
    {
        T* ptr = (T*)LibNative.operator_new(count * s_size);
        for (ulong i = 0; i < count; ++i)
        {
            *ptr = defaultVal;
            ++ptr;
        }

        return ptr;
    }

    public static ref T NewAsRef(in T val)
    {
        return ref *New(val);
    }

    public static void Delete(T* ptr)
    {
        if (ptr is null)
        {
            return;
        }

        LibNative.operator_delete(ptr);
    }

    public static void DeleteAsRef(ref T val)
    {
        fixed (T* ptr = &val)
        {
            Delete(ptr);
        }
    }
}

public readonly unsafe ref struct HeapAlloc
{
    public static void* New(ulong size)
    {
        return LibNative.operator_new(size);
    }

    public static void Delete(void* block)
    {
        LibNative.operator_delete(block);
    }
}