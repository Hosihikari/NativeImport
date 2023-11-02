using System.Text.RegularExpressions;

namespace Hosihikari.NativeInterop.Generation;

public interface ITypeReferenceProvider
{
    public static abstract Regex Regex { get; }

    public static abstract Type? Matched(Match match);
}
