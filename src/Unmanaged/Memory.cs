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
}