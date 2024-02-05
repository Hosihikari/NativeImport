using System.Runtime.InteropServices;

namespace Hosihikari.NativeInterop.Import;

internal static partial class LibNative
{
    private const string LibName = "layer";

    #region memory

    [LibraryImport(LibName, EntryPoint = "operator_new")]
    public static unsafe partial void* operator_new(ulong size);

    [LibraryImport(LibName, EntryPoint = "operator_delete")]
    public static unsafe partial void operator_delete(void* block);

    [LibraryImport(LibName, EntryPoint = "operator_new_array")]
    public static unsafe partial void* operator_new_array(ulong size);

    [LibraryImport(LibName, EntryPoint = "operator_delete_array")]
    public static unsafe partial void operator_delete_array(void* block);

    #endregion

    #region std::string

    [LibraryImport(LibName, EntryPoint = "std_string_get_class_size")]
    public static unsafe partial ulong std_string_get_class_size();

    [LibraryImport(LibName, EntryPoint = "std_string_placement_new_default")]
    public static unsafe partial void std_string_placement_new_default(void* where);

    [LibraryImport(LibName, EntryPoint = "std_string_placement_new_c_style_str")]
    public static unsafe partial void std_string_placement_new_c_style_str(void* where, byte* str);

    [LibraryImport(LibName, EntryPoint = "std_string_placement_new_copy")]
    public static unsafe partial void std_string_placement_new_copy(void* where, void* str);

    [LibraryImport(LibName, EntryPoint = "std_string_placement_new_move")]
    public static unsafe partial void std_string_placement_new_move(void* where, void* str);

    [LibraryImport(LibName, EntryPoint = "std_string_destructor")]
    public static unsafe partial void std_string_destructor(void* str);

    [LibraryImport(LibName, EntryPoint = "std_string_new")]
    public static unsafe partial void* std_string_new();

    [LibraryImport(LibName, EntryPoint = "std_string_new_str")]
    public static unsafe partial void* std_string_new_str(byte* s);

    [LibraryImport(LibName, EntryPoint = "std_string_delete")]
    public static unsafe partial void std_string_delete(void* str);

    [LibraryImport(LibName, EntryPoint = "std_string_length")]
    public static unsafe partial ulong std_string_length(void* str);

    [LibraryImport(LibName, EntryPoint = "std_string_append")]
    public static unsafe partial void std_string_append(void* str, byte* s);

    [LibraryImport(LibName, EntryPoint = "std_string_append_std_string")]
    public static unsafe partial void std_string_append_std_string(void* str, void* s);

    [LibraryImport(LibName, EntryPoint = "std_string_clear")]
    public static unsafe partial void std_string_clear(void* str);

    [LibraryImport(LibName, EntryPoint = "std_string_data")]
    public static unsafe partial byte* std_string_data(void* str);

    #endregion

    #region std::vector

    [LibraryImport(LibName, EntryPoint = "std_vector_new")]
    public static unsafe partial void* std_vector_new();

    [LibraryImport(LibName, EntryPoint = "std_vector_delete")]
    public static unsafe partial void std_vector_delete(void* vec);

    [LibraryImport(LibName, EntryPoint = "std_vector_push_back")]
    public static unsafe partial void std_vector_push_back(void* vec, byte* c, int length);

    [LibraryImport(LibName, EntryPoint = "std_vector_data")]
    public static unsafe partial byte* std_vector_data(void* vec);

    [LibraryImport(LibName, EntryPoint = "std_vector_size")]
    public static unsafe partial int std_vector_size(void* vec);

    [LibraryImport(LibName, EntryPoint = "std_vector_at")]
    public static unsafe partial byte* std_vector_at(void* vec, int index);

    [LibraryImport(LibName, EntryPoint = "std_vector_front")]
    public static unsafe partial byte* std_vector_front(void* vec);

    [LibraryImport(LibName, EntryPoint = "std_vector_back")]
    public static unsafe partial byte* std_vector_back(void* vec);

    [LibraryImport(LibName, EntryPoint = "std_vector_pop_back")]
    public static unsafe partial void std_vector_pop_back(void* vec);

    [LibraryImport(LibName, EntryPoint = "std_vector_clear")]
    public static unsafe partial void std_vector_clear(void* vec);

    #endregion

    #region std::istream

    [LibraryImport(LibName, EntryPoint = "std_istream_new")]
    public static unsafe partial void std_istream_new(
        byte* buffer,
        int length,
        out void* stream,
        out void* sbuf
    );

    [LibraryImport(LibName, EntryPoint = "std_istream_delete")]
    public static unsafe partial void std_istream_delete(void* stream, void* sbuf);

    [LibraryImport(LibName, EntryPoint = "std_istream_read")]
    [return: MarshalAs(UnmanagedType.U1)]
    public static unsafe partial bool std_istream_read(void* stream, byte* buffer, int length);

    #endregion
}