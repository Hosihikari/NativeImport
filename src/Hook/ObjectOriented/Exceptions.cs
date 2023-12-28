using Hosihikari.NativeInterop.Layer;

namespace Hosihikari.NativeInterop.Hook.ObjectOriented;

public class HookAlreadyInstalledException() : Exception($"Hook already registered");

public class HookNotInstalledException() : Exception($"Hook not installed");

public class HookInstalledFailedException : Exception
{
    internal HookInstalledFailedException(HookResult result)
        : base($"Hook installed failed, result: {result}")
    {
        Result = result;
    }

    public HookResult Result { get; }
}
public class HookUninstalledFailedException : Exception
{
    internal HookUninstalledFailedException(HookResult result)
        : base($"Hook uninstalled failed, result: {result}")
    {
        Result = result;
    }

    public HookResult Result { get; }
}
