using System.Diagnostics;

namespace Hosihikari.NativeInterop;

public static class HandleHelper
{
    private static readonly Lazy<nint> s_lazyHandle;

    static HandleHelper()
    {
        s_lazyHandle =
            new(() =>
            {
                Process cp = Process.GetCurrentProcess();
                return cp.MainModule!.BaseAddress;
            });
    }

    /// <summary>
    ///     The handle of the main module (bedrock_server).
    /// </summary>
    public static nint MainHandleHandle => s_lazyHandle.Value;
}