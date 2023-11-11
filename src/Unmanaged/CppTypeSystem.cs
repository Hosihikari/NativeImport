using Hosihikari.NativeInterop.Unmanaged.Attributes;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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

[AttributeUsage(AttributeTargets.Method)]
public class OverrideAttribute : Attribute
{
    public int VirtualMethodIndex { get; private set; }

    public OverrideAttribute(int virtualMethodIndex)
    {
        VirtualMethodIndex = virtualMethodIndex;
    }
}

public unsafe interface INativeVirtualMethodOverrideProvider<T, TVtable>
    where T : class, ICppInstance<T>
    where TVtable : unmanaged, ICppVtable
{
    public class VTableHandle : SafeHandle
    {
        public VTableHandle() : base(0, true)
        {
            TVtable* ptr = null;
            try
            {
                ptr = HeapAlloc<TVtable>.New(default);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (ptr is not null)
                {
                    HeapAlloc<TVtable>.Delete(ptr);
                }
            }
        }

        public override bool IsInvalid => false;

        protected override bool ReleaseHandle()
        {
            if (handle is 0) return true;
            HeapAlloc<TVtable>.Delete((TVtable*)handle);
            handle = 0;
            return true;
        }

        public TVtable* VTable => (TVtable*)handle;
    }

    private static readonly bool isVirtualCppClass;
    private static readonly (int, nint)[]? virtFptrs;

    static INativeVirtualMethodOverrideProvider()
    {
        isVirtualCppClass = typeof(T).GetCustomAttribute<VirtualCppClassAttribute>() is not null;
        if (isVirtualCppClass) return;

        List<(int, nint)> list = new();

        foreach (var method in typeof(T).GetMethods(BindingFlags.Public | BindingFlags.NonPublic))
        {
            var overrideAttr = method.GetCustomAttribute<OverrideAttribute>();
            if (overrideAttr is null) continue;

            _ = method.GetCustomAttribute<UnmanagedCallersOnlyAttribute>() ?? throw new InvalidProgramException();
            list.Add((overrideAttr.VirtualMethodIndex, method.MethodHandle.GetFunctionPointer()));
        }

        virtFptrs = list.ToArray();
    }

    static VTableHandle? SetupVtable(ICppInstanceNonGeneric ins)
    {
        if (isVirtualCppClass is false || virtFptrs is null) return null;

        var handle = new VTableHandle();
        Unsafe.CopyBlock(handle.VTable, CppTypeSystem.GetVTable((void*)ins.Pointer), (uint)sizeof(TVtable));

        foreach (var (index, fptr) in virtFptrs)
        {
            *(void**)(((long)handle.VTable) + index * sizeof(void*)) = (void*)fptr;
        }

        *(void**)(void*)ins.Pointer = handle.VTable;
        return handle;
    }
}