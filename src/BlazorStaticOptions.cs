namespace BlazorStatic;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class BlazorStaticOptions
{
    public string OutputFolderPath { get; set; } = "output";
    public bool SuppressFileGeneration { get; set; }
    
    /// <summary>
    /// List of pages to generate with url to call and path of .html file to generate.
    /// </summary>
    public List<PageToGenerate> PagesToGenerate { get; } = new();
    
    public bool AddNonParametrizedRazorPages { get; set; } = true;
    
    /// <summary>
    /// Where to look for non-parametrized razor pages 
    /// Non-parametrized razor page: @page "/about"
    /// Parametrized razor page: @page "/docs/{slug}"
    /// Parametrized razor page should be handled in own way.
    /// (by calling AddExtraPages)
    /// </summary>
    public List<string> RazorPagesPaths { get; internal set; } = new(){ Path.Combine("Components", "Pages")};
    public string IndexPageHtml { get; set; } = "index.html";
    /// <summary>
    /// Optional usage for adding additional pages, i.e. docs pages
    /// Is called in GenerateStaticPages, after all other pages are added
    /// </summary>
    public Action? BeforeFilesGenerationAction { get; set; }
    public List<string> IgnoredPathsOnContentCopy { get; } = new();
    public List<ContentToCopy> ContentToCopyToOutput { get; } = new()
    {
        new ContentToCopy("wwwroot", "") // All from wwwroot will be placed in output dir
    };
    
    public IDeserializer FrontMatterDeserializer { get; set; } = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();
}

public class BlogOptions<TFrontMatter>
    where TFrontMatter : class, IFrontMatter
{
    public string ContentPath { get; set; } = Path.Combine("Content", "Blog");
    /// <summary>
    /// folder in ContentPath where media files are stored.
    /// Important for app.UseStaticFiles targeting the correct folder
    /// </summary>
    public string MediaFolderRelativeToContentPath { get; set; } = "media";
    /// <summary>
    /// URL path for media files for blog posts.
    /// Used in app.UseStaticFiles to target the correct folder
    /// and in ParseBlogPosts to generate correct URLs for images
    /// changes ![alt](media/image.png) to ![alt](Content/Blog/media/image.png
    /// leading slash / is necessary for RequestPath in app.UseStaticFiles,
    /// is removed in ParseBlogPosts
    /// </summary>
    public string MediaRequestPath => Path.Combine(ContentPath, MediaFolderRelativeToContentPath).Replace(@"\", "/");
    /// <summary>
    /// pattern for blog post files in ContentPath
    /// </summary>
    public string PostFilePattern { get; set; } = "*.md";
    public List<Post<TFrontMatter>> Posts { get; } = new();
    /// <summary>
    /// tag pages will be generated from all tags found in blog posts
    /// </summary>
    public bool AddTagPagesFromPosts { get; set; } = true;
    
    public string BlogPageUrl { get; set; } = "blog";
    public string TagsPageUrl { get; set; } = "tags";
    
    public Action? AfterBlogParsedAndAddedAction { get; set; }


   
}
