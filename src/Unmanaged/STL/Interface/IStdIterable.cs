namespace Hosihikari.NativeInterop.Unmanaged.STL.Interface;

internal interface IStdIterable<TIterator, T>
    where TIterator : unmanaged, IStdIterator<TIterator, T>
    where T : unmanaged
{
    public TIterator Begin();
    public TIterator End();

    public ReverseIterator<TIterator, T> RBegin();

    public ReverseIterator<TIterator, T> REnd();
}
