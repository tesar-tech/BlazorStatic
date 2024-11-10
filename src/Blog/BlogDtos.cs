// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.
namespace BlazorStatic.Blog;

public class BlogPost<TBlogFrontMatter>:IPost<TBlogFrontMatter>, IPostWithTags
    where TBlogFrontMatter : class, IBlogFrontMatter, new()
{
    public TBlogFrontMatter FrontMatter { get; set; } = new();
    public string Url { get; set; } = "";
    public string HtmlContent { get; set; } = "";
    public List<Tag> Tags { get; set; } = [];
    // public void InitTags()
    // {
    //     var distinctTags = FrontMatter.Tags.Distinct();
    //     foreach (var tag in distinctTags)
    //         Tags.Add(new
    //         {
    //             Name = tag,
    //             EncodedName =
    //         });
    // }
}



/// <summary>
/// For blog front matter that has Tags
/// </summary>
public interface IBlogFrontMatter : IFrontMatter
{
    /// <summary>
    ///     Tags for the blog post. When no tags are specified, implement empty list.
    ///     If you have a different name for tags, or tags in complex objects, expose tags as a list of strings here.
    ///     Useful for generating tag pages.
    /// </summary>
    List<string> Tags { get; set; }

    DateTime Published { get; set; }
}


/// <summary>
///     Showcase of a IBlogFrontMatter implementation. If you have a different front matter format, implement your own class.
/// </summary>
public class BlogFrontMatter : IBlogFrontMatter
{
    /// <summary>
    ///     Title of the blog post.
    /// </summary>
    public string Title { get; set; } = "Empty title";
    /// <summary>
    ///     Lead or description of the blog post.
    /// </summary>
    public string Lead { get; set; } = "";
    /// <summary>
    ///     Date of publishing the blog post.
    /// </summary>
    public DateTime Published { get; set; } = DateTime.Now;

    /// <summary>
    ///     Authors of the blog post.
    /// </summary>
    public List<Author> Authors { get; set; } = [];

    /// <inheritdoc />
    public bool IsDraft { get; set; }




    /// <inheritdoc />
    public List<string> Tags { get; set; } = [];
}


/// <summary>
///     Author of a blog post.
/// </summary>
public class Author
{
    /// <summary>
    ///     Name of the author.
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    ///     GitHub username of the author.
    /// </summary>
    public string? GitHubUserName { get; set; }
    /// <summary>
    ///     X username of the author.
    /// </summary>
    public string? XUserName { get; set; }
}




