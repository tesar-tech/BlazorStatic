namespace BlazorStaticWebsite;

using BlazorStatic;

public class ProjectFrontMatter:IFrontMatter
{
    public List<string> Tags { get; set; } = [];
    public string Name  { get; set; } = "";
    public string Description { get; set; } = "";
}

