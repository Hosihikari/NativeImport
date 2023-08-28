using Hosihikari.FastElfQuery;

namespace Hosihikari.NativeInterop;

public static class SymbolHelper
{
    static SymbolHelper()
    {
        SymbolTable = new ElfSymbolQueryTable("bedrock_server_symbols.debug");
    }

    public static ElfSymbolQueryTable SymbolTable { get; }

    public static bool TryDlsym(string symbolName, out nint address)
    {
        if (SymbolTable.TryQuery(symbolName, out int offset))
        {
            address = HandleHelper.MainHandleHandle + offset;
            return true;
        }
        address = default;
        return false;
    }

    public static unsafe bool TryDlsym(string symbolName, out void* address)
    {
        bool result = TryDlsym(symbolName, out nint addressPtr);
        address = addressPtr.ToPointer();
        return result;
    }

    public static nint Dlsym(string symbolName) =>
        SymbolTable.Query(symbolName) + HandleHelper.MainHandleHandle;

    public static Lazy<nint> DlsymLazy(string symbolName) =>
        new(() => Dlsym(symbolName));

    public static unsafe void* DlsymPointer(string symbolName) =>
        Dlsym(symbolName).ToPointer();
}
