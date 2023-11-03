namespace BlazorStatic;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public static class BlazorStaticExtensions
{
    public static IServiceCollection AddBlazorStaticServices<TFrontMatter>(this IServiceCollection services,
        Action<BlazorStaticOptions<TFrontMatter>>? configureOptions = null)
        where TFrontMatter : class, IFrontMatter
    {
        var options = new BlazorStaticOptions<TFrontMatter>();
        configureOptions?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<BlazorStaticService<TFrontMatter>>();

        return services;
    }

    public static void UseBlazorStaticGenerator<TFrontMatter>(this WebApplication app, bool shutdownApp = false)
        where TFrontMatter : class, IFrontMatter
    {
        var blazorStaticService = app.Services.GetRequiredService<BlazorStaticService<TFrontMatter>>();

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
