﻿using System.Diagnostics;

namespace NativeInterop;

public class HandleHelper
{
    private static readonly Lazy<nint> LazyHandle =
        new(() =>
        {
            var cp = Process.GetCurrentProcess();
            return cp.MainModule!.BaseAddress;
        });

    /// <summary>
    /// The handle of the main module (bedrock_server).
    /// </summary>
    public static nint MainHandleHandle => LazyHandle.Value;
}
