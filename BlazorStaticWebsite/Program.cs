using BlazorStatic;
using BlazorStaticWebsite.Components;
using BlazorStaticWebsite;
using Microsoft.Extensions.FileProviders;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddBlazorStaticService(opt => {
    opt.IgnoredPathsOnContentCopy.AddRange(new[] { "app.css" });//pre-build version for tailwind
    opt.BeforeFilesGenerationAction = () => {
        //add docs pages
        var docsFiles = Directory.GetFiles(Path.Combine("Content", "Docs"), "*.md")
            .Where(x=>!x.EndsWith("README.md"));//ignore readme

        foreach (string? fileName in docsFiles.Select(Path.GetFileNameWithoutExtension))
        {
            opt.PagesToGenerate.Add(new($"/docs/{fileName}", Path.Combine("docs", $"{fileName}.html")));
        }
    };
}
).AddBlogService<FrontMatter>(opt => {
 
}
);


// Add services to the container.
builder.Services.AddRazorComponents();


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
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),"Content","Docs","media")),
    RequestPath = "/Content/Docs/media"
});


app.UseAntiforgery();

app.MapRazorComponents<App>();

app.UseBlog<FrontMatter>();
app.UseBlazorStaticGenerator(shutdownApp: !app.Environment.IsDevelopment());

app.Run();


public static  class WebsiteKeys
{
    public const string BlogPostStorageAddress = "https://github.com/tesar-tech/BlazorStatic/tree/master/BlazorStaticWebsite/Content/Blog/";

    public const string GitHubRepo  = "https://github.com/tesar-tech/blob/BlazorStatic/";
}

