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

    public T Target => instance;
}
#pragma warning restore IDE1006
#pragma warning restore CS8981