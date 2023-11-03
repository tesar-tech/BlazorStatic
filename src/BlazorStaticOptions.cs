namespace BlazorStatic;

public class BlazorStaticOptions<TFrontMatter>
    where TFrontMatter :class, IFrontMatter
{
    public string OutputFolderPath { get; set; } = "output";

    public bool SuppressFileGeneration { get; set; } 
    public List<PageToGenerate> PagesToGenerate { get; } = new();


    public bool ParseBlogPosts { get; set; } = true;
    public bool AddBlogPosts { get; set; } = true;
    public string BlogContentPath { get; set; } =  Path.Combine("Content","Blog");
    public string BlogPostFilePattern { get; set; } = "*.md";
    public List<Post<TFrontMatter>> BlogPosts { get;  } = new();
    
    public bool AddNonParametrizedRazorPages { get; set; } = true;
    public string RazorPagesPath { get; set; } = Path.Combine("Components","Pages");
    
    public string IndexPageHtml { get; set; } = "index.html";
    public bool AddTagPagesFromBlogPosts { get; set; } = true;
    public Action? AddExtraPages { get; set; } 
    
    public List<string> IgnoredPathsOnContentCopy { get; } = new();
    public List<ContentToCopy> ContentToCopyToOutput { get;  } = new()
    {
        new ("wwwroot","")//all from wwwroot will be placed in output dir
    };
    
    
}
