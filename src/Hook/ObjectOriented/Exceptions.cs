using Hosihikari.NativeInterop.Import;

namespace Hosihikari.NativeInterop.Hook.ObjectOriented;

public abstract class HookException(string? message) : InvalidOperationException(message);

public sealed class HookAlreadyInstalledException() : HookException("Hook already registered");

public sealed class HookNotInstalledException() : HookException("Hook not installed");

public sealed class HookInstalledFailedException : HookException
{
    internal HookInstalledFailedException(HookResult result)
        : base($"Hook installed failed, result: {result}")
    {
        Result = result;
    }

    public HookResult Result { get; }
}

public sealed class HookUninstalledFailedException : HookException
{
    internal HookUninstalledFailedException(HookResult result)
        : base($"Hook uninstalled failed, result: {result}")
    {
        Result = result;
    }

    public HookResult Result { get; }
}