using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.Unmanaged.STL;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct StdErrorCondition
{
    public int val;
    public StdErrorCategory* category;
}

[StructLayout(LayoutKind.Explicit, Size = 16)]
public unsafe struct StdErrorCategory
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct VTable : ICppVtable
    {
        // 0
        public readonly delegate* unmanaged<StdErrorCategory*, void> destructor;

        // 1
        public readonly delegate* unmanaged<StdErrorCategory*, byte*> name;

        // 2
        public readonly delegate* unmanaged<StdErrorCategory*, int, StdString> message;

        // 3
        public readonly delegate* unmanaged<StdErrorCategory*, int, StdErrorCondition> default_error_condition;

        // 4
        public readonly delegate* unmanaged<StdErrorCategory*, int, in StdErrorCondition, bool> equivalent;

        // 5
        public readonly delegate* unmanaged<StdErrorCategory*, in StdErrorCode, int, bool> equivalent_overload;
        public static ulong VtableLength => 6;
    }
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct StdErrorCode
{
    public int val;
    public void* errorCategory;
}