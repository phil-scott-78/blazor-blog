using System.Collections.Immutable;
using BlazorBlog.Web.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorBlog.Web.Pages
{
    public class BasePage : LayoutComponentBase
    {
        // this needs to be injected via attribute because we can't inherit from
        // a base page unless it has a parameterless constructor
        [Inject] private BlogProvider BlogProvider { get; set; } = null!;

        protected ImmutableList<BlogPost> Posts = ImmutableList<BlogPost>.Empty;

        protected ImmutableList<BlazorBlog.Web.Services.Tag> Tags = ImmutableList<BlazorBlog.Web.Services.Tag>.Empty;

        protected BlogSettings BlogSettings = null!;

        protected override void OnInitialized()
        {
            this.Posts = this.BlogProvider.Posts;
            this.Tags = this.BlogProvider.Tags;
            this.BlogSettings = this.BlogProvider.Settings;
            base.OnInitialized();
        }
    }
}
