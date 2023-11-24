

Start by creating new Blazor project. `Interactivity` is set to `None` since that is not something we need for static site. 

`dotnet new blazor --name BlazorStaticMinimalBlog --output BlazorStaticMinimalBlog --interactivity None --empty`


Remove `blazor.web.js` from `App.razor`

Make necessary changes to your layout. I am using tailwindcss, so everything is accomadated to that. 

When you run `dotnet watch` your app should be running. 

(if you also using tailwind, you need to run `tailwindcss -i .\wwwroot\app.css -o .\wwwroot\app.min.css -w` or npm command )



Now we can constract our static site.

For that we need some contentent in markdown. 
Add folder `Content` to the root of the project. Then add `Blog` folder to `Content`.
The directory structure can be customized (by BlazorStaticOptions), but for simplicity we will use the default one.

Add `first-post.md` to `Blog` folder. 

The post start with yaml front matter metadata. 

```yaml
---
title: First post
lead: Sample post so you can see how it works
published: 2023-11-04
tags: [tag-001, another-sample-tag]
authors:
    - name: "Jan Tesař"
      gitHubUserName: "tesar-tech"
      twitterUserName: "tesar-tech"
---
```

These metadata has to match C# class that we will use now.

I also added second post (`second-post.md` to `Blog` folder), to actually see some content.

Now we need to add `BlazorStatic` package to our project.

> dotnet add package BlazorStatic --prerelease

Now we add BlazorStaticService and BlogService to our `Program.cs`

```csharp
builder.Services.AddBlazorStaticService(opt => {
    opt.IgnoredPathsOnContentCopy.Add("app.css");//pre-build version for tailwind
}
).AddBlogService<FrontMatter>();
```

`FrontMatter` is the class that will be used to parse the metadata from markdown files. This is part of the BlazorStatic package. And it exactly matches the metadata in `first-post.md` and `second-post.md`. Nothing stops you from creating your own class that will match your own metadata. It just needs to implement interface `IFrontMatter`, which has just one property `List<string> Tags { get; set; }`.

BlazorStaticService has options you can change. For example you can change the directory structure (where your md files are located), or you can ignore some files (like `app.css` in this case (we have `app.min.css`)).

Before running the app we need to tell the service to acttually do something:

```csharp
app.UseBlog<FrontMatter>();
app.UseBlazorStaticGenerator(shutdownApp: !app.Environment.IsDevelopment());
```

UseBlog will parse the markdown files and expose them to the app as Post<FrontMatter> through `BlogService`.
UseBlazorStaticGenerator will generate the static files a put them into the `output` folder. It will shutdown the app outside of development environment. This is meant for CI/CD pipelines, otherwise the app would run "forever" in particualr job.

How is the file generation done? It will run the app, and use `HttpClient` to get the html of all the pages. Then it will save it to the file.
How does it know what pages to generate? 

- Blog posts in `Content/Blog` folder (and also tag pages)
- Non-parametrized razor pages (that have `@page` directive without a parameter in `{}`). For example `@page "/blog"`
- Pages you tell it to generate using `opt.PagesToGenerate.Add(new($"/docs/paramValue", "someFileName.html")`


Before we run the app, we need an UI to display the posts.
First is `Blog.razor` page with thse page directives:
  
  ```razor
@page "/blog" 
@page "/blog/{fileName}"
  ```
First will display the list of posts, second will display the post itself. The list of post is in `PostList.razor` component. 

Also add a page for tags











### dotnet new
- Create Blazor App with `dotnet new blazor -o MyNewWebsite`
- Add `BlazorStatic` package to your project
- Add:
 
  ```csharp
  //Program.cs
  builder.Services.AddBlazorStaticServices<FrontMatter>(); 
  //...
  //before app.Run():
  app.UseBlazorStaticGenerator<FrontMatter>();
  ```
- Add content with markdown
- Change configuration that will suits your needs and folder structure.
- Delete `blazor.js` 
- All the default config is described here 
