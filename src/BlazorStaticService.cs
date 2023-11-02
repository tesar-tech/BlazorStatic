namespace BlazorStatic;

using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class BlazorStaticService<TFrontMatter>(BlazorStaticOptions<TFrontMatter> options,
    ILogger<BlazorStaticService<TFrontMatter>> logger)
    where TFrontMatter : class, IFrontMatter
{

    public List<Post<TFrontMatter>> BlogPosts => options.BlogPosts;
    internal bool ShutdownAppAfterFileGeneration => options.ShutdownAppAfterFileGeneration;
    public async Task GenerateStaticPages(string appUrl)
    {
        
        if (options.AddBlogPosts)
            await ParseBlogPosts();

        if (options.SuppressFileGeneration) return;
        
        if (options.AddNonParametrizedRazorPages)
            AddNonParametrizedRazorPages();
        
        if (options.AddBlogPosts)
            foreach (var post in options.BlogPosts)
            {
                options.PagesToGenerate.Add(new($"blog/{post.FileNameNoExtension}", Path.Combine("blog", $"{post.FileNameNoExtension}.html")));
            }

        if (options.AddTagPagesFromBlogPosts)
            foreach (var tag in options.BlogPosts.SelectMany(x => x.FrontMatter.Tags).Distinct())//gather all unique tags from all blog posts
            {
                options.PagesToGenerate.Add(new($"/tag/{tag}", Path.Combine("Tag", $"{tag}.html")));
            }
        
        options.AddExtraPages?.Invoke();

        if (Directory.Exists(options.OutputFolderPath))//clear output folder
            Directory.Delete(options.OutputFolderPath, true);
        Directory.CreateDirectory(options.OutputFolderPath);
        foreach (var pathToCopy in options.ContentToCopyToOutput)
        {
            CopyContent(pathToCopy.SourcePath, Path.Combine(options.OutputFolderPath, pathToCopy.TargetPath), options.IgnoredPathsOnContentCopy);
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
    /// Asynchronously parses the markdown blog posts located in the specified directory, extracts the front matter,
    /// converts the markdown content to HTML, and populates the <c>options.BlogPosts</c> collection with the parsed posts.
    /// </summary>
    /// <remarks>
    /// This method performs the following steps for each markdown file found in the <c>options.BlogContentPath</c> directory
    /// matching the <c>options.BlogPostFilePattern</c> file pattern:
    /// <list type="number">
    /// <item>Creates a new <see cref="Markdig.MarkdownPipeline"/> for parsing markdown with YAML front matter.</item>
    /// <item>Reads the markdown content from the file asynchronously.</item>
    /// <item>Parses the markdown content to extract the YAML front matter and the markdown body.</item>
    /// <item>Deserializes the YAML front matter to an instance of <c>TFrontMatter</c>.</item>
    /// <item>Converts the markdown body to HTML.</item>
    /// <item>Creates a new <c>Post&lt;TFrontMatter&gt;</c> object populated with the parsed front matter, HTML content,
    /// and file name (without extension).</item>
    /// <item>Adds the created <c>Post&lt;TFrontMatter&gt;</c> object to the <c>options.BlogPosts</c> collection.</item>
    /// </list>
    /// </remarks>
    /// <returns>
    /// A <see cref="System.Threading.Tasks.Task"/> that represents the asynchronous operation.
    /// </returns>
    async Task ParseBlogPosts()
    {
        var files = Directory.GetFiles(options.BlogContentPath, options.BlogPostFilePattern);

        foreach (string file in files)
        {
            MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                .Build();
            string markdownContent = await File.ReadAllTextAsync(file);
            MarkdownDocument document = Markdown.Parse(markdownContent, pipeline);

            YamlFrontMatterBlock? yamlBlock = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();
            if (yamlBlock == null)
                continue;
            string frontMatterYaml = yamlBlock.Lines.ToString();
            IDeserializer deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            TFrontMatter frontMatter = deserializer.Deserialize<TFrontMatter>(frontMatterYaml);

            string contentWithoutFrontMatter = markdownContent[(yamlBlock.Span.End + 1)..];
            string htmlContent = Markdown.ToHtml(contentWithoutFrontMatter, pipeline);

            Post<TFrontMatter> post = new()
            {
                FrontMatter = frontMatter,
                FileNameNoExtension = Path.GetFileNameWithoutExtension(file),
                HtmlContent = htmlContent
            };
            options.BlogPosts.Add(post);
        }
    }



    void CopyContent(string sourcePath, string targetPath, List<string> ignoredPaths)
    {
        if (File.Exists(sourcePath))//source path is a file
        {
            string? dir = Path.GetDirectoryName(targetPath);
            if (dir == null) return;
            Directory.CreateDirectory(dir);
            File.Copy(sourcePath, targetPath, true);
            return;
        }
        if (!Directory.Exists(sourcePath))
        {
            logger.LogError("Source path ({sourcePath}) does not exist", sourcePath);
            return;
        }

        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }
        var ignoredPathsWithTarget = ignoredPaths.Select(x => Path.Combine(targetPath, x)).ToList();


        //Now Create all of the directories 
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            string newDirPath = ChangeRootFolder(dirPath);
            if (ignoredPathsWithTarget.Contains(newDirPath))//folder is mentioned in ignoredPaths, don't create it
                continue;
            Directory.CreateDirectory(newDirPath);
        }
        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            string newPathWithNewDir = ChangeRootFolder(newPath);
            if (ignoredPathsWithTarget.Contains(newPathWithNewDir)//file is mentioned in ignoredPaths
                || !Directory.Exists(Path.GetDirectoryName(newPathWithNewDir)))//folder where this file resides is mentioned in ignoredPaths
                continue;
            File.Copy(newPath, newPathWithNewDir, true);
        }
        return;

        string ChangeRootFolder(string dirPath)//for example  from "wwwroot/imgs" to "output/imgs" (safer string.Replace)
        {
            string relativePath = dirPath[sourcePath.Length..].TrimStart(Path.DirectorySeparatorChar);
            return Path.Combine(targetPath, relativePath);
        }
    }


    void AddNonParametrizedRazorPages()
    {
        if (!options.AddNonParametrizedRazorPages) return;

        var regex = new Regex("""
                              @page "/(?!.*\{.*\})(.*?)"
                              """);// Regular expression to match @page directive, but ignore when {} are present

        var filePaths = Directory.GetFiles(options.RazorPagesPath, "*.razor");// Get all .razor files
        foreach (var filePath in filePaths)
        {
            var content = File.ReadAllText(filePath);

            var match = regex.Match(content);
            if (match is not { Success: true, Groups.Count: > 1 })
                continue;
            string url = match.Groups[1].Value;
            string file = url == "" ? options.IndexPageHtml : $"{url}.html";
            options.PagesToGenerate.Add(new($"/{url}", file));
        }
    }
}
