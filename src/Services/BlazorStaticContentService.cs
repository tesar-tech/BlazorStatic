using System.Reflection;

namespace BlazorStatic.Services;

/// <summary>
///     The BlazorStaticContentService is responsible for parsing and adding blog posts.
///     It adds pages with blog posts to the options.PagesToGenerate list,
///     that is used later by BlazorStaticService to generate static pages.
/// </summary>
/// a
/// <param name="options"></param>
/// <param name="helpers"></param>
/// <param name="blazorStaticService"></param>
/// <typeparam name="TFrontMatter"></typeparam>
public class BlazorStaticContentService<TFrontMatter>(
    BlazorStaticContentOptions<TFrontMatter> options,
    BlazorStaticHelpers helpers,
    BlazorStaticService blazorStaticService)
    where TFrontMatter : class, IFrontMatter, new()
{
    /// <summary>
    ///     The list of posts parsed and added to the BlazorStaticContentService.
    /// </summary>
    public List<Post<TFrontMatter>> Posts => options.Posts;

    /// <summary>
    ///     Obsolete property. Use <see cref="Posts" /> instead. This property will be removed in future versions.
    /// </summary>
    [Obsolete("Use Posts instead. This property will be removed in future versions.")]
    public List<Post<TFrontMatter>> BlogPosts => Posts;


    /// <summary>
    ///     The BlazorStaticContentOptions used to configure the BlazorStaticContentService.
    /// </summary>
    public BlazorStaticContentOptions<TFrontMatter> Options => options;


    /// <summary>
    ///     Obsolete method. Use <see cref="ParseAndAddPosts" /> instead. This method will be removed in future versions.
    /// </summary>
    [Obsolete("Use ParseAndAddPosts instead. This method will be removed in future versions.")]
    public async Task ParseAndAddBlogPosts()
    {
        await ParseAndAddPosts();
    }

    /// <summary>
    ///     Parses and adds posts to the BlazorStaticContentService. This method reads markdown files
    ///     from a specified directory, parses them to extract front matter and content,
    ///     and then adds them as posts to the options.PagesToGenerate.
    /// </summary>
    public async Task ParseAndAddPosts()
    {
        string absContentPath; //gets initialized in GetPostsPath
        var files = GetPostsPath();

        (string, string)? mediaPaths =
            options.MediaFolderRelativeToContentPath == null || options.MediaRequestPath == null
                ? null
                : (options.MediaFolderRelativeToContentPath, options.MediaRequestPath);

        foreach (var file in files)
        {
            var (htmlContent, frontMatter) = await helpers.ParseMarkdownFile<TFrontMatter>(file, mediaPaths);

            if (frontMatter.IsDraft)
            {
                continue;
            }

            Post<TFrontMatter> post = new()
            {
                FrontMatter = frontMatter,
                Url = GetRelativePathWithFilename(file),
                HtmlContent = htmlContent
            };

            options.Posts.Add(post);

            blazorStaticService.Options.PagesToGenerate.Add(new PageToGenerate($"{options.PageUrl}/{post.Url}",
                Path.Combine(options.PageUrl, $"{post.Url}.html"), post.FrontMatter.AdditionalInfo));
        }

        //copy media folder to output
        if (options.MediaFolderRelativeToContentPath != null)
        {
            var pathWithMedia = Path.Combine(options.ContentPath, options.MediaFolderRelativeToContentPath);
            blazorStaticService.Options.ContentToCopyToOutput.Add(new ContentToCopy(pathWithMedia, pathWithMedia));
        }

        //add tags pages
        if (options.AddTagPagesFromPosts)
        {
            // blazorStaticService.Options.PagesToGenerate.Add(new($"{options.TagsPageUrl}", Path.Combine(options.TagsPageUrl, "index.html")));
            foreach (var tag in options.Posts.SelectMany(x => x.FrontMatter.Tags).Distinct()) //gather all unique tags from all posts
            {
                blazorStaticService.Options.PagesToGenerate.Add(new PageToGenerate($"{options.TagsPageUrl}/{tag}",
                    Path.Combine(options.TagsPageUrl, $"{tag}.html")));
            }
        }

        options.AfterContentParsedAndAddedAction?.Invoke();
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
                    .FullName; //! is ok, null only in empty path or root

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
}
