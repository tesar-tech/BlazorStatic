---
title: "1.0.0-beta.14 - Tags have encoder, welcome to blazorstatic.net, new BlazorStatic organization, templates are fixed for green button"
lead: ""
isDraft: true
published: 2024-10-30
tags: [release, tag encoder, blazorstatic.net , BlazorStatic organization, dotnet new template]
authors:
  - name: "Jan Tesař"
    gitHubUserName: "tesar-tech"
    xUserName: "tesar_tech"
  - name: "Patrick T Coakley"
    gitHubUserName: "patricktcoakley"

---

## Major changes
The way how BlazorStatic services work has been adjusted. Mainly which service is responsible for what. This resulted in total separation of
`Blog` related stuff into separated folder (and namespace `BlazorStatic.Blog`). Now that's kind of recepie how to broathen the functionality of
BlazorStatic.

The main reason for this was the work with Tags. Previously the tags were part of `IFrontMatter`, that's not the case anymore, but new interface has appeared
called `IBlogFrontMatter:IFrontMatter`. In similar manner all other service and options has been adjusted.

- In `Program.cs`: `.AddBlazorStaticContentService<MyFrontMatter>` becomes eitrher stays the same or becomes `.AddBlogService<MyFrontMatter>`. In the later chase the `MyFrontMatter` needs to implement `IBlogFrontMatter`
(which makes the tags available).
- You also change this when injectiong the service (typicaly in `.razor`). `@inject BlazorStaticContentService<BlogFrontMatter> blazorStaticContentService
` => `@inject BlogService<BlogFrontMatter> blogService`

Why the tags were the reason for this change? Because it's a Blog thing, not static content thing. You can imagine static content area without tags.
Having just tags there wouldn't be a problem, but what about future categories? what if we want to extend the tags with some computation logic?
Where do you draw the line between what is and what is not supposed to be as a part of static content? Now check the line is really about
"what will be part of every static content" (media, posts, url. See BlazorStaticContentOptions) vs "blog related stuff" (now just the tags. See BlogOptions).

Also I've implemented the interface for options (IBlazorStaticContentOptions) with few required properties and methods including `CheckOptions` that will check if
the necessary properties are set up. You cannot really do this with `required` keyword, because it would make the service setup impossible. I mean this:

```csharp
  .AddBlazorStaticContentService<ProjectFrontMatter>(opt=> {
        opt.ContentPath = Path.Combine("Content", "Projects");//opt has to be created before here
        opt.PageUrl = WebsiteKeys.ProjectsUrl;
    });
```



## Breaking Changes

- The services setup is broken, but it will tell you and fix is easy..

## tags have encoders

### Feedback

Try out the template and let me know if it meets your expectations or if you need any adjustments.
Share your feedback [by creating an issue](https://github.com/BlazorStatic/BlazorStatic/issues/new) or join the
conversation in the [Discord server](https://discord.gg/DsAXsMuEbx).
