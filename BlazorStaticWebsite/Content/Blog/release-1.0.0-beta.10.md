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

- `RazorPagesPaths` is no longer availabl~~~~e. It was used to get the location of razor pages where to scan for `@page` directive. 
Since now, it scans the assembly, this is no longer needed.  

## Features

- Now you don't need to use `@page` directive, but you can use `Route` attribute (`@page` directive is translated to that anyway)

Before:

```
@page "/projects"
```

After:

```
@attribute [Route("/projects")]*@
```

Well - you might say it is uglier now and you would be right. The `@page` directive is there to simplify the `Route` attribute usage.
But it doesn't support anything else except strings... You might want to (and I recommend) that to avoid magic strings and keep all the
routes in single place. For example here in BlazorStaticWebsite the `projects` page is defined with:

```
@attribute [Route($"/{WebsiteKeys.ProjectsUrl}")]
```

Which keeps the `project` string definition in one place. 



