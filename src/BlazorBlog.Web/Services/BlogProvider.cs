using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace BlazorBlog.Web.Services
{
    public record BlogPost(
        string Slug,
        string AbsolutePath,
        string Title,
        string Description,
        string[] Tags,
        DateTime Date,
        string? RepositoryUrl
    );

    public record Tag(string Slug, string Title, BlogPost[] Posts);

    public record Series(string Slug, string Title, BlogPost[] Posts);

    public class BlogProvider
    {
        private readonly IBlogParser _parser;
        private readonly BlogSettings _blogSettings;

        private readonly Lazy<(
            ImmutableList<BlogPost> Posts,
            ImmutableList<Tag> Tags,
            ImmutableList<Series> Series)> _data;

        public ImmutableList<BlogPost> Posts => _data.Value.Posts;
        public ImmutableList<Series> Series => _data.Value.Series;
        public ImmutableList<Tag> Tags => _data.Value.Tags;
        public BlogSettings Settings => _blogSettings;

        public BlogProvider(IBlogParser parser, BlogSettings blogSettings)
        {
            _parser = parser;
            _blogSettings = blogSettings;
            _data = new Lazy<(ImmutableList<BlogPost> Posts, ImmutableList<Tag> Tags, ImmutableList<Series>)>(Populate);
        }


        private (ImmutableList<BlogPost>, ImmutableList<Tag>, ImmutableList<Series>) Populate()
        {
            var posts = new List<BlogPost>();
            var tagsToBlogPost = new List<(string Tag, BlogPost Post)>();
            var seriesToBlogPost = new List<(string Series, BlogPost Post)>();

            var parsedPosts = _parser.GetAllPosts();

            foreach (var (frontMatter, fileModifiedDateTime, absolutePath, relativePath) in parsedPosts)
            {
                string path = !relativePath.EndsWith(".md") ? relativePath : relativePath.Replace(".md", "");

                var slug = StringHelper.UrlFriendly(path, preserveFrontSlash: true);
                var post = new BlogPost(
                    slug,
                    absolutePath,
                    frontMatter?.Title ?? slug,
                    frontMatter?.Description ?? string.Empty,
                    frontMatter?.Tags ?? Array.Empty<string>(),
                    frontMatter?.Date ?? fileModifiedDateTime,
                    frontMatter?.Repository
                );

                posts.Add(post);
                tagsToBlogPost.AddRange(post.Tags.Select(tag => (tag, post)));
                if (string.IsNullOrWhiteSpace(frontMatter?.Series) == false)
                {
                    seriesToBlogPost.Add((frontMatter.Series, post));
                }
            }

            var tags = tagsToBlogPost.GroupBy(i => StringHelper.UrlFriendly(i.Tag))
                .Select(groupedTag => new
                {
                    seriesName = groupedTag,
                    mostPopularSeriesName = groupedTag.GroupBy(i => i.Tag)
                        .OrderByDescending(grp => grp.Count())
                        .Select(grp => grp.Key)
                        .First()
                })
                .Select(grp => new Tag(grp.seriesName.Key, grp.mostPopularSeriesName,
                    grp.seriesName.Select(i => i.Post).ToArray()));

            var series = seriesToBlogPost.GroupBy((i => StringHelper.UrlFriendly(i.Series)))
                .Select(groupedSeries => new
                {
                    seriesName = groupedSeries,
                    mostPopularSeriesName = groupedSeries.GroupBy(i => i.Series)
                        .OrderByDescending(grp => grp.Count())
                        .Select(grp => grp.Key)
                        .First()
                })
                .Select(grp => new Series(grp.seriesName.Key, grp.mostPopularSeriesName,
                    grp.seriesName.Select(i => i.Post).ToArray()));

            return (
                posts.ToImmutableList(),
                tags.ToImmutableList(),
                series.ToImmutableList()
            );
        }
    }
}
