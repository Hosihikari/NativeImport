using System.Diagnostics;

namespace NativeInterop;

public static class SymbolHelper
{
    static SymbolHelper()
    {
        Console.WriteLine("Loading symbol table...");
        Stopwatch sw = new();
        SymbolTable = new FastElfQuery.ElfSymbolQueryTable("bedrock_server_symbols.debug");
        sw.Stop();
        Console.WriteLine($"Symbol table loaded in {sw.ElapsedMilliseconds}ms");
    }

    public static FastElfQuery.ElfSymbolQueryTable SymbolTable { get; }

    public static bool TryDlsym(string symbolName, out nint address)
    {
        if (SymbolTable.TryQuery(symbolName, out var offset))
        {
            address = LibHook.MainHandleHandle + offset;
            return true;
        }
        address = default;
        return false;
    }

    public static unsafe bool TryDlsym(string symbolName, out void* address)
    {
        if (SymbolTable.TryQuery(symbolName, out var offset))
        {
            address = (LibHook.MainHandleHandle + offset).ToPointer();
            return true;
        }
        address = default;
        return false;
    }

    public static nint Dlsym(string symbolName)
    {
        return SymbolTable.Query(symbolName) + LibHook.MainHandleHandle;
    }

    public static unsafe void* DlsymPointer(string symbolName)
    {
        return (SymbolTable.Query(symbolName) + LibHook.MainHandleHandle).ToPointer();
    }
}
