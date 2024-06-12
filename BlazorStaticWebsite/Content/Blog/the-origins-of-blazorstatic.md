---
title: The Origins of BlazorStatic
lead: BlazorStatic started as a weekend side project, growing into a practical tool for .NET developers seeking an efficient static site generator with Blazor's flexibility.
published: 2023-11-27
tags: [story]
authors:
    - name: "Jan Tesař"
      gitHubUserName: "tesar-tech"
      xUserName: "tesar_tech"
---

I first encountered static site generators back in 2018. At that time, the go-to option in the .NET ecosystem was Wyam, which eventually evolved into [Statiq](https://statiq.dev/). With the advent of Blazor, I began exploring the potential of its component model for static site generation. This idea, however, wasn't embraced in the official channels, as seen in this [GitHub issue](https://github.com/dotnet/aspnetcore/issues/28849) .

My interest in static sites remained dormant until I came across [Genzor](https://github.com/egil/genzor) by Egil Hansen. 

> "Genzor is an experimental project ideally suited for generating files spanning multiple folders, using Blazor component model to generating the output." - Genzor README

My experiments with Genzor were promising for static site generation, but due to time constraints and the eventual archiving of Genzor, the idea didn't immediately progress.

Later, I revisited the .NET landscape for static site generators and realized the options were still limited. However, I stumbled upon [AspNetStatic](https://github.com/ZarehD/AspNetStatic), a relatively new project that inspired me with its simplicity. It used an ASP.NET Core application to generate static sites by fetching rendered pages via HttpClient and saving them as files. This approach was ingenious, yet I found myself needing more features and a different focus than what AspNetStatic offered. Even the author of AspNetStatic [acknowledged](https://github.com/ZarehD/AspNetStatic/issues/3) some limitations with Blazor (but it was in May when no SSR was possible).

Motivated by this, I decided to embark on creating my own static site generator. What I envisioned as a quick weekend project extended over two months of sporadic, yet focused work.
. And now, I'm excited to present BlazorStatic – a tool born from a blend of inspiration, experimentation, and the desire to fill a gap in the .NET static site generation landscape.




