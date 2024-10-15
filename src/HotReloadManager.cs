using BlazorStatic;
using System.Reflection.Metadata;

[assembly: MetadataUpdateHandler(typeof(HotReloadManager))]

namespace BlazorStatic;

/// <summary>
///     Used for subscribing to the hotReload update event, which re-generates the outputed content.
/// </summary>
internal sealed class HotReloadManager
{
    internal static bool HotReloadEnabled { get; set; }

    public static void UpdateApplication(Type[]? updatedTypes)
    {
        if (HotReloadEnabled)
        {
            BlazorStaticExtensions.UseBlazorStaticGeneratorOnHotReload();
        }
    }
}
