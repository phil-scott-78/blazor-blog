using System;
using YamlDotNet.Serialization;

namespace BlazorBlog.Web.Services
{
#nullable disable

    public class BlogFrontMatter
    {
        [YamlMember(Alias = "tags")] public string[] Tags { get; set; }

        [YamlMember(Alias = "title")] public string Title { get; set; }

        [YamlMember(Alias = "description")] public string Description { get; set; }

        [YamlMember(Alias = "series")] public string Series { get; set; }

        [YamlMember(Alias = "date")] public DateTime Date { get; set; }

        [YamlMember(Alias = "repository")] public string Repository { get; set; }
    }
}
