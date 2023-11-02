# Blazor Static

Is a way how to use Blazor component model to scaffold static website with some helpers for making it pleasant experience.

> README under construction

## How to start?

Multiple ways:

### BlazorStaticMinimalBlog

- Fork [link](https://github.com/tesar-tech/BlazorStaticMinimalBlog)
- Let it build and boom - you have your website up and running.
- Edit the content, change styling, configuration, etc.

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



## Key features

- Create website with .NET 8, run it, test it, debug it as usual.
- Add a little bit of magic to your `Program.cs` and the whole website will be generated as static html.
- Use Markdown for content you you wish to do it. 
- Host static html on anywhere (GitHub Pages, Azure Static Web Apps, Netlify, ...)

## How it works?

- You work in the components, for example check the Blog.razor. It is using markdown file as a source of content.
You can use all the stuff like markwond settings, changing urls, etc in here.

We need the magic for few things:

- Generate static html from Blazor components
- Use Markdown

## Why TailwindCSS?

Because I like it. You don't have to use it. You can use any CSS framework you like, just like in any other Blazor app.

The theme is inspired by https://github.com/timlrx/tailwind-nextjs-starter-blog

## What is static website?

### Blazor Static vs Blazor WebAssembly

## How to use it?

## Creating your own theme from default Blazor template

- Delete blazor.js

## Specify your own FrontMatter

## ToDo

- Tag cloud
- Comments
- Search
- RSS
- Sitemap
- Google Analytics
- SEO
- Subscribe to newsletter
