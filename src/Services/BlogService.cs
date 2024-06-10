namespace BlazorStatic.Services;

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// The BlogService is responsible for parsing and adding blog posts.
/// It adds pages with blog posts to the options.PagesToGenerate list,
/// that is used later by BlazorStaticService to generate static pages.
/// </summary>
/// <param name="options"></param>
/// <param name="helpers"></param>
/// <param name="blazorStaticService"></param>
/// <typeparam name="TFrontMatter"></typeparam>
public class BlogService<TFrontMatter>(
    BlogOptions<TFrontMatter> options,
    BlazorStaticHelpers helpers,
    BlazorStaticService blazorStaticService)
    where TFrontMatter : class, IFrontMatter, new()
{
    /// <summary>
    /// The list of blog posts parsed and added to the blog service.
    /// </summary>
    public List<Post<TFrontMatter>> BlogPosts => options.Posts;
    /// <summary>
    /// The BlogOptions used to configure the blog service.
    /// </summary>
    public BlogOptions<TFrontMatter> Options => options;
    /// <summary>
    /// Parses and adds blog posts to the blog service. This method reads markdown files
    /// from a specified directory, parses them to extract front matter and content,
    /// and then adds them as blog posts to the options.PagesToGenerate.
    /// </summary>
    public async Task ParseAndAddBlogPosts()
    {
        string absContentPath;//gets initialized in GetPostsPath
        var files = GetPostsPath();

        (string, string)? mediaPaths =
            options.MediaFolderRelativeToContentPath == null || options.MediaRequestPath == null
                ? null
                : (options.MediaFolderRelativeToContentPath, options.MediaRequestPath);

        foreach (string file in files)
        {
            var (htmlContent, frontMatter) = await helpers.ParseMarkdownFile<TFrontMatter>(file, mediaPaths);

            if (frontMatter.IsDraft) continue;

            Post<TFrontMatter> post = new()
            {
                FrontMatter = frontMatter,
                Url = GetRelativePathWithFilename(file),
                HtmlContent = htmlContent
            };
            options.Posts.Add(post);

            blazorStaticService.Options.PagesToGenerate.Add(new($"{options.BlogPageUrl}/{post.Url}", Path.Combine(options.BlogPageUrl, $"{post.Url}.html")));
        }

        //copy media folder to output
        if (options.MediaFolderRelativeToContentPath!=null)
        {
            string pathWithMedia = Path.Combine(options.ContentPath, options.MediaFolderRelativeToContentPath);
            blazorStaticService.Options.ContentToCopyToOutput.Add(new(pathWithMedia, pathWithMedia));
        }

        //add tags pages
        if (options.AddTagPagesFromPosts)
        {
            // blazorStaticService.Options.PagesToGenerate.Add(new($"{options.TagsPageUrl}", Path.Combine(options.TagsPageUrl, "index.html")));   
            foreach (var tag in options.Posts.SelectMany(x => x.FrontMatter.Tags).Distinct())//gather all unique tags from all blog posts
            {
                blazorStaticService.Options.PagesToGenerate.Add(new($"{options.TagsPageUrl}/{tag}", Path.Combine(options.TagsPageUrl, $"{tag}.html")));
            }
        }
        options.AfterBlogParsedAndAddedAction?.Invoke();
        return;

        string[]  GetPostsPath(){//retrieves blog post from bin folder, where the app is running
            EnumerationOptions enumerationOptions = new()
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = true,
            };
            string execFolder = Directory.GetParent((Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).Location)!.FullName;//! is ok, null only in empty path or root
            absContentPath = Path.Combine(execFolder, options.ContentPath);
            return Directory.GetFiles(absContentPath, options.PostFilePattern, enumerationOptions);
        }

        //ex: file= "C:\Users\user\source\repos\MyBlog\Content\Blog\en\somePost.md"
        //returns "en/somePost"  
        string GetRelativePathWithFilename(string file)
        {
            string relativePathWithFileName = Path.GetRelativePath(absContentPath, file);
            return Path.Combine(Path.GetDirectoryName(relativePathWithFileName)!, Path.GetFileNameWithoutExtension(relativePathWithFileName)).Replace("\\", "/");
        }
    }
}
