---
title: Release of 1.0.0-beta.7
lead: Allows the use of multiple front matter classes
published: 2024-06-10
tags: [release]
authors:
- name: "Jan Tesař"
gitHubUserName: "tesar-tech"
xUserName: "tesar_tech"
---

## Breaking Changes

- The `BeforeFilesGenerationAction` property in `BlazorStaticOptions` has been removed. Use `BlazorStaticOptions.AddBeforeFilesGenerationAction` instead. This change is due to internal handling of blog post parsing. Blog post parsing no longer has a special property (`BlogAction`) and is now handled by `AddBeforeFilesGenerationAction`.
- Blog posts are now parsed **after** the custom `beforeFilesGenerationAction` (which can be added via `opt.AddBeforeFilesGenerationAction`). This change should have no significant effect.

## New Features

- Multiple `BlogServices` can now be used, which is valuable when you have multiple "sections" with different FrontMatter classes. In BlazorStaticWebsite, a new [projects section](projects) was created to demonstrate this usage. See `Program.cs` and `ProjectFrontMatter`.

  ```csharp
  builder.Services.AddBlogService<FrontMatter>(opt => {
  }).AddBlogService<ProjectFrontMatter>(opt => {
    opt.MediaFolderRelativeToContentPath = null;
    opt.ContentPath = Path.Combine("Content", "Projects");
    opt.AddTagPagesFromPosts = false;
    opt.BlogPageUrl = "projects";
  });
  ```

  This feature revealed a few refactorings (including the breaking changes) that have been done in this new version. It also highlighted that the name `BlogService` isn't quite precise. We will work on that.

- You can now define the blog media path as `null`, which will remove all warnings and errors related to a non-existent folder.

  ```csharp
  builder.Services.AddBlogService<ProjectFrontMatter>(opt => {
    opt.MediaFolderRelativeToContentPath = null;
  });
  ```

## Fixes

- The program will no longer fail when the media path doesn't exist. It will issue a warning instead.

  ```shell
  warn: BlazorStatic.Services.BlogService[0]
      The folder for the media path (C:\FullPath\BlazorStatic\BlazorStaticWebsite\Content\Projects\media) doesn't exist
  ```
