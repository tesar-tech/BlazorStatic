namespace BlazorStatic;
/// <summary>
/// Interface for front matter. Front matter is the metadata of a blog post.
/// </summary>
public interface IFrontMatter
{
    /// <summary>
    /// Tags for the blog post. When no tags are specified, implement empty list.
    /// If you have a different name for tags, or tags in complex objects, expose tags as a list of strings here.
    /// Useful for generating tag pages.
    /// </summary>
    List<string> Tags { get; set; }
    
    /// <summary>
    /// If true, the blog post will not be generated.
    /// </summary>
    bool IsDraft => false;
}


/// <summary>
/// Showcase of a front matter class. If you have a different front matter format, implement your own class.
/// </summary>
public class FrontMatter:IFrontMatter
{
    /// <summary>
    /// Title of the blog post.
    /// </summary>
    public string Title { get; set; } = "Empty title";
    /// <summary>
    /// Lead or description of the blog post.
    /// </summary>
    public string Lead { get; set; } = "";
    /// <summary>
    /// Date of publishing the blog post.
    /// </summary>
    public DateTime Published { get; set; } = DateTime.Now;
    /// <inheritdoc />
    public List<string> Tags { get; set; } = [];

    /// <inheritdoc />
    public bool IsDraft { get; set; }

    /// <summary>
    /// Authors of the blog post.
    /// </summary>
    public List<Author> Authors { get; set; } = [];
}
/// <summary>
/// Author of a blog post.
/// </summary>
public class Author
{
    /// <summary>
    /// Name of the author.
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// GitHub username of the author.
    /// </summary>
    public string? GitHubUserName { get; set; }
    /// <summary>
    /// X username of the author.
    /// </summary>
    public string? XUserName { get; set; }
}

/// <summary>
/// BlogPost keeps metadata and html content of a blog post (parsed from md).
/// </summary>
/// <typeparam name="TFrontMatter"></typeparam>
public class Post<TFrontMatter>
    where TFrontMatter : class

{
    /// <summary>
    /// Front matter of the blog post.
    /// </summary>
    public required TFrontMatter FrontMatter { get; set; }
    /// <summary>
    /// The url where the blog post will be generated.
    /// Processed from the file path (Content/Blog/subfolder/post-in-subfolder.md => blog/subfolder/post-in-subfolder).
    /// Used as url param "blog/{Url}".
    /// </summary>
    public required string Url { get; set; }
    /// <summary>
    /// HTML content of the blog post. Parsed from md. Without front matter part.
    /// </summary>
    public required string HtmlContent { get; set; }
}

/// <summary>
/// Class for keeping the content to copy properties together.
/// </summary>
/// <param name="SourcePath"></param>
/// <param name="TargetPath"></param>
public record ContentToCopy(string SourcePath, string TargetPath);


/// <summary>
/// Class for keeping the page to generate properties together.
/// </summary>
/// <param name="Url"></param>
/// <param name="OutputFile"></param>
public record PageToGenerate(string Url, string OutputFile);
