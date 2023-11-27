namespace BlazorStatic;

using Markdig;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

/// <summary>
/// Options for configuring the BlazorStatic generation process.
/// </summary>
public class BlazorStaticOptions
{
    /// <summary>
    /// Output folder for generated files. Relative to the project root.
    /// </summary>
    public string OutputFolderPath { get; set; } = "output";
    /// <summary>
    /// Allows to suppress file generation. Useful for website building, when you don't need the static files.
    /// </summary>
    public bool SuppressFileGeneration { get; set; }
    
    /// <summary>
    /// List of pages to generate with url to call and path of .html file to generate.
    /// </summary>
    public List<PageToGenerate> PagesToGenerate { get; } = new();
    
    /// <summary>
    /// Allows to add non-parametrized razor pages to the list of pages to generate.
    /// </summary>
    public bool AddNonParametrizedRazorPages { get; set; } = true;
    
    /// <summary>
    /// Where to look for non-parametrized razor pages 
    /// Non-parametrized razor page: @page "/about"
    /// Parametrized razor page: @page "/docs/{slug}"
    /// Parametrized razor page should be handled in own way.
    /// (by calling AddExtraPages)
    /// </summary>
    public List<string> RazorPagesPaths { get;  } = new(){ Path.Combine("Components", "Pages")};
    /// <summary>
    /// Name of the page used for index. For example @page "/blog" will be generated to blog/index.html   
    /// </summary>
    public string IndexPageHtml { get; set; } = "index.html";
    /// <summary>
    /// Optional usage for adding additional pages, i.e. docs pages
    /// Is called in GenerateStaticPages, after all other pages are added
    /// </summary>
    public Action? BeforeFilesGenerationAction { get; set; }
    /// <summary>
    /// Paths (files or dirs) relative to new location of output folder, that shouldn't be copied to output folder
    /// Example: Need to ignore wwwroot/app.css (because "tailwindiezd" app.min.css is used)
    ///         Content to copy is "wwwroot" to -> "" (root of output folder)
    ///         IgnoredPathsOnContentCopy is "app.css" (this would be the path in output folder) 
    /// </summary>
    public List<string> IgnoredPathsOnContentCopy { get; } = new();
    /// <summary>
    /// Paths (files or dirs) relative to project root, that should be copied to output folder
    /// </summary>
    public List<ContentToCopy> ContentToCopyToOutput { get; } = new()
    {
        new ContentToCopy("wwwroot", "") // All from wwwroot will be placed in output dir
    };
    
    /// <summary>
    /// Allows to customize YamlDotNet Deserializer used for parsing front matter
    /// </summary>
    public IDeserializer FrontMatterDeserializer { get; set; } = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();
    
    /// <summary>
    /// Allows to customize Markdig MarkdownPipeline used for parsing markdown files
    /// </summary>
    public MarkdownPipeline MarkdownPipeline { get; set; } = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseYamlFrontMatter()
        .Build();
}

/// <summary>
/// Options for configuring processing of blog posts
/// </summary>
/// <typeparam name="TFrontMatter"></typeparam>
public class BlogOptions<TFrontMatter>
    where TFrontMatter : class, IFrontMatter
{
    /// <summary>
    /// folder relative to project root where blog posts are stored
    /// </summary>
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
    /// <summary>
    /// Place where processed blog posts live (their HTML and front matter)
    /// </summary>
    public List<Post<TFrontMatter>> Posts { get; } = new();
    /// <summary>
    /// tag pages will be generated from all tags found in blog posts
    /// </summary>
    public bool AddTagPagesFromPosts { get; set; } = true;
    
    /// <summary>
    /// Should correspond to @page "/blog" (here in relative path: "blog")
    /// Useful for avoiding magic strings in .razor files
    /// </summary>
    public string BlogPageUrl { get; set; } = "blog";
    /// <summary>
    /// Should correspond to @page "/tags" (here in relative path: "tags")
    /// Useful for avoiding magic strings in .razor files
    /// </summary>
    public string TagsPageUrl { get; set; } = "tags";
    
    /// <summary>
    /// Action to run after blog posts are parsed and added to the collection.
    /// Useful for editing data in blog posts. For example changing image paths.
    /// </summary>
    public Action? AfterBlogParsedAndAddedAction { get; set; }


   
}
