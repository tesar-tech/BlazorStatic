namespace BlazorStatic;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services;

/// <summary>
/// Extension methods for configuring BlazorStatic services in an <see cref="IServiceCollection" />.
/// </summary>
public static class BlazorStaticExtensions
{
    /// <summary>
    /// holds the actions that will run when UseBlazorStaticGenerator is called.
    /// Will manage content for every BlazorStaticContentService added.
    /// </summary>
    private static List<Action<WebApplication>> staticContentUse { get; set; } = [];


    /// <summary>
    /// Adds a blog service to the specified IServiceCollection. The blog service uses a generic type
    /// for front matter, allowing customization of the metadata format used in blog posts.
    /// </summary>
    /// <typeparam name="TFrontMatter">The type of front matter used in the blog posts. Must implement IFrontMatter.</typeparam>
    /// <param name="services">The IServiceCollection to add the blog service to.</param>
    /// <param name="configureOptions">An optional action to configure the BlazorStaticContentOptions for the BlazorStaticConentService.
    /// Default values are set for Blog content.</param>
    /// <returns>The IServiceCollection, with the blog service added, allowing for method chaining.</returns>
    /// <remarks>
    /// The method configures and registers a singleton instance of BlazorStaticContentOptions`TFrontMatter` and 
    /// BlazorStaticContentService`TFrontMatter` in the service collection.
    /// </remarks>
    public static IServiceCollection AddBlazorStaticContentService<TFrontMatter>(this IServiceCollection services,
        Action<BlazorStaticContentOptions<TFrontMatter>>? configureOptions = null)
        where TFrontMatter : class, IFrontMatter, new()
    {
        var options = new BlazorStaticContentOptions<TFrontMatter>();
        configureOptions?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<BlazorStaticContentService<TFrontMatter>>();
        staticContentUse.Add(UseBlazorStaticContent<TFrontMatter>);

        return services;
    }


    /// <summary>
    /// Adds the Blazor static generation service to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the Blazor static service to.</param>
    /// <param name="configureOptions">An optional action to configure the BlazorStaticOptions for the service.</param>
    /// <returns>The IServiceCollection, with the Blazor static service added, allowing for method chaining.</returns>
    /// <remarks>
    /// The method registers a singleton instance of BlazorStaticHelpers and BlazorStaticService in the service collection.
    /// Additionally, it creates and configures an instance of BlazorStaticOptions, which is used to control the behavior
    /// of the static generation process. This setup is essential for applications leveraging Blazor for static site generation,
    /// providing customizable options for the generation process.
    /// </remarks>
    public static IServiceCollection AddBlazorStaticService(this IServiceCollection services,
        Action<BlazorStaticOptions>? configureOptions = null)
    {

        services.AddSingleton<BlazorStaticHelpers>();
        var options = new BlazorStaticOptions();
        configureOptions?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<BlazorStaticService>();

        return services;
    }


    /// <summary>
    /// Runs the actions necessary to generating static content by settings defined in options
    /// </summary>
    /// <param name="app"></param>
    /// <typeparam name="TFrontMatter"></typeparam>
    static void UseBlazorStaticContent<TFrontMatter>(WebApplication app)
        where TFrontMatter : class, IFrontMatter, new()
    {
        var blogService = app.Services.GetRequiredService<BlazorStaticContentService<TFrontMatter>>();
        var options = app.Services.GetRequiredService<BlazorStaticContentOptions<TFrontMatter>>();
        var blazorStaticService = app.Services.GetRequiredService<BlazorStaticService>();


        //Add static files for media files to be accessible while running the app

        //StaticFileOptions doesn't like ".." parent dir (e.g "Content/Blog/en/../media")
        //This converts it to "/Content/Blog/media"...

        if (options.MediaRequestPath is not null && options.MediaFolderRelativeToContentPath is not null)
        {
            string requestPath = "/" + Path.GetFullPath(options.MediaRequestPath)[Directory.GetCurrentDirectory().Length..]
                .TrimStart(Path.DirectorySeparatorChar)
                .Replace("\\", "/");


            string realPath = Path.Combine(Directory.GetCurrentDirectory(), options.ContentPath, options.MediaFolderRelativeToContentPath);
            if (!Directory.Exists(realPath))
            {
                app.Logger.LogWarning("folder for media path ({Folder}) doesn't exist", realPath);
            }
            else
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(realPath),
                    RequestPath = requestPath
                });
            }
        }
        //

        blazorStaticService.Options.AddBeforeFilesGenerationAction(blogService.ParseAndAddBlogPosts);//will run later in GenerateStaticPages

    }

    /// <summary>
    /// Adds the Blazor static generation service to the specified IServiceCollection.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="shutdownApp"></param>
    public static void UseBlazorStaticGenerator(this WebApplication app, bool shutdownApp = false)
    {
        foreach (Action<WebApplication> use in staticContentUse)
            use.Invoke(app);

        var blazorStaticService = app.Services.GetRequiredService<BlazorStaticService>();

        //adds wwwroot files (or any other files that has been added as static content) to the output
        AddStaticWebAssetsToOutput(app.Environment.WebRootFileProvider, string.Empty, blazorStaticService);

        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

        var logger = app.Services.GetRequiredService<ILogger<BlazorStaticService>>();

        lifetime.ApplicationStarted.Register(
        async () => {
            try
            {
                await blazorStaticService.GenerateStaticPages(app.Urls.First()).ConfigureAwait(false);
                if (shutdownApp)
                    lifetime.StopApplication();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while generating static pages: {ErrorMessage}", ex.Message);
            }
        }
        );
    }
    
    /// <summary>
    /// Takes the provider, search it recursively and add all the files found.
    /// </summary>
    /// <param name="fileProvider"></param>
    /// <param name="subPath"></param>
    /// <param name="blazorStaticService"></param>
    private static void AddStaticWebAssetsToOutput(IFileProvider fileProvider, string subPath, BlazorStaticService blazorStaticService)
    {
        IDirectoryContents contents = fileProvider.GetDirectoryContents(subPath);

        foreach (var item in contents)
        {
            string fullPath = $"{subPath}{item.Name}";
            if (item.IsDirectory)
            {
                AddStaticWebAssetsToOutput(fileProvider, $"{fullPath}/", blazorStaticService);
            }
            else
            {
                if (item.PhysicalPath is not null)
                    blazorStaticService.Options.ContentToCopyToOutput.Add(new(item.PhysicalPath, fullPath));
            }
        }
    }
}
