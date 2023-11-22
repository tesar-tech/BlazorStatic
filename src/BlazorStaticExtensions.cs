namespace BlazorStatic;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Services;

public static class BlazorStaticExtensions
{
    public static IServiceCollection AddBlogService<TFrontMatter>(this IServiceCollection services,
        Action<BlogOptions<TFrontMatter>>? configureOptions = null)
        where TFrontMatter : class, IFrontMatter, new()
    {
        var options = new BlogOptions<TFrontMatter>();
        configureOptions?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<BlogService<TFrontMatter>>();

        return services;
    }

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
    
    
    public static void UseBlog<TFrontMatter>(this WebApplication app)
        where TFrontMatter : class, IFrontMatter, new()
    {
        var blogService = app.Services.GetRequiredService<BlogService<TFrontMatter>>();
        var options = app.Services.GetRequiredService<BlogOptions<TFrontMatter>>();
        var blazorStaticService = app.Services.GetRequiredService<BlazorStaticService>();
        
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),
            options.ContentPath,options.MediaFolderRelativeToContentPath)),
            RequestPath = "/" + options.MediaRequestPath
        });
        blazorStaticService.BlogAction = blogService.ParseAndAddBlogPosts;//will run later 
        //in GenerateStaticPages
        
    }
    
    public static void UseBlazorStaticGenerator(this WebApplication app, bool shutdownApp = false)
    {
        var blazorStaticService = app.Services.GetRequiredService<BlazorStaticService>();
        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

        lifetime.ApplicationStarted.Register(
        async () => {
            await blazorStaticService.GenerateStaticPages(app.Urls.First());
            if (shutdownApp)
                lifetime.StopApplication();
        }
        );
    }
}
