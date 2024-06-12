---
title: Release of 1.0.0-beta.4
lead: Static web assets are always copied. The IsDraft property in the default FrontMatter is now supported. 
published: 2024-01-20
tags: [release]
authors:
    - name: "Jan Tesař"
      gitHubUserName: "tesar-tech"
      xUserName: "tesar_tech"
---

## Breaking change:

- Use `builder.WebHost.UseStaticWebAssets();` to ensure static assets are copied to the output folder. See [One little catch](#one-little-catch-with-one-liner-solution) for more info.

## Static web assets are always copied

**tl;dr**: It works as expected, just add `builder.WebHost.UseStaticWebAssets();` to your `Program.cs`.

Previously, it wasn't possible to copy static web assets from a Razor class library to the output folder. These assets, residing in the NuGet cache, are served by the framework. Static web assets from RCLs (e.g., CSS files or images) are accessed through the `_content/` path and are copied to the `wwwroot` folder upon publishing. However, BlazorStatic is designed to output the entire website, even during development (without publishing). An addition was made to copy the static web assets from RLCs using `app.Environment.WebRootFileProvider`.

Now, you can use any NuGet package with static web assets (for example, FluentUI; [check how it runs with BlazorStatic](/docs/componentlibs)), and it will work as expected.

This also addresses the lack of support for scoped CSS in BlazorStatic. Use scoped CSS as you normally would.

### One little catch (with one-liner solution)
BlazorStatic now relies on `app.Environment.WebRootFileProvider` to copy static web assets. You need to enable `StaticWebAssets`. In non-dev environments (Staging, Production, etc.), `StaticWebAssets` are turned off. The issue is described [here](https://dev.to/j_sakamoto/how-to-deal-with-the-http-404-content-foo-bar-css-not-found-when-using-razor-component-package-on-asp-net-core-blazor-app-aai) (with older .NET, but the principle is the same):

See it in the `WebHost.cs` file in the [aspnetcore repo](https://github.com/dotnet/aspnetcore/blob/cc9cff31eb828f5849c07afc46b08baeda42b399/src/DefaultBuilder/src/WebHost.cs#L221):

This becomes a problem when switching the `ASPNETCORE_ENVIRONMENT` to anything other than Development, which I recommend doing in GitHub Actions workflows (or similar).

The problem results in missing static web assets (CSS, images, etc.) in the output folder.

How to prevent this? Easily by activating `StaticWebAssets` in your `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseStaticWebAssets(); //👈
```
You might wonder: "Aren't StaticWebAssets turned off in prod for a reason?"

Yes, but: (simply put) for apps that run on a server.

A web app using BlazorStatic is meant for quick static website generation, not server deployment.

If you need to disable `StaticWebAssets`, consider this workaround:

- Add the `wwwroot` folder to `opt.ContentToCopyToOutput`.
- Publish the app before running.

I haven't tested this; let me know if you need it. I am curious about the use case.

### .nojekyll file on GitHub Pages
- Don't forget to add `.nojekyll` file to your `wwwroot` folder, otherwise GitHub Pages will ignore folders starting with
  underscore `_` (e.g. `_content`). [more info](https://github.blog/2009-12-29-bypassing-jekyll-on-github-pages/)
- The best way is to use [build pipeline](https://github.com/tesar-tech/BlazorStaticMinimalBlog/blob/master/.github/workflows/publish-to-ghpages-and-nuget.yml) for that


## IsDraft property in default FrontMatter is now supported

First merged PR from community! Thanks to [Chris Gonzales](https://github.com/chrisg32) for this contribution.

Default `FrontMatter` has now support for `IsDraft` property.
If you set it to `true` the page will be ignored during generation. A useful feature indeed.

Note: You don't have to use the default FrontMatter at all.
You can tailor the front matter class based on your markdown posts' structure.  

## Ignoring files for the generation works

To ignore certain files, configure it in `Program.cs`:

```csharp
builder.Services.AddBlazorStaticService(opt => {
    opt.IgnoredPathsOnContentCopy.AddRange(new[] { "app.css" });
    });
```

For example, I ignore the `app.css` file used by TailwindCSS to create the final CSS version (`app.min.css`).
There's no need to keep `app.css` in the output.

