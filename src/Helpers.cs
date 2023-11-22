namespace BlazorStatic;

using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

public class BlazorStaticHelpers(BlazorStaticOptions options, ILogger<BlazorStaticHelpers> logger)
{
    public  async Task<  (string htmlContent, T frontMatter)> 
        ParseMarkdownFile<T>(string file, (string mediaPathToBeReplaced, string mediaPathNew)? mediaPaths = default, IDeserializer? yamlDeserializer = default) where T : new()
    {
        yamlDeserializer ??= options.FrontMatterDeserializer;
        
        MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
            .UseYamlFrontMatter()
            .Build();
        string markdownContent = await File.ReadAllTextAsync(file);
        MarkdownDocument document = Markdown.Parse(markdownContent, pipeline);

        YamlFrontMatterBlock? yamlBlock = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();
        T frontMatter ;
        if (yamlBlock == null)
        {
            //logger.LogWarning("No YAML front matter found in {file}. The default one will be used!", file);
            frontMatter = new();
        }
        else
        {
            string frontMatterYaml = yamlBlock.Lines.ToString();
                
            try
            {
                frontMatter = yamlDeserializer.Deserialize<T>(frontMatterYaml);
            }
            catch (Exception e)
            {
                frontMatter = new();
                logger.LogWarning("Cannot deserialize YAML front matter in {file}. The default one will be used! Error: {exceptionMessage}", file, e.Message + e.InnerException?.Message);
            }

        }

        string contentWithoutFrontMatter = markdownContent[(yamlBlock == null ? 0 : yamlBlock.Span.End + 1)..];
        string htmlContent = Markdown.ToHtml(ReplaceImagePathsInMarkdown(contentWithoutFrontMatter,mediaPaths), pipeline);
        return (htmlContent, frontMatter);

    }
    
    
      public  void CopyContent(string sourcePath, string targetPath, List<string> ignoredPaths)
    {
        if (File.Exists(sourcePath))//source path is a file
        {
            string? dir = Path.GetDirectoryName(targetPath);
            if (dir == null) return;
            Directory.CreateDirectory(dir);
            File.Copy(sourcePath, targetPath, true);
            return;
        }
        if (!Directory.Exists(sourcePath))
        {
            logger.LogError("Source path ({sourcePath}) does not exist", sourcePath);
            return;
        }

        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }
        var ignoredPathsWithTarget = ignoredPaths.Select(x => Path.Combine(targetPath, x)).ToList();


        //Now Create all of the directories 
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            string newDirPath = ChangeRootFolder(dirPath);
            if (ignoredPathsWithTarget.Contains(newDirPath))//folder is mentioned in ignoredPaths, don't create it
                continue;
            Directory.CreateDirectory(newDirPath);
        }
        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            string newPathWithNewDir = ChangeRootFolder(newPath);
            if (ignoredPathsWithTarget.Contains(newPathWithNewDir)//file is mentioned in ignoredPaths
                || !Directory.Exists(Path.GetDirectoryName(newPathWithNewDir)))//folder where this file resides is mentioned in ignoredPaths
                continue;
            File.Copy(newPath, newPathWithNewDir, true);
        }
        return;

        string ChangeRootFolder(string dirPath)//for example  from "wwwroot/imgs" to "output/imgs" (safer string.Replace)
        {
            string relativePath = dirPath[sourcePath.Length..].TrimStart(Path.DirectorySeparatorChar);
            return Path.Combine(targetPath, relativePath);
        }
    }
      
      //replace the image paths in the markdown content with the correct relative path
      //looks for options.Blog.MediaFolder/fileName nad make it to blogMediaRequestPathRelative/fileName
      //this way the .md file can be edited with images in folder next to them, like users are used to.
      string ReplaceImagePathsInMarkdown(string markdownContent,(string originalPath ,string newPath )? mediaPaths = default)
      {
          //options.Blog.MediaFolder
          if (mediaPaths == null) return markdownContent;
          string pattern = $@"!\[(.*?)\]\({mediaPaths.Value.originalPath}\/(.*?)\)";
          string replacement = $"![$1]({mediaPaths.Value.newPath}/$2)";
          string modifiedMarkdown = Regex.Replace(markdownContent, pattern, replacement);

          return modifiedMarkdown;
      }
}
