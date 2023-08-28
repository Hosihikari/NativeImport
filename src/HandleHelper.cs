using System.Diagnostics;

namespace Hosihikari.NativeInterop;

public class HandleHelper
{
    private static readonly Lazy<nint> LazyHandle =
        new(() =>
        {
            Process cp = Process.GetCurrentProcess();
            return cp.MainModule!.BaseAddress;
        });

    /// <summary>
    /// The handle of the main module (bedrock_server).
    /// </summary>
    public static nint MainHandleHandle => LazyHandle.Value;
}
