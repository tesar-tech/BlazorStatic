using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace BlazorStatic.Services;

// borrowed code with minor changes
// https://andrewlock.net/finding-all-routable-components-in-a-webassembly-app/
internal static class RoutesHelper
{
    /// <summary>
    ///     Gets the static routes of a blazor app
    /// </summary>
    /// <param name="assembly">assembly of the blazor app</param>
    /// <returns>a list of static routes</returns>
    public static List<string> GetRoutesToRender(Assembly assembly)
    {
        // Get all the components whose base class is ComponentBase
        var components = assembly
            .ExportedTypes
            .Where(t => t.IsSubclassOf(typeof(ComponentBase)));

        // get all the routes that don't contain parameters
        List<string> routes = components
            .Select(GetRouteFromComponent)
            .Where(route => route is not null)
            .ToList()!;//previous null check guarantee not nulls 

        return routes;
    }

    /// <summary>
    ///     Gets the route from a blazor component
    /// </summary>
    /// <param name="component"></param>
    /// <returns>
    ///     The route of the component.
    ///     Returns null if the component is not a page (doesn't have RouteAttr) or the route has parameters.
    /// </returns>
    private static string? GetRouteFromComponent(Type component)
    {
        var attributes = component.GetCustomAttributes(true);

        // can't work with parameterized pages (such pages has params defined with {paramName})
        return attributes.OfType<RouteAttribute>()
            .FirstOrDefault(x => !x.Template.Contains('{'))?.Template;
    }
}
