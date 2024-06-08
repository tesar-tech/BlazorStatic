---
title: Release of 1.0.0-beta.7
lead: Allows to use multiple front matter classes, ignores files, and copies static web assets 
published: 2024-06-10
tags: [release]
authors:
    - name: "Jan Tesař"
      gitHubUserName: "tesar-tech"
      twitterUserName: "tesar-tech"
---

## Breaking changes:

- No property `BeforeFilesGenerationAction` in `BlazorStaticOptions` anymore. Use `BlazorStaticOptions.AddBeforeFilesGenerationFunc`
  (or `AddBeforeFilesGenerationAction` if you don't need async). Reason for this comes from internal handling of blog post parsing. 
  Blog posts parsing has no special property (`BlogAction`) now - it is handled by `AddBeforeFilesGenerationFunc`.
- Blog post are now parsed **after** the custom `beforeFilesGenearionAction` (that you might add via `opt.AddBeforeFilesGenerationFunc/Action`)
  If you need to parse the blog post first (and operate with them in `opt.AddBeforeFilesGenerationFunc/Action`), use new property `BlogOptions.IsParsingAsFirstAction`


