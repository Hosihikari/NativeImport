namespace NativeInterop.Hook.OOP;

internal interface IHook
{
    void Install();
    void Uninstall();
    bool HasInstalled { get; }
}
