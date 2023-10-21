using Hosihikari.NativeInterop.Unmanaged.Attributes;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Hosihikari.NativeInterop.Unmanaged;

public static class CppTypeSystem
{
    public static unsafe void* GetVTable(void* ptr) => (void*)Unsafe.Read<long>(ptr);

    public static unsafe TVtable* GetVTable<TVtable>(void* ptr)
        where TVtable : unmanaged, ICppVtable
        => (TVtable*)GetVTable(ptr);

    public static unsafe TVtable* GetVTable<T, TVtable>(nint ptr, bool cheekAttribute = true)
        where T : class, ICppInstance<T>
        where TVtable : unmanaged, ICppVtable
    {
        if (cheekAttribute && typeof(T).GetCustomAttribute<VirtualCppClassAttribute>() is not null)
            return GetVTable<TVtable>((void*)ptr);

        throw new InvalidOperationException("\'VirtualCppClassAttribute\' instance is null.");
    }

    public static unsafe ValuePointer<TVtable> GetVTable<T, TVtable>(Pointer<T> ptr, bool cheekAttribute = true)
        where T : class, ICppInstance<T>
        where TVtable : unmanaged, ICppVtable
        => GetVTable<T, TVtable>(ptr, cheekAttribute);

    public static unsafe ref TVtable GetVTable<T, TVtable>(T obj, bool cheekAttribute = true)
        where T : class, ICppInstance<T>
        where TVtable : unmanaged, ICppVtable
        => ref *GetVTable<T, TVtable>(obj.Pointer, cheekAttribute);
}

public static class CppTypeSystemUtils
{
    public static T As<T>(this ICppInstanceNonGeneric @this, bool releaseSrc = false)
        where T : class, IDisposable, ICppInstance<T>
    {
        if (releaseSrc)
        {
            T temp = T.ConstructInstance(@this.Pointer, @this.IsOwner, @this.IsTempStackValue);
            @this.Pointer = 0;
            @this.IsOwner = false;
            @this.IsTempStackValue = false;

            @this.Dispose();

            return temp;
        }
        return T.ConstructInstance(@this.Pointer, false, true);
    }
}
