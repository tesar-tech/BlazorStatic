using System.Reflection;

namespace BlazorStatic.Services;

using Microsoft.Extensions.Logging;

/// <summary>
///     The BlazorStaticContentService is responsible for parsing and adding blog posts.
///     It adds pages with blog posts to the options.PagesToGenerate list,
///     that is used later by BlazorStaticService to generate static pages.
/// </summary>
/// /// <typeparam name="TFrontMatter"></typeparam>
public class BlazorStaticContentService<TFrontMatter>(
    BlazorStaticContentOptions<TFrontMatter> options,
    BlazorStaticHelpers helpers,
    BlazorStaticService blazorStaticService,
    ILogger<BlazorStaticContentService<TFrontMatter>> logger)
    where TFrontMatter : class, IFrontMatter, new()
{
    /// <summary>
    /// Place where processed blog posts live (their HTML and front matter).
    /// </summary>
    public List<Post<TFrontMatter>> Posts { get; } = [];

    /// <summary>
    ///     The BlazorStaticContentOptions used to configure the BlazorStaticContentService.
    /// </summary>
    public BlazorStaticContentOptions<TFrontMatter> Options => options;

    /// <summary>
    ///     Parses and adds posts to the BlazorStaticContentService. This method reads markdown files
    ///     from a specified directory, parses them to extract front matter and content,
    ///     and then adds them as posts to the options.PagesToGenerate.
    /// </summary>
    public async Task ParseAndAddPosts()
    {
        string absContentPath;//gets initialized in GetPostsPath
        var files = GetPostsPath();

        (string, string)? mediaPaths =
            options.MediaFolderRelativeToContentPath == null || options.MediaRequestPath == null
                ? null
                : (options.MediaFolderRelativeToContentPath, options.MediaRequestPath);

        foreach(var file in files)
        {
            var (htmlContent, frontMatter) = await helpers.ParseMarkdownFile<TFrontMatter>(file, mediaPaths);

            if(frontMatter.IsDraft)
            {
                continue;
            }
            Post<TFrontMatter> post = new()
            {
                FrontMatter = frontMatter,
                Url = GetRelativePathWithFilename(file),
                HtmlContent = htmlContent
            };

            Posts.Add(post);

            blazorStaticService.Options.PagesToGenerate.Add(new PageToGenerate($"{options.PageUrl}/{post.Url}",
            Path.Combine(options.PageUrl, $"{post.Url}.html"), post.FrontMatter.AdditionalInfo));
        }

        //copy media folder to output
        if(options.MediaFolderRelativeToContentPath != null)
        {
            var pathWithMedia = Path.Combine(options.ContentPath, options.MediaFolderRelativeToContentPath);
            blazorStaticService.Options.ContentToCopyToOutput.Add(new ContentToCopy(pathWithMedia, pathWithMedia));
        }

        ProcessTags();

        options.AfterContentParsedAndAddedAction?.Invoke(blazorStaticService,this);
        return;

        string[] GetPostsPath()
        {
            //retrieves post from bin folder, where the app is running
            EnumerationOptions enumerationOptions = new()
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = true
            };

            var execFolder =
                Directory.GetParent((Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).Location)!
                    .FullName;//! is ok, null only in empty path or root

            absContentPath = Path.Combine(execFolder, options.ContentPath);
            return Directory.GetFiles(absContentPath, options.PostFilePattern, enumerationOptions);
        }

        //ex: file= "C:\Users\user\source\repos\MyBlog\Content\Blog\en\somePost.md"
        //returns "en/somePost"
        string GetRelativePathWithFilename(string file)
        {
            var relativePathWithFileName = Path.GetRelativePath(absContentPath, file);
            return Path.Combine(Path.GetDirectoryName(relativePathWithFileName)!, Path.GetFileNameWithoutExtension(relativePathWithFileName))
                .Replace("\\", "/");
        }
    }

    /// <summary>
    /// A dictionary of unique Tags parsed from the FrontMatter of all posts.
    /// Each Tag is distinct, and every Post references a collection of these Tag objects.
    /// </summary>
    public Dictionary<string, Tag> AllTags { get; private set; } = [];


    private void ProcessTags()
    {
        if(!typeof(IFrontMatterWithTags).IsAssignableFrom(typeof(TFrontMatter)))
        {
            if(options.Tags.AddTagPagesFromPosts)
                logger.LogWarning("BlazorStaticContentOptions.Tags.AddTagPagesFromPosts is true, but the used FrontMatter does not inherit from IFrontMatterWithTags. No tags were processed.");
            return;
        }
        //gather List<string> tags and create Tag objects from them.
        AllTags = Posts
            .SelectMany(post => (post.FrontMatter as IFrontMatterWithTags)?.Tags ?? Enumerable.Empty<string>())
            .Distinct()
            .Select(tag => new Tag { Name = tag, EncodedName = options.Tags.TagEncodeFunc(tag) })
            .ToDictionary(tag => tag.Name);


        foreach(var post in Posts)
        {
            //add Tag objects to every post based on the front matter tags
            post.Tags = ((IFrontMatterWithTags)post.FrontMatter).Tags
                .Where(tagName => AllTags.ContainsKey(tagName))
                .Select(tagName => AllTags[tagName])
                .ToList();
        }

        if(!options.Tags.AddTagPagesFromPosts) return;

        if(options.Tags.TagsPageUrl is null)
        {
            logger.LogWarning("BlazorStaticContentService.Options.Tags.TagsPageUrl is null, but AddTagPagesFromPosts is true");
            return;
        }
        foreach(var tag in AllTags.Values)
        {
            blazorStaticService.Options.PagesToGenerate.Add(new PageToGenerate($"{options.Tags.TagsPageUrl}/{tag.EncodedName}",
            Path.Combine(options.Tags.TagsPageUrl, $"{tag.EncodedName}.html")));
        }
    }

}
