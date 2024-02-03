namespace Hosihikari.NativeInterop.Hook.ObjectOriented;

public interface IHook
{
    bool HasInstalled { get; }
    void Install();
    void Uninstall();
    bool TryInstall();
    bool TryUninstall();
}