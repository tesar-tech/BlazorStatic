namespace BlazorStatic.Services;

using System.Reflection;
using Microsoft.AspNetCore.Components;

// borrowed code with minor changes
// https://andrewlock.net/finding-all-routable-components-in-a-webassembly-app/
internal static class RoutesHelper
{
    public static List<string?> GetRoutesToRender(Assembly assembly)
    {
        // Get all the components whose base class is ComponentBase
        var components = assembly
            .ExportedTypes
            .Where(t => t.IsSubclassOf(typeof(ComponentBase)));

        var routes = components
            .Select(GetRouteFromComponent)
            .Where(config => config is not null)
            .ToList();

        return routes;
    }

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

        if (string.IsNullOrEmpty(route))
        {
            throw new Exception($"RouteAttribute in component '{component}' has empty route template");
        }

        // Doesn't support tokens yet
        if (route.Contains('{'))
        {
            // throw new Exception($"RouteAttribute for component '{component}' contains route values. Route values are invalid for prerendering");
            return null;
        }

        return route;
    }
}
