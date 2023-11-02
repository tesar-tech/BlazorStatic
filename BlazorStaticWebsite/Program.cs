using BlazorStatic;
using BlazorStaticWebsite.Components;
using BlazorStaticWebsite;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddBlazorStaticServices<FrontMatter>(opt => {
    opt.IgnoredPathsOnContentCopy.AddRange(new[] { "app.css" });//pre-build version for tailwind

    opt.AddExtraPages = () => {
        //add docs pages
        var docsFiles = Directory.GetFiles(Path.Combine("Content", "Docs"), "*.md").ToList();
        docsFiles.RemoveAll(x => x.EndsWith("README.md"));//readme is added in Docs.razor

        foreach (string? fileName in docsFiles.Select(Path.GetFileNameWithoutExtension))
        {
            opt.PagesToGenerate.Add(new($"/Docs/{fileName}", Path.Combine("Docs", $"{fileName}.html")));
        }
    };
});


// Add services to the container.
builder.Services.AddRazorComponents();

builder.Services.AddOptions<AppSettings>()
    .BindConfiguration(nameof(AppSettings))
    .ValidateDataAnnotations()
    .ValidateOnStart();

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
app.UseAntiforgery();

app.MapRazorComponents<App>();

app.UseBlazorStaticGenerator<FrontMatter>();
app.Run();
