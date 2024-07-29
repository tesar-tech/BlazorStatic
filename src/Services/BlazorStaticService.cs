namespace BlazorStatic.Services;

using Microsoft.Extensions.Logging;
using System.Reflection;

/// <summary>
/// The BlazorStaticService is responsible for generating static pages for a Blazor application.
/// </summary>
/// <param name="options"></param>
/// <param name="helpers"></param>
/// <param name="logger"></param>
public class BlazorStaticService(BlazorStaticOptions options,
    BlazorStaticHelpers helpers,
    ILogger<BlazorStaticService> logger)
{
    /// <summary>
    /// The BlazorStaticOptions used to configure the generation process.
    /// </summary>
    public BlazorStaticOptions Options => options;
    
    /// <summary>
    /// Generates static pages for the Blazor application. This method performs several key operations:
    /// - Invokes an optional pre-defined blog action.
    /// - Conditionally generates non-parametrized Razor pages based on configuration.
    /// - Clears the existing output folder and creates a fresh one for new content.
    /// - Copies specified content to the output folder.
    /// - Uses an HttpClient to fetch and save the content of each configured page.
    /// The method respects the configuration provided in 'options', including the suppression of file generation,
    /// paths for content copying, and the list of pages to generate.
    /// </summary>
    /// <param name="appUrl">The base URL of the application, used for making HTTP requests to fetch page content.</param>
    internal async Task GenerateStaticPages(string appUrl)
    {

        if (options.AddNonParametrizedRazorPages)
            RegisterNonTokenizedPages();

        foreach (Func<Task> action in options.GetBeforeFilesGenerationActions())
        {
            await action.Invoke();
        }

        if (options.SuppressFileGeneration) return;

        if (Directory.Exists(options.OutputFolderPath)) //clear output folder
            Directory.Delete(options.OutputFolderPath, true);
        Directory.CreateDirectory(options.OutputFolderPath);

        List<string> ignoredPathsWithOutputFolder = options.IgnoredPathsOnContentCopy.Select(x => Path.Combine(options.OutputFolderPath, x)).ToList();
        foreach (var pathToCopy in options.ContentToCopyToOutput)
        {
            logger.LogInformation("Copying {sourcePath} to {targetPath}", pathToCopy.SourcePath, Path.Combine(options.OutputFolderPath, pathToCopy.TargetPath));
            helpers.CopyContent(pathToCopy.SourcePath, Path.Combine(options.OutputFolderPath, pathToCopy.TargetPath), ignoredPathsWithOutputFolder);
        }

        HttpClient client = new() { BaseAddress = new Uri(appUrl) };

        foreach (PageToGenerate page in options.PagesToGenerate)
        {
            if (page is null) continue;

            logger.LogInformation("Generating {pageUrl} into {pageOutputFile}", page.Url, page.OutputFile);
            string content;
            try
            {
                content = await client.GetStringAsync(page.Url);
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning("Failed to retrieve page at {pageUrl}. StatusCode:{statusCode}. Error: {exceptionMessage}", page.Url, ex.StatusCode, ex.Message);
                continue;
            }

            var outFilePath = Path.Combine(options.OutputFolderPath, page.OutputFile.TrimStart('/'));

            string? directoryPath = Path.GetDirectoryName(outFilePath);
            if (!string.IsNullOrEmpty(directoryPath))
                Directory.CreateDirectory(directoryPath);

            await File.WriteAllTextAsync(outFilePath, content);
        }
    }

    /// <summary>
    /// Registers razor pages that have no parameters to be generated as static pages.
    /// </summary>
    private void RegisterNonTokenizedPages()
    {
        var entryAssembly = Assembly.GetEntryAssembly()!;
        List<string?> routesToGenerate = RoutesHelper.GetRoutesToRender(entryAssembly);

        foreach (var route in routesToGenerate)
        {
            if (route is null) continue;
            options.PagesToGenerate.Add(new (route, Path.Combine(route, options.IndexPageHtml)));
        }
    }
}
