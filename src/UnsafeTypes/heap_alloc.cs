using Hosihikari.NativeInterop.LibLoader;

namespace Hosihikari.NativeInterop.UnsafeTypes;

public readonly unsafe ref struct heap_alloc<T> where T : unmanaged
{
    private static readonly ulong size = (ulong)sizeof(T);

    public readonly ref T New(in T val)
    {
        T* ptr = (T*)LibNative.operator_new(size);
        *ptr = val;
        return ref *ptr;
    }

    public readonly void Delete(ref T val) 
    {
        fixed (T* ptr = &val)
        {
            LibNative.operator_delete(ptr);
        }
    }
}
