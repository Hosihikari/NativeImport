namespace Hosihikari.NativeInterop.Unmanaged;

public interface ICppVtable
{
    public static abstract ulong VtableLength { get; }
    public static abstract int Offset { get; }
}