namespace BlazorStatic.Services;

using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

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
        if (options.SuppressFileGeneration) return;

        if (options.AddNonParametrizedRazorPages)
            AddNonParametrizedRazorPages();
        
        foreach (Func<Task> action in options.GetBeforeFilesGenerationActions())
        {
            await action.Invoke();
        }

        if (Directory.Exists(options.OutputFolderPath))//clear output folder
            Directory.Delete(options.OutputFolderPath, true);
        Directory.CreateDirectory(options.OutputFolderPath);

        List<string> ignoredPathsWithOutputFolder = options.IgnoredPathsOnContentCopy.Select(x => Path.Combine(options.OutputFolderPath, x)).ToList();
        foreach (var pathToCopy in options.ContentToCopyToOutput)
        {
            logger.LogInformation("Copying {sourcePath} to {targetPath}", pathToCopy.SourcePath, Path.Combine(options.OutputFolderPath,  pathToCopy.TargetPath ));
            helpers.CopyContent(pathToCopy.SourcePath, Path.Combine(options.OutputFolderPath, pathToCopy.TargetPath), ignoredPathsWithOutputFolder);
        }


        HttpClient client = new() { BaseAddress = new Uri(appUrl) };

        foreach (PageToGenerate page in options.PagesToGenerate)
        {
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

            var outFilePath = Path.Combine(options.OutputFolderPath, page.OutputFile);

            string? directoryPath = Path.GetDirectoryName(outFilePath);
            if (directoryPath != null)
                Directory.CreateDirectory(directoryPath);
            await File.WriteAllTextAsync(outFilePath, content);
        }
    }
    /// <summary>
    /// Adds non-parametrized Razor pages to the list of pages to be generated as static content.
    /// This method scans specified directories for Razor files and identifies those with a @page directive
    /// that do not include parameters (i.e., no '{}' in the directive). For each matching file, it constructs
    /// a URL based on the @page directive and determines a corresponding file path for the static HTML output.
    /// The method assumes that each URL path should map to a folder structure with an 'index.html' file in it,
    /// as specified in options.IndexPageHtml.
    /// </summary>
    /// <remarks>
    /// This operation is controlled by the options.AddNonParametrizedRazorPages flag. Only Razor files in
    /// directories specified in options.RazorPagesPaths are considered. The method employs regular expression
    /// matching to identify suitable @page directives.
    /// </remarks>
    void AddNonParametrizedRazorPages()
    {
        if (!options.AddNonParametrizedRazorPages) return;

        var regex = new Regex("""
                              @page "/(?!.*\{.*\})(.*?)"
                              """);// Regular expression to match @page directive, but ignore when {} are present

        var allRazorFilePaths = new List<string>();

        foreach (var path in options.RazorPagesPaths)
        {
            var filePaths = Directory.GetFiles(path, "*.razor");
            allRazorFilePaths.AddRange(filePaths);
        }
        foreach (var filePath in allRazorFilePaths)
        {
            var content = File.ReadAllText(filePath);

            var match = regex.Match(content);
            if (match is not { Success: true, Groups.Count: > 1 })
                continue;
            string url = match.Groups[1].Value;
            string file = Path.Combine(url, options.IndexPageHtml);//for @page "/blog" generate blog/index.html 
            options.PagesToGenerate.Add(new($"{url}", file));
        }
    }
}





