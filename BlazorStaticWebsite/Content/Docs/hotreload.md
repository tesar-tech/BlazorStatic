# HotReload

When working with BlazorStatic, you can use `dotnet watch` to watch all the changes and recreate the generated pages on
every change. It is turned on by default

## How to Enable Hot Reload

If you'd like to monitor changes in your `.md` files (and you probably do), you should configure your `.csproj` file to
watch these files ([docs](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-watch#watch-additional-files)):

```xml
<ItemGroup>
    <Watch Include="Content/**/*" />
</ItemGroup>
```

### Disable HotReload

To disable HotReload, modify `Program.cs`:

```csharp
builder.Services.AddBlazorStaticService(opt => {
    //...
    opt.HotReloadEnabled = false;
});
```

### What's the Benefit of HotReload?

The core philosophy of BlazorStatic is simple:
"Run your app as usual, add BlazorStatic, and it will generate the same website you're currently running on localhost."

With hot reload, we're getting even closer to this philosophy by allowing you to see changes instantly as you develop.
Hot reload integrates seamlessly with Blazor SSR (most of the time) and works just as effectively with BlazorStatic.

Previously, when developing your BlazorStatic website, you had to restart the app to see changes—whether in the running
application or the generated files. Now, by using `dotnet watch`, any changes to `.razor`, `.cs`, `.css`, or `.md` files
automatically regenerate the output files.

### Technical Details

Implementing hot reload required three key steps:

1. **HotReloadManager**: This custom class, marked with the `MetadataUpdateHandler` attribute, ensures that
   `UpdateApplication` is called whenever a hot reload event occurs. The method must be static and named exactly as
   specified, though the class name is flexible.

2. **Page Regeneration**: The application triggers BlazorStatic to regenerate the pages and output the HTML files.
   This is handled by invoking `app.UseBlazorStaticGenerator` in `Program.cs` as usual. Cleanup is performed for
   `PagesToGenerate` and `ContentToCopyToOutput`, and options are re-evaluated to account for any changes in
   `AddBlazorStaticService`.

3. **Markdown File Updates**: To update `.md` files, they must be copied to the `bin` folder, where they are parsed and
   added to `PagesToGenerate`. This is facilitated by the `<Watch Include="Content/**/*" />` directive. Changes to these
   files are considered a "rude edit," triggering a full rebuild. However, this process is quick since no significant
   source code changes are involved.

### Limitations

The feature generally works as expected, even for changes in `Program.cs`, which isn't always the case for other
settings. However, there are a few limitations and potential issues:

- The entire output folder is regenerated on every change, regardless of the file type. Modifying a single `.md` file
  will result in deleting and recreating all output. This isn't a problem with a few pages, but performance could
  degrade with larger sites.
  This highlights the need for performance optimization. Previously, optimizations weren't critical because the
  generation process ran only once. This is no longer the case.
- Additional limitations might be discovered as we gather more feedback.

### Feedback

Share any feedback [by creating an issue](https://github.com/tesar-tech/BlazorStatic/issues/new) or by sending a message
in the [Discord server](https://discord.gg/DsAXsMuEbx).
