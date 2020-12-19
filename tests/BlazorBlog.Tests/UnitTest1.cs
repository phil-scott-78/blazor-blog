using System.Linq;
using BlazorBlog.Web.Services;
using Bogus;
using Moq;
using Shouldly;
using Xunit;

namespace BlazorBlog.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Can_parse_blog_posts_into_data()
        {
            var posts = new[]
            {
                GetBogusPost("Item 1", new[] {"SQL Server", "Entity Framework"}, "My entity framework posts"),
                GetBogusPost("Item 2", new[] {"SQL-Server", "Entity Framework"}, "My entity framework posts"),
                GetBogusPost("Item 3", new[] {"Sql Server", "Entity Framework"}, "My entity framework posts"),
                GetBogusPost("Item 4", new[] {"Sql Server", "Entity-Framework"}, "My entity framework posts"),
                GetBogusPost("Item 5", new[] {"Roslyn"}, ""),
            };

            var mock = new Mock<IBlogParser>();
            mock.Setup(i => i.GetAllPosts()).Returns(posts);

            var provider = new BlogProvider(mock.Object, new BlogSettings());

            provider.Posts.ShouldSatisfyAllConditions(i =>
            {
                i.Count.ShouldBe(5);
            });

            provider.Series.ShouldSatisfyAllConditions(i =>
            {
                i.Count.ShouldBe(1);
                i.First().ShouldSatisfyAllConditions(series =>
                {
                    var (slug, title, blogPosts) = series;
                    title.ShouldBe("My entity framework posts");
                    slug.ShouldBe("my-entity-framework-posts");
                    blogPosts.Length.ShouldBe(4);
                });
            });

            provider.Tags.ShouldSatisfyAllConditions(i =>
            {
                i.Count.ShouldBe(3);
                i.Select(tag => tag.Slug).ShouldBe(new[] {"sql-server", "roslyn", "entity-framework"}, true);
                i.Select(tag => tag.Title).ShouldBe(new[] {"Sql Server", "Roslyn", "Entity Framework"}, true);
                i.First(tag => tag.Slug == "sql-server").Posts.Length.ShouldBe(4);
                i.First(tag => tag.Slug == "entity-framework").Posts.Length.ShouldBe(4);
                i.First(tag => tag.Slug == "roslyn").Posts.Length.ShouldBe(1);
            });
        }

        private static BlogPostFromMarkdown GetBogusPost(string title, string[] tags, string series)
        {
            var faker = new Faker();
            var frontMatter = new BlogFrontMatter()
            {
                Series = series,
                Tags = tags,
                Title = title,
                Date = faker.Date.Past(),
                Description = faker.Lorem.Paragraph(),
                Repository = faker.Internet.UrlWithPath("github"),
            };

            return new BlogPostFromMarkdown(frontMatter, faker.Date.Past(), faker.System.FilePath(),
                faker.System.FileName());
        }
    }
}
