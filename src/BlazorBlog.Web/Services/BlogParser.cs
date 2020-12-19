using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace BlazorBlog.Web.Services
{
    public record BlogPostFromMarkdown(
        BlogFrontMatter? FrontMatter,
        DateTime FileModifiedDateTime,
        string AbsolutePath,
        string RelativePath);


    public interface IBlogParser
    {
        IEnumerable<BlogPostFromMarkdown> GetAllPosts();
    }

    public class BlogParser : IBlogParser
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BlogParser(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IEnumerable<BlogPostFromMarkdown> GetAllPosts()
        {
            var webRootPath = Path.Combine(_webHostEnvironment.ContentRootPath, "_posts");
            var directory = new DirectoryInfo(webRootPath)
                .GetFiles("*.md", SearchOption.AllDirectories);

            foreach (var file in directory)
            {
                var fileFullName = file.FullName;
                var content = File.ReadAllText(fileFullName);
                var frontMatter = content.GetFrontMatter<BlogFrontMatter>();
                var relativePath = Path.GetRelativePath(webRootPath, fileFullName);
                yield return new BlogPostFromMarkdown(frontMatter, file.LastWriteTime, fileFullName, relativePath);
            }
        }
    }
}
