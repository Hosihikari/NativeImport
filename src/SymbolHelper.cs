using System.Diagnostics;
using Hosihikari.FastElfQuery;

namespace Hosihikari.NativeInterop;

public static class SymbolHelper
{
    static SymbolHelper()
    {
        Console.WriteLine("Loading symbol table...");
        Stopwatch sw = Stopwatch.StartNew();
        SymbolTable = new ElfSymbolQueryTable("bedrock_server_symbols.debug");
        sw.Stop();
        Console.WriteLine($"Symbol table loaded in {sw.ElapsedMilliseconds}ms");
    }

    public static ElfSymbolQueryTable SymbolTable { get; }

    public static bool TryDlsym(string symbolName, out nint address)
    {
        if (SymbolTable.TryQuery(symbolName, out var offset))
        {
            address = HandleHelper.MainHandleHandle + offset;
            return true;
        }
        address = default;
        return false;
    }

    public static unsafe bool TryDlsym(string symbolName, out void* address)
    {
        if (SymbolTable.TryQuery(symbolName, out var offset))
        {
            address = (HandleHelper.MainHandleHandle + offset).ToPointer();
            return true;
        }
        address = default;
        return false;
    }

    public static nint Dlsym(string symbolName)
    {
        return SymbolTable.Query(symbolName) + HandleHelper.MainHandleHandle;
    }

    public static unsafe void* DlsymPointer(string symbolName)
    {
        return (SymbolTable.Query(symbolName) + HandleHelper.MainHandleHandle).ToPointer();
    }
}
