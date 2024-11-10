// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.
namespace BlazorStatic.Blog;

using System.Net;
using Services;

public class BlogOptions<TBlogFrontMatter> : IBlazorStaticContentOptions<TBlogFrontMatter, BlogPost<TBlogFrontMatter>>
    where TBlogFrontMatter : class, IBlogFrontMatter, new()
{
    public BlogOptions()
    {
        AfterContentParsedAndAddedAction = blazorStaticService => {

            //add tags pages
            if(TagsPageUrl is null) return;
            AllTags = Posts.SelectMany(x => x.FrontMatter.Tags)
                .Distinct()
                .Select(x => new Tag { Name = x, EncodedName = TagEncodeFunc(x) })
                .ToDictionary(x => x.Name);

            foreach(var post in Posts)
                post.Tags = post.FrontMatter.Tags.Select(x => AllTags[x]).ToList();

            foreach(var tag in AllTags.Values)
            {
                blazorStaticService.Options.PagesToGenerate.Add(new PageToGenerate($"{TagsPageUrl}/{tag.EncodedName}",
                Path.Combine(TagsPageUrl, $"{tag.EncodedName}.html")));
            }




        };
    }


    /// <summary>
    ///     Should correspond to @page "/tags" (here in relative path: "tags")
    ///     Useful for avoiding magic strings in .razor files
    ///     If null, no tag pages will be generated
    /// </summary>
    public string? TagsPageUrl { get; set; } = "tags";

    public Dictionary<string, Tag> AllTags = [];

    /// <summary>
    /// Func to convert tag string to file-name/url.
    /// You might want to change this if you don't like non-alphanumerical chars in your url (like tags/.net%2FC%23)
    /// The default is WebUtility.UrlEncode, which makes changes, like:
    /// "nice tag" -> "nice+tag", "ci/cd" -> "ci%2Fcd", ".net/C# " -> ".net%2FC%23".
    /// Also don't forget to use the same encoder while creating tag links
    /// </summary>
    public Func<string, string> TagEncodeFunc { get; set; } = WebUtility.UrlEncode;


    /// <inheritdoc />
    public string ContentPath { get; set; } = Path.Combine("Content", "Blog");
    /// <inheritdoc />
    public string? MediaFolderRelativeToContentPath { get; set; } = "media";
    /// <inheritdoc />
    public string PostFilePattern { get; set; } = "*.md";

    /// <inheritdoc />
    public List<BlogPost<TBlogFrontMatter>> Posts { get; } = [];
    /// <inheritdoc />
    public string PageUrl { get; set; } = "blog";
    /// <inheritdoc />
    public Action<BlazorStaticService>? AfterContentParsedAndAddedAction { get; set; }

    public Func<TBlogFrontMatter, AdditionalInfo>? GetAdditionalInfoFromFrontMatter { get; set; } = fm => new AdditionalInfo{LastMod = fm.Published};
}
