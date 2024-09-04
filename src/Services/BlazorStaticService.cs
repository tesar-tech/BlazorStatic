namespace BlazorStatic.Services;

using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Xml.Linq;
using System.Text;
using System.Web;

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
        if (options.AddPagesWithoutParameters)
            AddPagesWithoutParameters();

        foreach (Func<Task> action in options.GetBeforeFilesGenerationActions())
        {
            await action.Invoke();
        }

        if (options.SuppressFileGeneration) return;

        if (Directory.Exists(options.OutputFolderPath)) //clear output folder
            Directory.Delete(options.OutputFolderPath, true);
        Directory.CreateDirectory(options.OutputFolderPath);
        
        //sitemap generation has to happen before copying the wwwroot files, because it outputs the sitemap there
        if (Options.ShouldGenerateSitemap)
            await TryGenerateSitemap(); 

        List<string> ignoredPathsWithOutputFolder = options.IgnoredPathsOnContentCopy.Select(x => Path.Combine(options.OutputFolderPath, x)).ToList();
        foreach (var pathToCopy in options.ContentToCopyToOutput)
        {
            logger.LogInformation("Copying {sourcePath} to {targetPath}", pathToCopy.SourcePath, Path.Combine(options.OutputFolderPath, pathToCopy.TargetPath));
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

            var outFilePath = Path.Combine(options.OutputFolderPath, page.OutputFile.TrimStart('/'));

            string? directoryPath = Path.GetDirectoryName(outFilePath);
            if (!string.IsNullOrEmpty(directoryPath))
                Directory.CreateDirectory(directoryPath);

            await File.WriteAllTextAsync(outFilePath, content);

        }

      
    }

    /// <summary>
    /// Generates an XML sitemap from the registered URLs. <br />
    /// !requires BlazorStaticOptions.SiteUrl to not be null!
    /// Otherwise, returns with warning
    /// </summary>
    private async Task TryGenerateSitemap()
    {
        if (string.IsNullOrWhiteSpace(Options.SiteUrl))
        {
            logger.LogWarning("'BlazorStaticOptions.SiteUrl' is null or empty! Can't generate Sitemap." +
            " Either provide the site url or set 'BlazorStaticOptions.ShouldGenerateSitemap' to false");
            return;
        }

        XNamespace xmlns = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9");
        List<XElement> xmlUrlList = [];

        foreach (PageToGenerate page in options.PagesToGenerate)
        {
            string pageUrl = Options.SiteUrl.TrimEnd('/') + EncodeUrl(page.Url);
            List<XElement> xElements = [new XElement(xmlns + "loc", pageUrl)];

            // only add a <lastmod> node if the file is a blog post
            if (page.AdditionalInfo?.LastMod != null)
            {
                xElements.Add(new XElement(xmlns + "lastmod", $"{page.AdditionalInfo.LastMod:yyyy-MM-dd}"));
            }

            xmlUrlList.Add(new XElement(xmlns + "url", xElements));
        }

        XDocument xDocument = new(
            new XDeclaration("1.0", "UTF-8", null),
            new XElement(xmlns + "urlset", xmlUrlList)
        );

        const string sitemapFileName = "sitemap.xml";
        string sitemapPath = Path.Combine(options.SitemapOutputFolderPath, sitemapFileName);
        await File.WriteAllTextAsync(sitemapPath, xDocument.Declaration + xDocument.ToString());
        logger.LogInformation("Sitemap generated into {pageOutputFile}",  sitemapPath);
        options.ContentToCopyToOutput.Add(new(sitemapPath,sitemapFileName));//it is not copied with wwwroot as 
        return;

        static string EncodeUrl(string url)
        {
            string encodedUrl = HttpUtility.UrlEncode(url, Encoding.UTF8).Replace("%2f", "/");
            return encodedUrl.StartsWith('/') ? encodedUrl : '/' + encodedUrl;
        }
    }

    /// <summary>
    /// Registers razor pages that have no parameters to be generated as static pages.
    /// Page is defined by Route parameter: either `@page "Example"` or `@attribute [Route("Example")]`
    /// </summary>
    private void AddPagesWithoutParameters()
    {
        var entryAssembly = Assembly.GetEntryAssembly()!;
        List<string> routesToGenerate = RoutesHelper.GetRoutesToRender(entryAssembly);

        foreach (var route in routesToGenerate)
        {
            options.PagesToGenerate.Add(new(route, Path.Combine(route, options.IndexPageHtml)));
        }
    }
}
