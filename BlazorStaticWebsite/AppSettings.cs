namespace BlazorStaticWebsite;

using System.ComponentModel.DataAnnotations;

//consumes the config and validates it
public  class AppSettings
{
    [Required]
    public required  string BlogPostStorageAddress { get; init; } 

    [Required]
    public required string GitHubRepo { get; init; } 

}
