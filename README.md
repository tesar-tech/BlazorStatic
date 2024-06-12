# <img id="imglogo" src="./BlazorStaticWebsite/wwwroot/imgs/logo.png" alt="blazor static">



[![Discord](https://img.shields.io/discord/798312431893348414?style=flat&logo=discord&logoColor=white&label=Blazor%20Community%2F%23BlazorStatic&labelColor=5865f2&color=gray)
](https://discord.gg/DsAXsMuEbx)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/BlazorStatic)](https://www.nuget.org/packages/BlazorStatic/)
[![Build, publish to gh pages anN nuget](https://github.com/tesar-tech/BlazorStatic/actions/workflows/publish-to-ghpages-and-nuget.yml/badge.svg)](https://github.com/tesar-tech/BlazorStatic/actions/workflows/publish-to-ghpages-and-nuget.yml)
[![Netlify Status](https://api.netlify.com/api/v1/badges/4fa2c17a-6385-4cc6-9919-e32c134175d9/deploy-status)](https://app.netlify.com/sites/blazorstatic/deploys)


Transform your Blazor app into a static site generator.

Embrace the capabilities of Blazor on .NET 8 to craft static websites.

## How to start: 

BlazorStatic is a small library designed for integration into Blazor applications. Here are a few straightforward methods to get started:

### Fork or clone BlazorStaticMinimalBlog

**RECOMMENDED STARTING POINT**: [BlazorStaticMinimalBlog](https://github.com/tesar-tech/BlazorStaticMinimalBlog) offers a quick and convenient way to deploy your site within minutes.


### Build from Scratch

Begin by creating a Blazor application, then proceed to add content and integrate BlazorStatic. Comprehensive instructions are available [here](./BlazorStaticWebsite/Content/Docs/new-start.md). This approach will help you build a project akin to BlazorStaticMinimalBlog and is beneficial for understanding the inner workings of BlazorStatic.

### Using dotnet new blazorStatic

Ideally, this command would set up your project automatically. However, this feature is currently a significant **[TODO](https://github.com/tesar-tech/BlazorStatic/issues/2)** awaiting implementation.


## How it works? What it does?

BlazorStatic:

- Generates static HTML files by running the app and fetching page HTML with `HttpClient`  
  - Use the following in your app:: 
  ```csharp
  app.UseBlog<FrontMatter>(); //processes markdown files, adds blog and tags pages
  app.UseBlazorStaticGenerator(shutdownApp: !app.Environment.IsDevelopment());
  ```
  - `shutdownApp` is essential for CI/CD pipelines to prevent indefinite running.


- Automates the discovery of pages to generate by scanning for the `@page` directive in Razor files. It targets only non-parametrized pages (e.g., `@page "/mypage"`, not `@page "/mypage/{param}"`).

- Enables adding additional pages for generation using

  ```csharp
  builder.Services.AddBlazorStaticService(opt => {
    opt.PagesToGenerate.Add(new($"/mypage/paramValue", "paramValue.html"))
  }); 
  ```
  Example [here](https://github.com/tesar-tech/BlazorStatic/blob/master/BlazorStaticWebsite/Program.cs) for adding `docs` pages.   

- Simplifies blog post generation from markdown files, respecting a specified folder structure:
  ```csharp
  builder.Services.AddBlogService<FrontMatter>(opt => {
      opt.BlogPageUrl = "blog"; //default value
      opt.PostFilePattern = "*.md"; //default value
      opt.ContentPath = Path.Combine("Content", "Blog"); //default path
      opt.MediaFolderRelativeToContentPath = "media"; //default folder
  });
  ```

- Provides `FrontMatter` class for parsing blog post metadata.
- Allows for custom `IFrontMatter` implementations to suit various markdown (front matter) formats. You can even have multiple sections with multiple `IFrontMatter` classes.

- Facilitates copying necessary files to the output folder:

  ```csharp
  builder.Services.AddBlazorStaticService(opt => {
     opt.OutputFolderPath = "output";//root of the output 
     //wwwroot and _content are copied by default
     opt.IgnoredPathsOnContentCopy.AddRange(new[] { "app.css" }); //don't copy app.css
  }); 
  ```



- Offers flexibility in CSS frameworks and themes, without locking you into a specific choice. TailwindCSS is used in the default theme, but it's fully customizable. Open to suggestions and contributions for design improvements (do it, I am not the right person).

- Easy deployment: run your app in a CI/CD pipeline and deploy the generated files to platforms like GitHub Pages, Azure Static Web Apps, Netlify, etc. See [the pipline](https://github.com/tesar-tech/BlazorStaticMinimalBlog/blob/master/.github/workflows/publish-to-gh-pages.yml) for a minimal blog setup or check the [deployment guide](./BlazorStaticWebsite/Content/Docs/deployment.md) for more details

## Samples

| Description | Source | Live |
| --- | --- | --- |
|Page about BlazorStatic (this repo contains the code itself)|[source](https://github.com/tesar-tech/BlazorStatic/tree/master/BlazorStaticWebsite) | [live](https://tesar-tech.github.io/BlazorStatic/)|
| Minimal blog  |[source](https://github.com/tesar-tech/BlazorStaticMinimalBlog)|[live](https://tesar-tech.github.io/BlazorStaticMinimalBlog/)|
|Zodoc - image processing and deep learning sample| [source](https://github.com/tesar-tech/zodoc/)|[live](https://zodoc.tech/)|
|❓ Add your page here!!!||

## Contributions

Contributions are highly encouraged and appreciated. If you find something missing, unclear, or encounter an issue with the code, I warmly welcome your input. Feel free to:

- Create a new [issue](https://github.com/tesar-tech/BlazorStatic/issues) or submit a PR.
- Contact [me](https://github.com/tesar-tech/) directly for any queries or suggestions.
- Ask questions or start a discussion on the [Blazor Community Discord server](https://discord.gg/DsAXsMuEbx).


The main repo contains the MinimalBlog submodule, after you clone the repo, you can initialize the submodule with:

```shell
git submodule update --init --recursive
```



