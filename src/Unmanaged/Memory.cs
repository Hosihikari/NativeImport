namespace Hosihikari.NativeInterop.Unmanaged;

public unsafe static class Memory
{
    public static T* DAccess<T>(void* address, int offset)
        where T : unmanaged
        => (T*)((nint)address + offset);

    public static ref T DAccess<T>(nint address, int offset)
        where T : unmanaged
        => ref *(T*)(address + offset);

    public static Pointer<T> DAccessAsPointer<T>(nint address, int offset)
        where T : class, ICppInstance<T>
        => (Pointer<T>)(address + offset);

    public static Reference<T> DAccessAsReference<T>(nint address, int offset)
        where T : class, ICppInstance<T>
        => (Reference<T>)(address + offset);

    public static void* DAccess(void* address, int offset)
        => (void*)((nint)address + offset);
}
