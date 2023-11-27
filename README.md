# Blazor Static


 [![](https://dcbadge.vercel.app/api/server/DsAXsMuEbx?style=flat)](https://discord.gg/DsAXsMuEbx)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/BlazorStatic)](https://www.nuget.org/packages/BlazorStatic/)


Transform your Blazor app into a static site generator.

  <img id="imglogo" src="BlazorStaticWebsite/wwwroot/imgs/logo.png" alt="blazor static logo">

Embrace the capabilities of Blazor on .NET 8 to craft static websites.

## How to start: 

BlazorStatic is a small library designed for integration into Blazor applications. Here are a few straightforward methods to get started:

### Fork or clone BlazorStaticMinimalBlog

[BlazorStaticMinimalBlog](https://github.com/tesar-tech/BlazorStaticMinimalBlog) offers a quick and convenient way to deploy your site within minutes. It includes a GitHub Actions workflow for publishing, allowing you to fork the repository, enable GitHub Pages, and launch your BlazorStatic website effortlessly. From there, you can personalize the content, modify the styling, adjust configurations, and more.

### Using dotnet new blazorStatic

Ideally, this command would set up your project automatically. However, this feature is currently a significant **[TODO](https://github.com/tesar-tech/BlazorStatic/issues/2)** awaiting implementation.

### Build from Scratch

Begin by creating a Blazor application, then proceed to add content and integrate BlazorStatic. Comprehensive instructions are available [here](BlazorStatitWebsite/Content/Docs/deployment.md/new-start). This approach will help you build a project akin to BlazorStaticMinimalBlog and is beneficial for understanding the inner workings of BlazorStatic, although it's not strictly necessary.


## How it works? What it does?

BlazorStatic:
- Automates the discovery of pages to generate by scanning for the `@page` directive in Razor files.
  - It targets only non-parametrized pages (e.g., `@page "/mypage"`, not `@page "/mypage/{param}"`).
  - Specify paths with `opt.RazorPagesPaths` (default is `Components/Pages` folder).
  - More information [here](https://github.com/tesar-tech/BlazorStatic/blob/master/BlazorStaticWebsite/Program.cs).

- Enables adding additional pages for generation using `opt.PagesToGenerate.Add(new($"/mypage/paramValue", "paramValue.html"))`. Example [here](https://github.com/tesar-tech/BlazorStatic/blob/master/BlazorStaticWebsite/Program.cs) for adding `docs` pages. This is also integrated for `Blog` and `Tags` pages.
  

- Simplifies blog post generation from markdown files, respecting a specified folder structure:
  ```csharp
  builder.Services.AddBlogService<FrontMatter>(opt => {
      // Defaults, customizable
      opt.ContentPath = Path.Combine("Content", "Blog");
      opt.BlogPageUrl = "/blog";
      opt.PostFilePattern = "*.md";
      opt.MediaFolderRelativeToContentPath = "media";
  });
  ```

- Provides `FrontMatter` class for parsing blog post metadata. Example metadata format:

  ```yml
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

- Allows for custom `IFrontMatter` implementations to suit various markdown (front matter) formats.

- Facilitates copying necessary files to the output folder (e.g. `wwwroot` folder (this is the default, you can add it or change by: `ContentToCopyToOutput.Add(new("MyFolder","SomeFolder/MyFolder"))` ) ). You can specify files to ignore by `opt.IgnoredPathsOnContentCopy.Add("app.css")`.

-Outputs generated files to a designated folder (`output` as default), customizable via `opt.OutputFolderPath = "myOutputFolder"`.

- Generates static HTML files by running the app and fetching page HTML with `HttpClient`  
  - Use the following in your app:: 
  ```csharp
  app.UseBlog<FrontMatter>();
  app.UseBlazorStaticGenerator(shutdownApp: !app.Environment.IsDevelopment());
  ```
  - `UseBlog` processes markdown files, adds tag pages, etc.
  - `shutdownApp` is essential for CI/CD pipelines to prevent indefinite running.

- Offers flexibility in CSS frameworks and themes, without locking you into a specific choice. TailwindCSS is used in the default theme, but it's fully customizable. Open to suggestions and contributions for design improvements (do it, I am not the right person).

- Easy deployment: run your app in a CI/CD pipeline and deploy the generated files to platforms like GitHub Pages, Azure Static Web Apps, Netlify, etc. See [the pipline](https://github.com/tesar-tech/BlazorStaticMinimalBlog/blob/master/.github/workflows/publish-to-gh-pages.yml) for a minimal blog setup or check the [deployement guid](BlazorStatitWebsite/Content/Docs/deployment.md) for more details

## Samples

| Description | Source | Live |
| --- | --- | --- |
|Page about BlazorStatic (this repo contains the code itself)|[source](https://github.com/tesar-tech/BlazorStatic/tree/master/BlazorStaticWebsite) | [live](https://tesar-tech.github.io/BlazorStatic/)|
| Minimal blog  |[source](https://github.com/tesar-tech/BlazorStaticMinimalBlog)|[live](https://tesar-tech.github.io/BlazorStaticMinimalBlog/)|
|Zodoc - image processing and deep learning sample| [source](https://github.com/tesar-tech/zodoc/)|[live](https://zodoc.tech/)|
|✅ Add your page here!!!||

## Contributions

Contributions are highly encouraged and appreciated. If you find something missing, unclear, or encounter an issue with the code, I warmly welcome your input. Feel free to:

- Create a new [issue](https://github.com/tesar-tech/BlazorStatic/issues) or submit a PR.
- Contact [me](https://github.com/tesar-tech/) directly for any queries or suggestions.
- Ask questions or start a discussion on the [Blazor Community Discord server](https://discord.gg/DsAXsMuEbx).



