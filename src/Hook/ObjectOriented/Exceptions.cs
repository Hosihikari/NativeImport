using Hosihikari.NativeInterop.Import;

namespace Hosihikari.NativeInterop.Hook.ObjectOriented;

public sealed class HookAlreadyInstalledException() : Exception("Hook already registered");

public sealed class HookNotInstalledException() : Exception("Hook not installed");

public sealed class HookInstalledFailedException : Exception
{
    internal HookInstalledFailedException(HookResult result)
        : base($"Hook installed failed, result: {result}")
    {
        Result = result;
    }

    public HookResult Result { get; }
}

public sealed class HookUninstalledFailedException : Exception
{
    internal HookUninstalledFailedException(HookResult result)
        : base($"Hook uninstalled failed, result: {result}")
    {
        Result = result;
    }

    public HookResult Result { get; }
}