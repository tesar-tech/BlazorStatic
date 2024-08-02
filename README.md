# <img id="imglogo" src="./BlazorStaticWebsite/wwwroot/imgs/logo.png" alt="blazor static">



[![Discord](https://img.shields.io/discord/798312431893348414?style=flat&logo=discord&logoColor=white&label=Blazor%20Community%2F%23BlazorStatic&labelColor=5865f2&color=gray)
](https://discord.gg/DsAXsMuEbx)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/BlazorStatic)](https://www.nuget.org/packages/BlazorStatic/)
[![Build, publish to gh pages anN nuget](https://github.com/tesar-tech/BlazorStatic/actions/workflows/publish-to-ghpages-and-nuget.yml/badge.svg)](https://github.com/tesar-tech/BlazorStatic/actions/workflows/publish-to-ghpages-and-nuget.yml)
[![Netlify Status](https://api.netlify.com/api/v1/badges/4fa2c17a-6385-4cc6-9919-e32c134175d9/deploy-status)](https://app.netlify.com/sites/blazorstatic/deploys)


Embrace the capabilities of Blazor on .NET 8 to craft static websites.

Transform your Blazor app into a static site generator.

# How does it work?

You have your SSR Blazor app that you can develop, run, and test. You add the **BlazorStatic** package and configure it to specify which URLs convert to HTML files.
Upon running the app, **BlazorStatic** fetches the pages' HTML with `HttpClient`, outputs HTML files and assets to the `output` folder, which you can deploy to any static hosting.

There are a lot of defaults to keep usage simple, but you can configure it extensively, for example, to match your YAML front matter in markdown files or ignore files in the output.

## Features

- Work with Blazor as you are used to.
- Easily parse and consume markdown files.
- Support for custom YAML front matters.
- Automatically discovers pages among Razor files.
- Ability to add/remove pages as needed.
- Works with all CSS frameworks and themes.
- Easy to deploy with CI/CD pipeline.

# Getting Started

You can fork [BlazorStaticMinimalBlog](https://github.com/tesar-tech/BlazorStaticMinimalBlog) as a template if you want something quick and easy to start with. It is ready to work with GitHub Pages, and you can have your static page in a matter of minutes.

If you want to start from scratch, follow the steps below.

## 1. Creating a Project (if you don't have one)

Create a new Blazor project with the following command:

```shell
dotnet new blazor --name MyBlazorStaticApp  --interactivity None --empty
```

## 2. Installation

Just add the `BlazorStatic` Nuget package to your project:

```shell
cd MyBlazorStaticApp
```

```shell
$ dotnet add package BlazorStatic --prerelease
```

## 3. Registering Services

3 lines of code in `Program.cs` and you are ready to go:

```cs
using BlazorStatic;

builder.Services.AddBlazorStaticService();
//...
app.UseBlazorStaticGenerator();
```

## 4. Running

Run the app as usual (for example, `dotnet run`), and you will see the static files generated in the `output` folder.

You can see similar output in the console:


```shell
Copying C:\PathToRepo\MyBlazorStaticApp\wwwroot\app.css to output\app.css
Generating into index.html
```

Inside the `output` folder, you will find the generated files. It includes the `index.html` file and `app.css`, which **BlazorStatic** copied from the `wwwroot` folder. You can deploy the `output` folder to any static hosting, and it will just work.

With this approach, you can add any page to your app, and it will be generated as a static HTML file. **BlazorStatic** isn't a framework or a way to build websites. It is just a tool to generate static HTML files from your existing Blazor app.


# Markdown Content

Now you want to add some content to your page. **BlazorStatic** has some tools to handle markdown files.

## 1. Create a Markdown File

- Create a `Content/Blog` folder in the root of your project. If you don't like the naming, you can change it in the configuration, but for now, let's stick with the default.
- Create a markdown file in the `Content/Blog` folder. For example, `Content/Blog/first-post.md` with the following content:


    ```markdown
    ---
    title: First Post
    lead: First post description
    published: 2024-07-07
    tags: [tag1, tag2]
    authors:
        - name: "Author name"
        
    ---

    ## I am heading

    Here is some **markdown** content. 

    ```
- Instruct `MyBlazorStaticApp.csproj` to copy the content to the bin folder:
    
    ```xml
    <ItemGroup>
        <Content Include="Content\**\*" CopyToOutputDirectory="PreserveNewest" />
    </ItemGroup>
    ```

## 2. Parsing markdown

- As you can see, it uses Front Matter in YAML format. Now we need to instruct `BlazorStatic` to add the content (parse markdown):


    ```cs
    builder.Services.AddBlazorStaticContentService<BlogFrontMatter>(opt => {
        opt.ContentPath = Path.Combine("Content", "Blog");
    });

    ```
    - `BlogFrontMatter` is a class that matches the front matter in the markdown file. You can create your own class, which implements `IFrontMatter`.
    - `ContentPath` is the path to the folder with markdown files. You can change it to any other path.

- Now when you run the app it will parse markdown files, but it won't generate any HTML files. You need to add a page that will use the parsed content.

## 3. Adding a Page to Use the Content

Now we need to use the content in a page. This is not something **BlazorStatic** is meant to do. It just makes the content available, and it's up to you how you use it.

It assumes you will have a "/blog" page available (you can also configure this).

A simple example of such a page is this:


```razor
@page "/blog"
@page "/blog/{fileName}"
@using BlazorStatic
@using BlazorStatic.Services
@inject BlazorStaticContentService<BlogFrontMatter> blogService


@if (post == null)
{
   <div>There is total of @blogService.BlogPosts.Count() blog posts </div>
}
else
{
   <div>
      @post.FrontMatter.Title
      @((MarkupString)post.HtmlContent)
   </div>
}

@code
{
   [Parameter] public string? FileName { get; set; }
   Post<BlogFrontMatter>? post;

   protected override void OnInitialized()
   {
      if (string.IsNullOrWhiteSpace(FileName)) return;
      post = blogService.BlogPosts.FirstOrDefault(x => x.Url == FileName);
   }
}

```


Now **BlazorStatic** will generate a `blog` folder with an `index.html` file where you will see the total number of posts and a `first-post.html` file where you will see the content of the post we created earlier.

Check the `More In Depth` section to see how to configure **BlazorStatic** to match your needs.

## Deploying


You can deploy the `output` folder to any static hosting service, such as GitHub Pages, Netlify, Vercel, Azure Static Web Apps, etc.

Detailed instructions are [here](./BlazorStaticWebsite/Content/Docs/deployment.md).

Don't forget to shut down the app after the files are generated; otherwise, the pipeline will hang. You can do it like this:


```cs
app.UseBlazorStaticGenerator(shutdownApp: !app.Environment.IsDevelopment());
```

# More in Depth

The best way to see how things work is to check the existing code. For example, this repo contains a feature-rich demonstration of `BlazorStatic` capabilities.

You can also see a [different guide](./BlazorStaticWebsite/Content/Docs/new-start.md) for starting. These instructions will help you build a project similar to BlazorStaticMinimalBlog and are beneficial for understanding the inner workings of **BlazorStatic**.

# Samples

| Description                                                  | Source                                                                               | Live                                                          |
| ------------------------------------------------------------ | ------------------------------------------------------------------------------------ | ------------------------------------------------------------- |
| Page about BlazorStatic (this repo contains the code itself) | [source](https://github.com/tesar-tech/BlazorStatic/tree/master/BlazorStaticWebsite) | [live](https://tesar-tech.github.io/BlazorStatic/)            |
| Minimal blog                                                 | [source](https://github.com/tesar-tech/BlazorStaticMinimalBlog)                      | [live](https://tesar-tech.github.io/BlazorStaticMinimalBlog/) |
| Zodoc - image processing and deep learning sample            | [source](https://github.com/tesar-tech/zodoc/)                                       | [live](https://zodoc.tech/)                                   |
| ❓ Add your page here!!!                                      |                                                                                      |

# Contributions

Contributions are highly encouraged and appreciated. If you find something missing, unclear, or encounter an issue with the code, I warmly welcome your input. Feel free to:

- Create a new [issue](https://github.com/tesar-tech/BlazorStatic/issues) or submit a PR.
- Contact [me](https://github.com/tesar-tech/) directly for any queries or suggestions.
- Ask questions or start a discussion on the [Blazor Community Discord server](https://discord.gg/DsAXsMuEbx).
