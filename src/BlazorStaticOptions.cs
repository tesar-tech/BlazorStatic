﻿using Markdig;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BlazorStatic;

using Services;

/// <summary>
///     Options for configuring the BlazorStatic generation process.
/// </summary>
public class BlazorStaticOptions
{
    private readonly List<Func<Task>> _beforeFilesGenerationActions = [];
    /// <summary>
    ///     Output folder for generated files. Relative to the project root.
    /// </summary>
    public string OutputFolderPath { get; set; } = "output";
    /// <summary>
    ///     Allows to suppress file generation. Useful for website building, when you don't need the static files.
    /// </summary>
    public bool SuppressFileGeneration { get; set; }

    /// <summary>
    ///     List of routes to generate as static html files.
    /// </summary>
    public List<PageToGenerate> PagesToGenerate { get; } = [];

    /// <summary>
    ///     Allows to add non-parametrized razor pages to the list of pages to generate.
    ///     Non-parametrized razor page: @page "/about"
    ///     Parametrized razor page: @page "/docs/{slug}"
    /// </summary>
    public bool AddPagesWithoutParameters { get; set; } = true;

    /// <summary>
    ///     Name of the page used for index. For example @page "/blog" will be generated to blog/index.html
    /// </summary>
    public string IndexPageHtml { get; set; } = "index.html";

    /// <summary>
    ///     If set to true will generate a `sitemap.xml` file and place it in the root of the output folder.<br />
    ///     The sitemap file follows the Google model:<br />
    ///     https://developers.google.com/search/docs/crawling-indexing/sitemaps/build-sitemap#xml
    /// </summary>
    public bool ShouldGenerateSitemap { get; set; }

    /// <summary>
    ///     Hostname of your site. Needed to generate the sitemap. <br />
    ///     E.g. 'https://username.github.io'
    /// </summary>
    public string? SiteUrl { get; set; }

    /// <summary>
    ///     Specifies the output folder for the sitemap.xml file, relative to the project root.
    ///     This folder should be within a static web assets directory (e.g., wwwroot or any
    ///     folder configured with <c>app.UseStaticFiles</c>), ensuring it is available for the running web application.
    ///     Default value: <c>"wwwroot"</c>.
    /// </summary>
    public string SitemapOutputFolderPath { get; set; } = "wwwroot";


    /// <summary>
    ///     Paths (files or dirs) relative to new location of output folder, that shouldn't be copied to output folder
    ///     Example: Need to ignore wwwroot/app.css (because "tailwindiezd" app.min.css is used)
    ///     Content to copy is "wwwroot" to -> "" (root of output folder)
    ///     IgnoredPathsOnContentCopy is "app.css" (this would be the path in output folder)
    /// </summary>
    public List<string> IgnoredPathsOnContentCopy { get; } = [];
    /// <summary>
    ///     Paths (files or dirs) relative to project root, that should be copied to output folder.
    ///     Content from RCLs (from _content/) and wwwroot is copied by default
    /// </summary>
    public List<ContentToCopy> ContentToCopyToOutput { get; } = [];

    /// <summary>
    ///     Allows to customize YamlDotNet Deserializer used for parsing front matter
    /// </summary>
    public IDeserializer FrontMatterDeserializer { get; set; } = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    /// <summary>
    ///     Allows to customize Markdig MarkdownPipeline used for parsing markdown files
    /// </summary>
    public MarkdownPipeline MarkdownPipeline { get; set; } = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseYamlFrontMatter()
        .Build();


    /// <summary>
    ///     Hooks up to the hot reload event to re-generate the outputted pages. It also re-evaluates the options set up in
    ///     Program.cs.
    ///     <para>Works with:</para>
    ///     - Changes in Razor files, C# code, CSS, etc.
    ///     - Changes to .md files, if you set up the watch for them in .csproj, for example:
    ///     <para>&#160;</para>
    ///     &lt;ItemGroup&gt;
    ///     &lt;Watch Include="Content/**/*" /&gt;
    ///     &lt;/ItemGroup&gt;
    ///     <para>&#160;</para>
    ///     Note: Hot reload re-generation will clear the list of PagesToGenerate and ContentToCopyToOutput,
    ///     but these lists will be re-populated through the options.
    /// </summary>
    public bool HotReloadEnabled { get; set; }


    /// <summary>
    ///     Iterator for optional actions.
    ///     Is called in GenerateStaticPages, after all other pages are added
    /// </summary>
    internal IEnumerable<Func<Task>> GetBeforeFilesGenerationActions() => _beforeFilesGenerationActions;


    /// <summary>
    ///     Adds an async Func to the list of actions to be executed before files are generated.
    /// </summary>
    /// <param name="func">The asynchronous function to add.</param>
    public void AddBeforeFilesGenerationAction(Func<Task> func) => _beforeFilesGenerationActions.Add(func);

    /// <summary>
    ///     Adds an action to the list of actions to be executed before files are generated.
    /// </summary>
    /// <param name="action">The synchronous action to add.</param>
    public void AddBeforeFilesGenerationAction(Action action) =>
        AddBeforeFilesGenerationAction(() => {
            action();
            return Task.CompletedTask;
        });


