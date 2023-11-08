using System.Runtime.InteropServices;
using size_t = System.UInt64;

namespace Hosihikari.NativeInterop.Layer;

internal static partial class LibNative
{
#if LINUX
    internal const string LibName = "liblayer";
#else
    internal const string LibName = "Hosihikari.Preload";
#endif

    #region memory
    [LibraryImport(LibName, EntryPoint = "operator_new")]
    internal static unsafe partial void* operator_new(size_t size);

    [LibraryImport(LibName, EntryPoint = "operator_delete")]
    internal static unsafe partial void operator_delete(void* block);

    [LibraryImport(LibName, EntryPoint = "operator_new_array")]
    internal static unsafe partial void* operator_new_array(size_t size);

    [LibraryImport(LibName, EntryPoint = "operator_delete_array")]
    internal static unsafe partial void operator_delete_array(void* block);
    #endregion

    #region std::string

    [LibraryImport(LibName, EntryPoint = "std_string_get_class_size")]
    internal static unsafe partial size_t std_string_get_class_size();

    [LibraryImport(LibName, EntryPoint = "std_string_placement_new_default")]
    internal static unsafe partial void std_string_placement_new_default(void* where);

    [LibraryImport(LibName, EntryPoint = "std_string_placement_new_c_style_str")]
    internal static unsafe partial void std_string_placement_new_c_style_str(void* where, byte* str);

    [LibraryImport(LibName, EntryPoint = "std_string_placement_new_copy")]
    internal static unsafe partial void std_string_placement_new_copy(void* where, void* str);

    [LibraryImport(LibName, EntryPoint = "std_string_placement_new_move")]
    internal static unsafe partial void std_string_placement_new_move(void* where, void* str);

    [LibraryImport(LibName, EntryPoint = "std_string_destructor")]
    internal static unsafe partial void std_string_destructor(void* str);

    [LibraryImport(LibName, EntryPoint = "std_string_new")]
    internal static unsafe partial void* std_string_new();

    [LibraryImport(LibName, EntryPoint = "std_string_new_str")]
    internal static unsafe partial void* std_string_new_str(byte* s);

    [LibraryImport(LibName, EntryPoint = "std_string_delete")]
    internal static unsafe partial void std_string_delete(void* str);

    [LibraryImport(LibName, EntryPoint = "std_string_length")]
    internal static unsafe partial size_t std_string_length(void* str);

    [LibraryImport(LibName, EntryPoint = "std_string_append")]
    internal static unsafe partial void std_string_append(void* str, byte* s);

    [LibraryImport(LibName, EntryPoint = "std_string_append_std_string")]
    internal static unsafe partial void std_string_append_std_string(void* str, void* s);

    [LibraryImport(LibName, EntryPoint = "std_string_clear")]
    internal static unsafe partial void std_string_clear(void* str);

    [LibraryImport(LibName, EntryPoint = "std_string_data")]
    internal static unsafe partial byte* std_string_data(void* str);

    #endregion

    #region std::vector

    [LibraryImport(LibName, EntryPoint = "std_vector_new")]
    internal static unsafe partial void* std_vector_new();

    [LibraryImport(LibName, EntryPoint = "std_vector_delete")]
    internal static unsafe partial void std_vector_delete(void* vec);

    [LibraryImport(LibName, EntryPoint = "std_vector_push_back")]
    internal static unsafe partial void std_vector_push_back(void* vec, byte* c, int length);

    [LibraryImport(LibName, EntryPoint = "std_vector_data")]
    internal static unsafe partial byte* std_vector_data(void* vec);

    [LibraryImport(LibName, EntryPoint = "std_vector_size")]
    internal static unsafe partial int std_vector_size(void* vec);

    [LibraryImport(LibName, EntryPoint = "std_vector_at")]
    internal static unsafe partial byte* std_vector_at(void* vec, int index);

    [LibraryImport(LibName, EntryPoint = "std_vector_front")]
    internal static unsafe partial byte* std_vector_front(void* vec);

    [LibraryImport(LibName, EntryPoint = "std_vector_back")]
    internal static unsafe partial byte* std_vector_back(void* vec);

    [LibraryImport(LibName, EntryPoint = "std_vector_pop_back")]
    internal static unsafe partial void std_vector_pop_back(void* vec);

    [LibraryImport(LibName, EntryPoint = "std_vector_clear")]
    internal static unsafe partial void std_vector_clear(void* vec);

    #endregion

    #region std::istream

    [LibraryImport(LibName, EntryPoint = "std_istream_new")]
    internal static unsafe partial void std_istream_new(
        byte* buffer,
        int length,
        out void* stream,
        out void* sbuf
    );

    [LibraryImport(LibName, EntryPoint = "std_istream_delete")]
    internal static unsafe partial void std_istream_delete(void* stream, void* sbuf);

    [LibraryImport(LibName, EntryPoint = "std_istream_read")]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static unsafe partial bool std_istream_read(void* stream, byte* buffer, int length);

    #endregion
}
