using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.UnsafeTypes;

#pragma warning disable CS8981
#pragma warning disable IDE1006 
public readonly ref struct move_handle<T> where T : class, ICppInstance<T>
{
    private readonly T instance;

    public move_handle() => throw new NotSupportedException();


    [Obsolete("use std.move instead.", false)]
    public move_handle(T val)
    {
        instance = val;
    }

    public unsafe T MoveInstance()
    {
        var ptr = instance.Pointer;
        var own = instance.IsOwner;

        try
        {
            instance.Pointer = 0;
            instance.IsOwner = false;

            if (instance is IDisposable ins)
                ins.Dispose();

            return T.ConstructInstance(ptr, own);
        }
        catch
        {
            throw;
        }
        finally
        {
            if (own && ptr is not 0) T.DestructInstance(ptr);
            NativeMemory.Free((void*)ptr);
        }
    }
}
#pragma warning restore IDE1006
#pragma warning restore CS8981