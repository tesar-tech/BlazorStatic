using BlazorStatic;
using BlazorStaticWebsite;
using BlazorStaticWebsite.Components;
using Microsoft.Extensions.FileProviders;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseStaticWebAssets();

builder.Services.AddBlazorStaticService(opt => {
        opt.IgnoredPathsOnContentCopy.AddRange(new[] { "app.css" });//pre-build version for tailwind
        opt.ContentToCopyToOutput.Add(new("Content/Docs/media", "Content/Docs/media"));
        // add docs pages
        IEnumerable<string> docsFiles = Directory.GetFiles(Path.Combine("Content", "Docs"), "*.md")
            .Where(x => !x.EndsWith("README.md"));//ignore readme, it is handled in Pages/Docs.razor


        foreach (string? fileName in docsFiles.Select(Path.GetFileNameWithoutExtension))
        {
            opt.PagesToGenerate.Add(new($"/docs/{fileName}", Path.Combine("docs", $"{fileName}.html")));
        }
        // Must add a site url to generate the Sitemap!
        opt.ShouldGenerateSitemap = true;
        opt.SiteUrl = "https://tesar-tech.github.io/BlazorStatic";
    }
    )
    .AddBlazorStaticContentService<BlogFrontMatter>()
    .AddBlazorStaticContentService<ProjectFrontMatter>(opt => {
        opt.MediaFolderRelativeToContentPath = null;
        opt.ContentPath = Path.Combine("Content", "Projects");
        opt.AddTagPagesFromPosts = false;
        opt.PageUrl = WebsiteKeys.ProjectsUrl;
    });


// Add services to the container.
builder.Services.AddRazorComponents();
builder.Services.AddFluentUIComponents();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Content", "Docs", "media")),
    RequestPath = "/Content/Docs/media"
});


app.UseAntiforgery();

app.MapRazorComponents<App>();

app.UseBlazorStaticGenerator(shutdownApp: !app.Environment.IsDevelopment());

app.Run();


public static class WebsiteKeys
{
    public const string BlogPostStorageAddress = "https://github.com/tesar-tech/BlazorStatic/tree/master/BlazorStaticWebsite/Content/Blog/";

    public const string GitHubRepo = "https://github.com/tesar-tech/BlazorStatic/";

    public const string ProjectsUrl = "projects";

}
