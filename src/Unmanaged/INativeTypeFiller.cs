using System.Reflection;

namespace Hosihikari.NativeInterop.Unmanaged;

public interface INativeTypeFiller<TFiller, TManagedType>
    where TFiller : unmanaged, INativeTypeFiller<TFiller, TManagedType>
    where TManagedType : class, ICppInstance<TManagedType>
{
    public static unsafe abstract void Destruct(TFiller* @this);

    public static abstract implicit operator TManagedType(in TFiller filler);
}

internal static class NativeTypeFillerHelper
{
    public static unsafe bool TryGetDestructorFunctionPointer<TFiller>(out delegate* managed<TFiller*, void> result)
        where TFiller : unmanaged
    {
        result = null;
        Type type = typeof(TFiller);
        foreach (Type t in type.GetInterfaces())
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(INativeTypeFiller<,>))
            {
                result = (delegate* managed<TFiller*, void>)
                    type
                    .GetMethod("Destruct", BindingFlags.Public | BindingFlags.Static, new[] { typeof(TFiller*) })!
                    .MethodHandle
                    .GetFunctionPointer()
                    .ToPointer();
                return true;
            }
        }
        return false;
    }
}
