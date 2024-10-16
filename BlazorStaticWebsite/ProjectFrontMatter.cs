using BlazorStatic;

namespace BlazorStaticWebsite;

public class ProjectFrontMatter : IFrontMatter
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public List<string> Tags { get; set; } = [];
}