    /// <summary>
    ///     Clears the list. Useful for hot-reload, since we don't want the actions to be there multiple times
    /// </summary>
    internal void ClearBeforeFilesGenerationActions()
    {
        _beforeFilesGenerationActions.Clear();
    }
}



/// <summary>
/// Options for configuring processing of md files with front matter. Uses Post class
/// </summary>
/// <typeparam name="TFrontMatter">Any front matter type that inherits from IFrontMatter </typeparam>
public interface IBlazorStaticContentOptions<TFrontMatter>: IBlazorStaticContentOptions<TFrontMatter,Post<TFrontMatter>>
    where TFrontMatter : class, IFrontMatter, new()
{

}

/// <summary>
///     Options for configuring processing of md files with front matter.
///     Default values are set to work with posts (<see cref="ContentPath" /> and <see cref="PageUrl" /> ) .
/// </summary>
/// <typeparam name="TFrontMatter"></typeparam>
/// <typeparam name="TPost"></typeparam>
public interface IBlazorStaticContentOptions<TFrontMatter,TPost>
    where TFrontMatter : class, IFrontMatter
    where TPost : IPost<TFrontMatter>

{
    /// <summary>
    /// Folder relative to project root where posts are stored.
    /// Don't forget to copy the content to bin folder (use CopyToOutputDirectory in .csproj),
    /// because that's where the app will look for the files.
    /// Default is Content/Blog where posts are stored.
    /// </summary>
    string ContentPath { get; set; }

    /// <summary>
    /// Folder in ContentPath where media files are stored.
    /// Important for app.UseStaticFiles targeting the correct folder.
    /// Null in case of no media folder.
    /// </summary>
    string? MediaFolderRelativeToContentPath { get; set; }


    /// <summary>
    /// URL path for media files for posts.
    /// Used in app.UseStaticFiles to target the correct folder
    /// and in ParseAndAddPosts to generate correct URLs for images.
    /// Changes ![alt](media/image.png) to ![alt](Content/Blog/media/image.png).
    /// Leading slash / is necessary for RequestPath in app.UseStaticFiles,
    /// and is removed in ParseAndAddPosts. Null in case of no media.
    /// </summary>
    string? MediaRequestPath  => MediaFolderRelativeToContentPath is null
        ? null
        : Path.Combine(ContentPath, MediaFolderRelativeToContentPath).Replace(@"\", "/");

    /// <summary>
    /// Pattern for blog post files in ContentPath.
    /// </summary>
    string PostFilePattern { get; set; }

    /// <summary>
    /// Place where processed blog posts live (their HTML and front matter).
    /// </summary>
    List<TPost> Posts { get; }

    /// <summary>
    /// Should correspond to page that keeps the list of content.
    /// For example: @page "/blog" -> PageUrl="blog".
    /// This also serves as a generated folder name for the content.
    /// Useful for avoiding magic strings in .razor files.
    /// Default is "blog".
    /// </summary>
    string PageUrl { get; set; }

    /// <summary>
    /// Action to run after content is parsed and added to the collection.
    /// Useful for editing data in the posts, such as changing image paths.
    /// </summary>
    Action<BlazorStaticService>? AfterContentParsedAndAddedAction { get; set; }

    /// <summary>
    /// Validates the configuration properties to ensure required fields are set correctly.
    /// This validation is run when registering the service.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <see cref="ContentPath"/> or <see cref="PageUrl"/> are null or empty.
    /// </exception>
    void CheckOptions()
    {
        if (string.IsNullOrWhiteSpace(ContentPath))
            throw new InvalidOperationException("ContentPath must be set and cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(PageUrl))
            throw new InvalidOperationException("PageUrl must be set and cannot be null or empty.");
    }

    Func<TFrontMatter, AdditionalInfo>? GetAdditionalInfoFromFrontMatter => null;

}

public class BlazorStaticContentOptions<TFrontMatter> : BlazorStaticContentOptions<TFrontMatter, Post<TFrontMatter>>
    where TFrontMatter : class, IFrontMatter, new();

/// <inheritdoc />
public class BlazorStaticContentOptions<TFrontMatter, TPost> : IBlazorStaticContentOptions<TFrontMatter, TPost>
    where TFrontMatter : class, IFrontMatter
where TPost : IPost<TFrontMatter>
{
    /// <inheritdoc />
    public string ContentPath { get; set; } = null!;//is checked in "check opiton"

    /// <inheritdoc />
    public string? MediaFolderRelativeToContentPath { get; set; }

    /// <inheritdoc />
    public string? MediaRequestPath => MediaFolderRelativeToContentPath is null
        ? null
        : Path.Combine(ContentPath, MediaFolderRelativeToContentPath).Replace(@"\", "/");

    /// <inheritdoc />
    public string PostFilePattern { get; set; } = "*.md";

    /// <inheritdoc />
    public List<TPost> Posts { get; } = new();

    /// <inheritdoc />
    public string PageUrl { get; set; } = null!;//is checked in check options

    /// <inheritdoc />
    public Action<BlazorStaticService>? AfterContentParsedAndAddedAction { get; set; }
    public Func<TFrontMatter, AdditionalInfo>? GetAdditionalInfoFromFrontMatter { get; set; }
}


