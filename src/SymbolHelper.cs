using Hosihikari.FastElfQuery;
using Hosihikari.NativeInterop.Import;
using Hosihikari.NativeInterop.Unmanaged.Attributes;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Hosihikari.NativeInterop;

public static class SymbolHelper
{
    private static readonly ElfSymbolQueryTable? s_symbolTable;

    static SymbolHelper()
    {
        if (OperatingSystem.IsWindows())
        {
            return;
        }

        s_symbolTable = new("bedrock_server_symbols.debug");
    }

    /// <summary>
    ///     Try to get address of function from main module by symbol
    /// </summary>
    /// <param name="symbolName">Symbol of function</param>
    /// <param name="address">IntPtr of function</param>
    /// <returns>Is symbol available</returns>
    public static bool TryDlsym(string symbolName, out nint address)
    {
        if (OperatingSystem.IsWindows())
        {
            nint ptr = Dlsym(symbolName);
            if (ptr == nint.Zero)
            {
                address = default;
                return false;
            }

            address = ptr;
            return true;
        }

        if (!s_symbolTable!.TryQuery(symbolName, out nint offset))
        {
            address = default;
            return false;
        }

        address = HandleHelper.MainHandleHandle + offset;
        return true;
    }

    /// <summary>
    ///     Try to get address of function from main module by symbol
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
    ///     Get address of function from main module by symbol
    /// </summary>
    /// <param name="symbolName">Symbol of function</param>
    /// <returns>IntPtr of function</returns>
    public static nint Dlsym(string symbolName)
    {
        if (OperatingSystem.IsWindows())
        {
            return LibHook.ResolveSymbol(symbolName);
        }

        return s_symbolTable!.Query(symbolName) + HandleHelper.MainHandleHandle;
    }

    /// <summary>
    ///     Get address of function from main module by symbol
    /// </summary>
    /// <param name="symbolName">Symbol of function</param>
    /// <returns>IntPtr of function</returns>
    public static Lazy<nint> DlsymLazy(string symbolName)
    {
        return new(() => Dlsym(symbolName));
    }

    /// <summary>
    ///     Get address of function from main module by symbol
    /// </summary>
    /// <param name="symbolName">Symbol of function</param>
    /// <returns>Pointer of function</returns>
    public static unsafe void* DlsymPointer(string symbolName)
    {
        return Dlsym(symbolName).ToPointer();
    }

    public static bool TryQuerySymbol([NotNullWhen(true)] out string? symbol, PropertyInfo fptrProperty)
    {
        symbol = null;
        SymbolAttribute? attr = fptrProperty.GetCustomAttribute<SymbolAttribute>();
        if (attr is null)
        {
            return false;
        }

        symbol = attr.Symbol;
        return true;
    }

    public static string QuerySymbol(PropertyInfo fptrProperty)
    {
        if (!TryQuerySymbol(out string? symbol, fptrProperty))
        {
            throw new InvalidDataException();
        }

        return symbol;
    }

    public static bool TryQuerySymbol([NotNullWhen(true)] out string? symbol, MethodInfo method)
    {
        symbol = null;
        SymbolAttribute? attr = method.GetCustomAttribute<SymbolAttribute>();
        if (attr is null)
        {
            return false;
        }

        symbol = attr.Symbol;
        return true;
    }

    public static string QuerySymbol(MethodInfo method)
    {
        if (TryQuerySymbol(out string? symbol, method))
        {
            return symbol;
        }

        throw new InvalidDataException();
    }

    public static bool TryQuerySymbol([NotNullWhen(true)] out string? symbol, Delegate method)
    {
        return TryQuerySymbol(out symbol, method.Method);
    }

    public static string QuerySymbol(Delegate method)
    {
        if (!TryQuerySymbol(out string? symbol, method))
        {
            throw new InvalidDataException();
        }

        return symbol;
    }
}