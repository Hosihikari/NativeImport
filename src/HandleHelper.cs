using System.Diagnostics;

namespace Hosihikari.NativeInterop;

public static class HandleHelper
{
    static HandleHelper() =>
        LazyHandle =
            new(() =>
            {
                Process cp = Process.GetCurrentProcess();
                return cp.MainModule!.BaseAddress;
            });

    private static readonly Lazy<nint> LazyHandle;

    /// <summary>
    /// The handle of the main module (bedrock_server).
    /// </summary>
    public static nint MainHandleHandle => LazyHandle.Value;
}
