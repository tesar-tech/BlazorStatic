# Using BlazorStatic from scratch

This guide will show you how to create static site using BlazorStatic. It is good for understanding how it works, but you can also use the [BlazorStaticMinimalBlog](https://github.com/tesar-tech/BlazorStaticMinimalBlog) where everything is already set up for you.

## Create Blazor app

- Start by creating new Blazor project. `Interactivity` is set to `None` since that is not something we need for static site. 

  > `dotnet new blazor --name BlazorStaticMinimalBlog --output BlazorStaticMinimalBlog --interactivity None --empty`


- Remove `blazor.web.js` from `App.razor` (we don't need it for static site).

- Make necessary changes to your layout. I am using tailwindcss, so everything is accommodated to that. 

- When you run `dotnet watch` your app should be running. 

  (if you are also using tailwind, you need to run `tailwindcss -i .\wwwroot\app.css -o .\wwwroot\app.min.css -w` or npm command )


## Add content

- Add you markdown files following this structure:

  ```
  ├───Content
      └───Blog
          │   first-post.md
          │   second-post.md
          └───media
                  programming_bug.jpg
  ```
  The directory structure can be customized (by BlazorStaticOptions), but for simplicity we will use the default one.

- The post start with yaml front matter metadata. 

  ```yaml
  ---
  title: First post
  lead: Sample post so you can see how it works
  published: 2023-11-04
  tags: [tag-001, another-sample-tag]
  authors:
      - name: "Jan Tesař"
        gitHubUserName: "tesar-tech"
        xUserName: "tesar_tech"
  ---
  ```

  These metadata has to match C# class (`BlazorStatic.FrontMatter`) that we will use now. You can customize the class to match your own metadata.

- Mark Content folder to copy to the output:
  
    ```xml
    <ItemGroup>
    <None Update="Content/**/*">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
   </ItemGroup>
    ```
  
    > You can also use `dotnet publish` to copy the content folder to the output.


## BlazorStatic nuget

> dotnet add package BlazorStatic --prerelease

- Turn on `StaticWebAssets`. This will ensure wwwroot and RLCs assets are copied to the output folder. [More info](docs/release-1.0.0-beta.4)
  ```csharp
  builder.WebHost.UseStaticWebAssets();
  ```
- Register services in `Program.cs`

  ```csharp
  builder.Services.AddBlazorStaticService(opt => {
      opt.IgnoredPathsOnContentCopy.Add("app.css");//pre-build version for tailwind
  }
  ).AddBlogService<FrontMatter>();
  ```

  `FrontMatter` is the class that will be used to parse the metadata from markdown files. This is part of the BlazorStatic package. It exactly matches the metadata in `first-post.md` and `second-post.md`. Nothing stops you from creating your own class that will match your own metadata. It just needs to implement interface `IFrontMatter`, which has just one property `List<string> Tags { get; set; }`.

  As you can see `BlazorStaticService` has options you can change. For example you can change the directory structure (where your md files are located), or you can ignore some files (like `app.css` in this case (we have `app.min.css`)).

- Use `BlazorStatic`
  Before running the app we need to tell the service to actually do something:

  ```csharp
  app.UseBlog<FrontMatter>();
  app.UseBlazorStaticGenerator(shutdownApp: !app.Environment.IsDevelopment());
  ```

  `UseBlog` will parse the markdown files and expose them to the app as Post<FrontMatter> through `BlogService`.

  `UseBlazorStaticGenerator` will generate the static files a put them into the `output` folder (also customizable). It will shutdown the app outside of development environment. This is meant for CI/CD pipelines, otherwise the app would run "forever" in particular job.

## Scaffold the UI for blog posts

When you run the app right now, it will output the non-parametrized pages (e.g. `@page "/"` into `index.html`) and will copy content of `wwwroot` into the `output`. You can set where to look for non-parametrized pages by `opt.RazorPagesPaths` (default is `Components/Pages` folder).

> File generation is done by using `HttpClient` and saving the result into `.html` file. 

We need to scaffold the UI for blog posts. BlazorStatic doesn't force you to use any particular UI, but it will help you by providing `BlogService<FrontMatter>.Posts` collection where your processed posts live.

You get get some inspiration for th UI. These pages are important:

- [Blog.razor](https://github.com/tesar-tech/BlazorStaticMinimalBlog/blob/master/Components/Pages/Blog.razor) page which either displays the list of posts or the post itself. It starts with these page directives:

  ```razor
  @page "/blog" 
  @page "/blog/{fileName}"
  ```

  First will display the list of posts, second will display the post itself. The list of post is in `PostList.razor` component.
- [PostsList.razor](https://github.com/tesar-tech/BlazorStaticMinimalBlog/blob/master/Components/PostsList.razor) component, which is used in Blog.razor and Tags.razor pages to display the list of posts.
- [Tags.razor](https://github.com/tesar-tech/BlazorStaticMinimalBlog/blob/master/Components/Pages/Tags.razor) page, which either displays the tags cloud or the list of posts with particular tag. It have these page directives:

  ```razor
  @page "/tags" 
  @page "/tags/{tagName}"
  ```

For posts and tags generation, the directives must match the options, default value are:

  ```csharp
  builder.Services.AddBlogService<FrontMatter>(opt => {
      opt.BlogPageUrl = "blog";
      opt.TagsPageUrl = "tags";
  });
  ```

## Run and see

Now, when you run the app it outputs the static website into the `output` folder. When you open them the links and images will not work. But it is fine. For debugging purposes you just run the app and don't care about the output. You can even suppress the output by `opt.SuppressFileGeneration = true;` (default is `false`) and turn it on only for CI/CD pipeline.
