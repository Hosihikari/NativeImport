using Hosihikari.FastElfQuery;
using Hosihikari.NativeInterop.Unmanaged.Attributes;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop;

public static partial class SymbolHelper
{

#if WINDOWS
    private static ElfSymbolQueryTable SymbolTable => throw new NotSupportedException();
#else
    static SymbolHelper() =>
        SymbolTable = new ElfSymbolQueryTable("bedrock_server_symbols.debug");

    private static ElfSymbolQueryTable SymbolTable { get; }
#endif
    /// <summary>
    /// Try to get address of function from main module by symbol
    /// </summary>
    /// <param name="symbolName">Symbol of function</param>
    /// <param name="address">IntPtr of function</param>
    /// <returns>Is symbol available</returns>
    public static bool TryDlsym(string symbolName, out nint address)
    {
#if WINDOWS
        nint ptr = Dlsym(symbolName);
        if (ptr is not 0)
        {
            address = ptr;
            return true;
        }

        address = default;
        return false;
#else
        if (SymbolTable.TryQuery(symbolName, out int offset))
        {
            address = HandleHelper.MainHandleHandle + offset;
            return true;
        }
        address = default;
        return false;
#endif
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
#if WINDOWS
    internal const string LibName = "Hosihikari.Preload";
    [LibraryImport(LibName, EntryPoint = "dlsym")]
    internal static unsafe partial nint Dlsym([MarshalAs(UnmanagedType.LPWStr)] string symbolName);
#else
    public static nint Dlsym(string symbolName) =>
        SymbolTable.Query(symbolName) + HandleHelper.MainHandleHandle;
#endif

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


    public static bool TryQuerySymbol([NotNullWhen(true)] out string? symbol, PropertyInfo fptrProperty)
    {
        symbol = null;
        SymbolAttribute? attr = fptrProperty.GetCustomAttribute<SymbolAttribute>();
        if (attr is null) return false;

        symbol = attr.Symbol;
        return true;
    }

    public static string QuerySymbol(PropertyInfo fptrProperty)
    {
        if (TryQuerySymbol(out string? symbol, fptrProperty))
        {
            return symbol;
        }

        throw new InvalidOperationException();
    }

    public static bool TryQuerySymbol([NotNullWhen(true)] out string? symbol, MethodInfo method)
    {
        symbol = null;
        SymbolAttribute? attr = method.GetCustomAttribute<SymbolAttribute>();
        if (attr is null) return false;

        symbol = attr.Symbol;
        return true;
    }

    public static string QuerySymbol(MethodInfo method)
    {
        if (TryQuerySymbol(out string? symbol, method))
        {
            return symbol;
        }

        throw new InvalidOperationException();
    }

    public static bool TryQuerySymbol([NotNullWhen(true)] out string? symbol, Delegate method)
        => TryQuerySymbol(out symbol, method.Method);

    public static string QuerySymbol(Delegate method)
    {
        if (TryQuerySymbol(out string? symbol, method))
        {
            return symbol;
        }
        throw new InvalidOperationException();
    }
}
