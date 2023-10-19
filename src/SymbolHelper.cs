using Hosihikari.FastElfQuery;

namespace Hosihikari.NativeInterop;

public static class SymbolHelper
{
    static SymbolHelper() =>
        SymbolTable = new ElfSymbolQueryTable("bedrock_server_symbols.debug");

    private static ElfSymbolQueryTable SymbolTable { get; }

    /// <summary>
    /// Try to get address of function from main module by symbol
    /// </summary>
    /// <param name="symbolName">Symbol of function</param>
    /// <param name="address">IntPtr of function</param>
    /// <returns>Is symbol available</returns>
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

    /// <summary>
    /// Try to get address of function from main module by symbol
    /// </summary>
    /// <param name="symbolName">Symbol of function</param>
    /// <param name="address">Pointer of function</param>
    /// <returns>Is symbol available</returns>
    public static unsafe bool TryDlsym(string symbolName, out void* address)
    {
        bool result = TryDlsym(symbolName, out nint addressPtr);
        address = addressPtr.ToPointer();
        return result;
    }

    /// <summary>
    /// Get address of function from main module by symbol
    /// </summary>
    /// <param name="symbolName">Symbol of function</param>
    /// <returns>IntPtr of function</returns>
    public static nint Dlsym(string symbolName) =>
        SymbolTable.Query(symbolName) + HandleHelper.MainHandleHandle;

    /// <summary>
    /// Get address of function from main module by symbol
    /// </summary>
    /// <param name="symbolName">Symbol of function</param>
    /// <returns>IntPtr of function</returns>
    public static Lazy<nint> DlsymLazy(string symbolName) =>
        new(() => Dlsym(symbolName));

    /// <summary>
    /// Get address of function from main module by symbol
    /// </summary>
    /// <param name="symbolName">Symbol of function</param>
    /// <returns>Pointer of function</returns>
    public static unsafe void* DlsymPointer(string symbolName) =>
        Dlsym(symbolName).ToPointer();
}
