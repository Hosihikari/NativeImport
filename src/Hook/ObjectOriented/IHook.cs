namespace Hosihikari.NativeInterop.Hook.ObjectOriented;

internal interface IHook
{
    void Install();
    void Uninstall();
    bool HasInstalled { get; }
}
