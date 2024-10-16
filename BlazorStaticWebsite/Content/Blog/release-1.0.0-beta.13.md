---
title: 1.0.0-beta.13 - HotReload and `dotnet new` template
lead: Create your new blog in just 2 commands with the dotnet new template. HotReload is now enabled by default. Includes various fixes.
published: 2024-10-08
tags: [release, HotReload, dotnet new template]
authors:
  - name: "Jan Tesař"
    gitHubUserName: "tesar-tech"
    xUserName: "tesar_tech"
---

## Breaking Changes

- HotReload is now enabled by default. You can disable it with `opt.HotReloadEnabled = false;`.
- Renamed some public methods and properties:
    - `ParseAndAddBlogPosts` -> `ParseAndAddPosts`
    - `AfterBlogParsedAndAddedAction` -> `AfterContentParsedAndAddedAction`

  Using the old methods will trigger warnings, but they still function by calling the new methods. These deprecated
  methods will be removed in an upcoming version.

## HotReload

The previous version introduced HotReload, enhancing the development experience when building the site locally. With
HotReload, every time you update your source code or .md file, BlazorStatic automatically refreshes the post list and
regenerates all content. Learn more about HotReload [here](docs/hotreload).

## `dotnet new` template

Creating new sites just got easier! Install the new `dotnet new` templates with:

```sh
dotnet new install BlazorStatic.Templates
```

Then create your new BlazorStatic project:

```sh
dotnet new BlazorStaticMinimalBlog -o MyBlazorStaticApp
```

The template includes a GitHub action for easier publishing to GitHub Pages. If you plan to deploy elsewhere, see
the [deployment docs](docs/deployment).

This was my first experience with the [dotnet templating engine](https://github.com/dotnet/templating), and it exceeded
my expectations. For example, have you ever wondered why the localhost port of your new dotnet app is always
different? [Here’s the explanation](https://github.com/dotnet/aspnetcore/blob/6a40a23cd9242fc7ea01a9ae4f58886068f7257c/src/ProjectTemplates/Web.ProjectTemplates/content/BlazorWeb-CSharp/.template.config/template.json#L235).
BlazorStatic now supports similar functionality through the `dotnet new` template, as
shown [here](https://github.com/tesar-tech/BlazorStatic/blob/e1f07797b53559256ee17e1fcdc1f4d32784f1be/BlazorStaticTemplates/templates/BlazorStaticMinimalBlogTemplate/.template-config/template.json#L40).

The template is actually the [BlazorStaticMinimalBlog repo](https://github.com/tesar-tech/BlazorStaticMinimalBlog),
which you can "Use as template" if you prefer a quick setup without needing a desktop IDE.

`BlazorStaticMinimalBlog` is quite opinionated, featuring TailwindCSS and a layout I find readable. However, it doesn't
have to be this way—many other templates can be created and used with BlazorStatic now. If you have any ideas, don't
hesitate to share them.

## HTML Images in Markdown Files

This feature wasn't supported before, but now it is:

```markdown
I am **markdown**. Here is an image in HTML:

<img src="img.jpg"/>
```

### Feedback

Try out the template and let me know if it meets your expectations or if you need any adjustments.
Share your feedback [by creating an issue](https://github.com/tesar-tech/BlazorStatic/issues/new) or join the
conversation in the [Discord server](https://discord.gg/DsAXsMuEbx).
