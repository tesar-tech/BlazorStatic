# <img id="imglogo" src="./BlazorStaticWebsite/wwwroot/imgs/logo.png" alt="blazor static">



[![Discord](https://img.shields.io/discord/798312431893348414?style=flat&logo=discord&logoColor=white&label=Blazor%20Community%2F%23BlazorStatic&labelColor=5865f2&color=gray)
](https://discord.gg/DsAXsMuEbx)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/BlazorStatic)](https://www.nuget.org/packages/BlazorStatic/)
[![Build, publish to gh pages anN nuget](https://github.com/tesar-tech/BlazorStatic/actions/workflows/publish-to-ghpages-and-nuget.yml/badge.svg)](https://github.com/tesar-tech/BlazorStatic/actions/workflows/publish-to-ghpages-and-nuget.yml)
[![Netlify Status](https://api.netlify.com/api/v1/badges/4fa2c17a-6385-4cc6-9919-e32c134175d9/deploy-status)](https://app.netlify.com/sites/blazorstatic/deploys)


Embrace the capabilities of Blazor on .NET 8 to craft static websites.

Transform your Blazor app into a static site generator.

## How does it work?

**BlazorStatic** generates static HTML files by running the app and fetching the pages' HTML with `HttpClient`.

It automatically discovers pages to generate by scanning for the `@page` directive in Razor files, but It only targets non-parametrized pages (e.g., `@page "/mypage"`, not `@page "/mypage/{param}"`).

### Features

- Easily parse and consume markdown files.
- Support for custom YAML front matters.
- Ability to add / remove pages as needed.
- Works with all CSS frameworks and themes.
- Easy to deploy with CI/CD pipeline.

## Getting started

BlazorStatic is a small library designed to be integrated into Blazor web applications That was mainly created with the intention of making blogs, but it got improved to become a more general tool for creating static sites.

*Note*: You can use [BlazorStaticMinimalBlog](https://github.com/tesar-tech/BlazorStaticMinimalBlog) as a template if you want something quick and easy.

### 1. Creating project

*skip this if you already have a blazor project*

#### Using Dotnet new template

Ideally, this command would set up your project automatically. However, this feature is currently a significant **[TODO](https://github.com/tesar-tech/BlazorStatic/issues/2)** awaiting implementation.

### 2. Installation

#### Nuget:

```shell
$ dotnet add package BlazorStatic
```

### 3. Registering Services

```cs
// Program.cs
using BlazorStatic;

// register blazor static service to generate static files
builder.Services.AddBlazorStaticService(opt => {
    // won't copy the `app.css` when generating the static files
    opt.IgnoredPathsOnContentCopy.AddRange(new[] { "app.css" });
    // choose the output folder for the static files
    opt.OutputFolderPath = "output";
});

// register a service for your blog files
builder.Services.AddBlazorStaticContentService<BlogFrontMatter>(opt => {
    // assign where the blog files will be stored
    opt.ContentPath = Path.Combine("Content", "Blog");
});

// ...

// `shutdownApp` is essential for CI/CD pipelines to prevent indefinite running.
app.UseBlazorStaticGenerator(shutdownApp: !app.Environment.IsDevelopment());
```

### 4. Running

Starting the project will create a folder (`output` by default) which contains the generated static files of your site. (this can be disabled in `AddBlazorStaticService`)

### More in Depth

In-depth instructions are available [here](./BlazorStaticWebsite/Content/Docs/new-start.md). These instructions will help you build a project akin to BlazorStaticMinimalBlog and is beneficial for understanding the inner workings of BlazorStatic.

## Samples

| Description                                                  | Source                                                                               | Live                                                          |
| ------------------------------------------------------------ | ------------------------------------------------------------------------------------ | ------------------------------------------------------------- |
| Page about BlazorStatic (this repo contains the code itself) | [source](https://github.com/tesar-tech/BlazorStatic/tree/master/BlazorStaticWebsite) | [live](https://tesar-tech.github.io/BlazorStatic/)            |
| Minimal blog                                                 | [source](https://github.com/tesar-tech/BlazorStaticMinimalBlog)                      | [live](https://tesar-tech.github.io/BlazorStaticMinimalBlog/) |
| Zodoc - image processing and deep learning sample            | [source](https://github.com/tesar-tech/zodoc/)                                       | [live](https://zodoc.tech/)                                   |
| ❓ Add your page here!!!                                      |                                                                                      |

## Contributions

Contributions are highly encouraged and appreciated. If you find something missing, unclear, or encounter an issue with the code, I warmly welcome your input. Feel free to:

- Create a new [issue](https://github.com/tesar-tech/BlazorStatic/issues) or submit a PR.
- Contact [me](https://github.com/tesar-tech/) directly for any queries or suggestions.
- Ask questions or start a discussion on the [Blazor Community Discord server](https://discord.gg/DsAXsMuEbx).
