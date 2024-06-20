---
title: 1.0.0-beta.9 - Refactoring and simplifications 
lead: BlogService was renamed to BlazorStaticContentService and more.
published: 2024-06-20
tags: [release]
authors:
- name: "Jan Tesař"
gitHubUserName: "tesar-tech"
xUserName: "tesar_tech"
---



## Breaking Changes

- `BlogService` was renamed to `BlazorStaticContentService` as it serves more general purpose now.
- `BlogService` was renamed to `AddBlazorStaticContentService`. Use this in your `Program.cs`
- `BlogOption` was also renamed to `BlazorStaticContentOptions`.
- `FrontMatter` was renamed to `BlogFrontMatter` as it is directly related to blog posts. `IFrontMatter` is still there and can be used for any front matter of your choice.
- `UseBlog<TFrontMatter>` extension was made private and is handled in `UseBlazorStaticGenerator`. This makes your code 1 line shorter (remove `UseBlog` from your `Program.cs`) 

## More on these changes:


Before:

```csharp
builder.Services
    .AddBlogService<FrontMatter>()
    .AddBlogService<ProjectFrontMatter>(opt => {
        //...
    });
 //..
app.UseBlog<FrontMatter>();
app.UseBlog<ProjectFrontMatter>();
app.UseBlazorStaticGenerator(shutdownApp: !app.Environment.IsDevelopment());   
```

Now:

```csharp
builder.Services
    .AddBlazorStaticContentService<BlogFrontMatter>()
    .AddBlazorStaticContentService<ProjectFrontMatter>(opt => {
        //...
    });
//..
app.UseBlazorStaticGenerator(shutdownApp: !app.Environment.IsDevelopment());   
```


