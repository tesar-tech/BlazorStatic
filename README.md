# Blazor Static

![](BlazorStaticWebsite/wwwroot/imgs/logo.png)

Leverage the power of Blazor SSR to create a static website.

## How to start?

BlazorStatic is a library, that you use in your Blazor app. Few ways how to start:

### Fork or clone BlazorStaticMinimalBlog

[BlazorStaticMinimalBlog](https://github.com/tesar-tech/BlazorStaticMinimalBlog) is a minimal blog website that uses BlazorStatic.
Quick and easy way hot to have your page deployed in 3 minutes. It includes the publish github action, so you can just fork it, enable GitHub Pages and you have your own BlazorStatic website up and running. Then you can edit the content, change styling, configuration, etc.

### dotnet global tool
That would be great, it wouls scaffold the project for you. **TODO**


### Do it from scratch
- Starts with creating Blazor SSR app, then adding contnet and using BlazorStatic. All is described [here](new-start). 
You will end up with something like BlazorStaticMinimalBlog. Good (but not necessary) for understanding how it works. 


## How it works?

- Build and debug your website as usual.
  - BlazorStatic helps you with:
    - processing markdown and front matter (using Markdig an yamldotnet)
    - creating `Blog` and `Tags` pages by exposing `Post<FrontMatter>` so you can use it in your components (`post.FrontMatter.Title `)

- Use BlazorStatic to:
  - find pages to generate
  - copy neccessary contntet to the output (e.g. `wwwroot` folder)
  - generate html files.
- Build, run and generate static html files in CI/CD pipeline.
- Host static html on anywhere (GitHub Pages, Azure Static Web Apps, Netlify, ...)

Themes- no themese, doesnt force you
Taiwlidn ok, boostrap ok. 

> README under construction


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
