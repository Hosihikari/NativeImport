using System.Reflection;

namespace Hosihikari.NativeInterop.Unmanaged;

public interface INativeTypeFiller<TFiller, out TManagedType>
    where TFiller : unmanaged, INativeTypeFiller<TFiller, TManagedType>
    where TManagedType : class, ICppInstance<TManagedType>
{
    public static abstract unsafe void Destruct(TFiller* @this);
    public static abstract implicit operator TManagedType(in TFiller filler);
}

internal static class NativeTypeFillerHelper
{
    public static unsafe bool TryGetDestructorFunctionPointer<TFiller>(out delegate* managed<TFiller*, void> result)
        where TFiller : unmanaged
    {
        Type type = typeof(TFiller);
        if (type.GetInterfaces()
            .Any(t => t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(INativeTypeFiller<,>))))
        {
            result = (delegate* managed<TFiller*, void>)
                type.GetMethod("Destruct", BindingFlags.Public | BindingFlags.Static, [typeof(TFiller*)])!
                    .MethodHandle
                    .GetFunctionPointer()
                    .ToPointer();
            return true;
        }

        result = null;
        return false;
    }
}