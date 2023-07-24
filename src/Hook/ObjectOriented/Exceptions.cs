using Hosihikari.NativeInterop.LibLoader;

namespace Hosihikari.NativeInterop.Hook.ObjectOriented;

public class HookAlreadyInstalledException : Exception
{
    public HookAlreadyInstalledException()
        : base($"Hook already registered") { }
}

public class HookNotInstalledException : Exception
{
    public HookNotInstalledException()
        : base($"Hook not installed") { }
}

public class HookInstalledFailedException : Exception
{
    internal HookInstalledFailedException(HookResult result)
        : base($"Hook installed failed, result: {result}")
    {
        Result = result;
    }

    public HookResult Result { get; }
}
