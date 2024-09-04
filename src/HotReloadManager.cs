[assembly: System.Reflection.Metadata.MetadataUpdateHandler(typeof(BlazorStatic.HotReloadManager))]

namespace BlazorStatic;

internal sealed class HotReloadManager
{
    internal static bool HotReloadEnabled { get; set; }

    public static void UpdateApplication(Type[]? updatedTypes)
    {
        if (HotReloadEnabled)
            BlazorStaticExtensions.UseBlazorStaticGeneratorOnHotReload();
    }
}
