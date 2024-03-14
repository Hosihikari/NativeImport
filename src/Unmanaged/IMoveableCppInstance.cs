namespace Hosihikari.NativeInterop.Unmanaged;

public interface IMoveableCppInstance<TSelf> where TSelf : class, ICppInstance<TSelf>
{
    static abstract TSelf ConstructInstanceByMove(MoveHandle<TSelf> right);
}