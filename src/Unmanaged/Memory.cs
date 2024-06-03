namespace Hosihikari.NativeInterop.Unmanaged;

public static unsafe class Memory
{
    public static T* DAccess<T>(void* address, int offset)
        where T : unmanaged
    {
        return (T*)((nint)address + offset);
    }

    public static ref T DAccess<T>(nint address, int offset)
        where T : unmanaged
    {
        return ref *(T*)(address + offset);
    }

    public static Pointer<T> DAccessAsPointer<T>(nint address, int offset)
        where T : class, ICppInstance<T>
    {
        return (Pointer<T>)(address + offset);
    }

    public static Reference<T> DAccessAsReference<T>(nint address, int offset)
        where T : class, ICppInstance<T>
    {
        return (Reference<T>)(address + offset);
    }

    public static void* DAccess(void* address, int offset)
    {
        return (void*)((nint)address + offset);
    }

    public static int Memcmp(void* buf1, void* buf2, ulong count)
    {
        if (count is 0) return 0;

        while (--count is not 0 && *(byte*)buf1 == *(byte*)buf2)
        {
            buf1 = (char*)buf1 + 1;
            buf2 = (char*)buf2 + 1;
        }

        return *(byte*)buf1 - *(byte*)buf2;
    }
}