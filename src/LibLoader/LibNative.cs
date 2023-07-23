using System.Runtime.InteropServices;

namespace NativeInterop.LibLoader;

internal static partial class LibNative
{
    internal const string LibName = "libnative";

    [LibraryImport(LibName, EntryPoint = "std_string_new")]
    internal static unsafe partial void* std_string_new();

    [LibraryImport(LibName, EntryPoint = "std_string_new_str")]
    internal static unsafe partial void* std_string_new_str(byte* s);

    [LibraryImport(LibName, EntryPoint = "std_string_delete")]
    internal static unsafe partial void std_string_delete(void* str);

    [LibraryImport(LibName, EntryPoint = "std_string_length")]
    internal static unsafe partial int std_string_length(void* str);

    [LibraryImport(LibName, EntryPoint = "std_string_append")]
    internal static unsafe partial void std_string_append(void* str, byte* s);

    [LibraryImport(LibName, EntryPoint = "std_string_append_std_string")]
    internal static unsafe partial void std_string_append_std_string(void* str, void* s);

    [LibraryImport(LibName, EntryPoint = "std_string_clear")]
    internal static unsafe partial void std_string_clear(void* str);

    [LibraryImport(LibName, EntryPoint = "std_string_data")]
    internal static unsafe partial byte* std_string_data(void* str);
}
