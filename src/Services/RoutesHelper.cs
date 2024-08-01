namespace BlazorStatic.Services;

using System.Reflection;
using Microsoft.AspNetCore.Components;

// borrowed code with minor changes
// https://andrewlock.net/finding-all-routable-components-in-a-webassembly-app/
internal static class RoutesHelper
{
    /// <summary>
    /// Gets the static routes of a blazor app
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
        var routes = components
            .Select(GetRouteFromComponent)
            .Where(route => route is not null)
            .ToList();

        return routes!;
    }

    /// <summary>
    ///  Gets the route from a blazor component
    /// </summary>
    /// <param name="component"></param>
    /// <returns>
    /// The route of the component.
    /// Returns null if the component is not a page or the route has parameters.
    /// </returns>
    private static string? GetRouteFromComponent(Type component)
    {
        var attributes = component.GetCustomAttributes(inherit: true);

        var routeAttribute = attributes.OfType<RouteAttribute>().FirstOrDefault();

        if (routeAttribute is null)
        {
            // Only map rout-able components
            return null;
        }

        var route = routeAttribute.Template;

        // can't work with parameterized pages
        if (route.Contains('{'))
        {
            return null;
        }

        return route;
    }
}
