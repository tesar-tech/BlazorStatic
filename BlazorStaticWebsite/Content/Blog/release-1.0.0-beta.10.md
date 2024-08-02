---
title: 1.0.0-beta.10 - Better way of finding pages 
lead: No need for magic strings in routes
published: 2024-08-10
tags: [release]
authors:
- name: "Jan Tesař"
  gitHubUserName: "tesar-tech"
  xUserName: "tesar_tech"
- name: "Melty Observer"
  gitHubUserName: "MeltyObserver"
---

## Breaking Changes

- `RazorPagesPaths` is no longer available. It was used to get the location of razor pages to scan for the `@page` directive.
  Now, BlazorStatic scans the assembly for all pages.

## Features

- You no longer need to use the `@page` directive; you can use the `Route` attribute instead (the `@page` directive is translated to that anyway).

Before:

```
@page "/projects"
```

After:

```
@attribute [Route("/projects")]*@
```


You might say it is uglier now, and you would be right.
The `@page` directive simplifies the usage of the `Route` attribute, but it only supports strings.
To avoid magic strings and keep all routes in a single place, I recommend defining routes centrally.
For example, in BlazorStaticWebsite, the `projects` page is defined as:

```
@attribute [Route($"/{WebsiteKeys.ProjectsUrl}")]
```

Which keeps the `project` string definition in one place. 



