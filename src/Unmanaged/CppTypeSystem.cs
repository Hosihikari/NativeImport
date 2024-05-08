using Hosihikari.NativeInterop.Unmanaged.Attributes;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.Unmanaged;

public static class CppTypeSystem
{
    public static unsafe void* GetVTable(void* ptr, int offset = 0)
    {
        return *(void**)((long)ptr + offset);
    }

    public static unsafe TVtable* GetVTable<TVtable>(void* ptr, int offset = 0)
        where TVtable : unmanaged, ICppVtable
    {
        return (TVtable*)GetVTable(ptr, offset);
    }

    public static unsafe TVtable* GetVTable<T, TVtable>(nint ptr, int offset = 0, bool cheekAttribute = true)
        where T : class, ICppInstance<T>
        where TVtable : unmanaged, ICppVtable
    {
        if (!cheekAttribute || typeof(T).GetCustomAttribute<VirtualCppClassAttribute>() is not null)
        {
            return GetVTable<TVtable>((void*)ptr, offset);
        }

        throw new NullReferenceException();
    }

    public static unsafe ValuePointer<TVtable> GetVTable<T, TVtable>(Pointer<T> ptr, int offset = 0, bool cheekAttribute = true)
        where T : class, ICppInstance<T>
        where TVtable : unmanaged, ICppVtable
    {
        return GetVTable<T, TVtable>((nint)ptr, offset, cheekAttribute);
    }

    public static unsafe ref TVtable GetVTable<T, TVtable>(T obj, int offset = 0, bool cheekAttribute = true)
        where T : class, ICppInstance<T>
        where TVtable : unmanaged, ICppVtable
    {
        return ref *GetVTable<T, TVtable>(obj.Pointer, offset, cheekAttribute);
    }

    public static T As<T>(this ICppInstanceNonGeneric @this, bool releaseSrc = false)
        where T : class, IDisposable, ICppInstance<T>
    {
        if (!releaseSrc)
        {
            return T.ConstructInstance(@this.Pointer, false, false);
        }

        T temp = T.ConstructInstance(@this.Pointer, @this.OwnsInstance, @this.OwnsMemory);
        @this.Pointer = 0;
        @this.OwnsInstance = false;
        @this.OwnsMemory = false;
        @this.Dispose();
        return temp;
    }

    public static unsafe void* GetVurtualFunctionPointerByIndex(void* vtable, int index)
        => (void*)*((long*)vtable + index);
}

public interface IOverrideAttributeGeneric
{
    public abstract int Index { get; }
    public abstract int Offset { get; }

    public abstract ulong VTableLength { get; }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class OverrideAttribute<TVtable>(int virtualMethodIndex) : Attribute, IOverrideAttributeGeneric
    where TVtable : ICppVtable
{
    public int Index => virtualMethodIndex;
    public int Offset => TVtable.Offset;
    public ulong VTableLength => TVtable.VtableLength;
}

public unsafe interface INativeVirtualMethodOverrideProvider<TSelf>
    where TSelf : class, ICppInstance<TSelf>, INativeVirtualMethodOverrideProvider<TSelf>
{
    private static readonly VtableHandle vtableHandle;

    static INativeVirtualMethodOverrideProvider()
    {
        throw new NotImplementedException();

        var query = from method in typeof(TSelf).GetRuntimeMethods()
                    let attrs = method.GetCustomAttributes(false).ToList()
                    let overrideAttr = attrs.FirstOrDefault(a => a is IOverrideAttributeGeneric) as IOverrideAttributeGeneric
                    let unmanagedCallersOnlyAttr = attrs.OfType<UnmanagedCallersOnlyAttribute>().FirstOrDefault()
                    where overrideAttr != null && unmanagedCallersOnlyAttr != null
                    group (method.MethodHandle.GetFunctionPointer(), overrideAttr.Index, overrideAttr.Offset, overrideAttr.VTableLength)
                    by (overrideAttr.Offset, overrideAttr.VTableLength) into g
                    select (g.Key, g.Select(t => (t.Item1, t.Index)).ToArray());

        vtableHandle = new(query);
    }
}

internal unsafe struct VTable
{
    public int offset;
    public void* ptr;
}

internal unsafe class VtableHandle
{
    VTable[] vtables;

    public VtableHandle(IEnumerable<((int offset, ulong vtableLength), (nint ptr, int index)[])> values)
    {
        throw new NotImplementedException();

        List<VTable> list = [];

        foreach (var ((offset, vtblLength), ptrs) in values)
        {
            var vtblPtr = NativeAlloc<nint>.NewArray(vtblLength);
            for (int i = 0; i < (int)vtblLength; ++i)
            {
                vtblPtr[i] = ptrs[i].ptr;
            }
        }
    }

    ~VtableHandle()
    {

    }
}