namespace Hosihikari.NativeInterop.Hook.ObjectOriented;

public interface IHook
{
    void Install();
    void Uninstall();
    bool HasInstalled { get; }
    bool TryInstall();
    bool TryUninstall();
}