namespace BlazorStatic;
public interface IFrontMatter
{
    string Title { get; set; }
    List<string> Tags { get; set; }
}

public class FrontMatter:IFrontMatter
{
    public string Title { get; set; } = "Empty title";
    public string Lead { get; set; } = "";
    public DateTime Published { get; set; } = DateTime.Now;
    public List<string> Tags { get; set; } = new();
    public List<Author> Authors { get; set; } = new();
}
public class Author
{
    public string? Name { get; set; }
    public string? GitHubUserName { get; set; }
    public string? TwitterUserName { get; set; }
}

public class Post<TFrontMatter>
    where TFrontMatter : class

{
    public required TFrontMatter FrontMatter { get; set; }
    public required string FileNameNoExtension { get; set; }
    public required string HtmlContent { get; set; }
}

public record ContentToCopy(string SourcePath, string TargetPath);


public record PageToGenerate(string Url, string OutputFile);
