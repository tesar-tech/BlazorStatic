namespace BlazorStatic.Services;

using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

public class BlazorStaticService(BlazorStaticOptions options,
    BlazorStaticHelpers helpers,
    ILogger<BlazorStaticService> logger)
{
    public BlazorStaticOptions Options => options;

    internal Func<Task>? BlogAction { get; set; }
    internal async Task GenerateStaticPages(string appUrl)
    {
        if (BlogAction is not null)
            await BlogAction.Invoke();
        if (options.SuppressFileGeneration) return;

        if (options.AddNonParametrizedRazorPages)
            AddNonParametrizedRazorPages();

        options.BeforeFilesGenerationAction?.Invoke();

        if (Directory.Exists(options.OutputFolderPath))//clear output folder
            Directory.Delete(options.OutputFolderPath, true);
        Directory.CreateDirectory(options.OutputFolderPath);

        foreach (var pathToCopy in options.ContentToCopyToOutput)
        {
            logger.LogInformation("Copying {sourcePath} to {targetPath}", pathToCopy.SourcePath, Path.Combine(options.OutputFolderPath,  pathToCopy.TargetPath ));
            helpers.CopyContent(pathToCopy.SourcePath, Path.Combine(options.OutputFolderPath, pathToCopy.TargetPath), options.IgnoredPathsOnContentCopy);
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





