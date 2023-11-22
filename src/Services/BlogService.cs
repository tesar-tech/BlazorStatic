namespace BlazorStatic.Services;

using Microsoft.Extensions.Logging;

public class BlogService<TFrontMatter>(BlogOptions<TFrontMatter> options,
    BlazorStaticHelpers helpers,
    BlazorStaticService blazorStaticService,
    ILogger<BlogService<TFrontMatter>> logger)
    where TFrontMatter : class, IFrontMatter, new()
{
    public List<Post<TFrontMatter>> BlogPosts => options.Posts;
    public BlogOptions<TFrontMatter> Options => options;

   public async Task ParseAndAddBlogPosts()
    {
        var files = Directory.GetFiles(options.ContentPath, options.PostFilePattern);

        foreach (string file in files)
        {
            var (htmlContent, frontMatter) = await helpers.ParseMarkdownFile<TFrontMatter>(file, 
        
        (options.MediaFolder,options.MediaRequestPath));
            Post<TFrontMatter> post = new()
            {
                FrontMatter = frontMatter,
                FileNameNoExtension = Path.GetFileNameWithoutExtension(file),
                HtmlContent = htmlContent
            };
            options.Posts.Add(post);

            blazorStaticService.Options.PagesToGenerate.Add(new($"{options.BlogPageUrl}/{post.FileNameNoExtension}", Path.Combine("blog", $"{post.FileNameNoExtension}.html")));
        }
        
        if (options.AddTagPagesFromPosts)
            foreach (var tag in options.Posts.SelectMany(x => x.FrontMatter.Tags).Distinct())//gather all unique tags from all blog posts
            {
                blazorStaticService.Options.PagesToGenerate.Add(new($"{options.TagsPageUrl}/{tag}", Path.Combine(options.TagsPageUrl, $"{tag}.html")));
            }
        options.AfterBlogParsedAndAddedAction?.Invoke();

    }
}



