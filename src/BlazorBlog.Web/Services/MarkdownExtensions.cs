using System.Linq;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using YamlDotNet.Serialization;

namespace BlazorBlog.Web.Services
{
    public static class MarkdownExtensions
    {
        private static readonly IDeserializer s_yamlDeserializer =
            new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .Build();

        private static readonly MarkdownPipeline s_pipeline
            = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                .UseAdvancedExtensions()
                .Build();

        public static string GetHtmlFromMarkdown(this string markdown)
        {
            return Markdown.ToHtml(markdown, s_pipeline);
        }

        public static T? GetFrontMatter<T>(this string markdown)
        {
            var document = Markdown.Parse(markdown, s_pipeline);
            var block = document
                .Descendants<YamlFrontMatterBlock>()
                .FirstOrDefault();

            if (block == null)
                return default;

            var yaml =
                block
                    // this is not a mistake
                    // we have to call .Lines 2x
                    .Lines // StringLineGroup[]
                    .Lines // StringLine[]
                    .OrderByDescending(x => x.Line)
                    .Select(x => $"{x}\n")
                    .ToList()
                    .Select(x => x.Replace("---", string.Empty))
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Aggregate((s, agg) => agg + s);

            return s_yamlDeserializer.Deserialize<T>(yaml);
        }
    }
}
