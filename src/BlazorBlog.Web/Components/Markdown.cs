using System.IO;
using BlazorBlog.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorBlog.Web.Components
{
    public class Markdown : ComponentBase
    {
        [Parameter]
        public string FilePath { get; set; } = null!;

        private MarkupString _markupString;

        /// <inheritdoc/>
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
            builder.AddContent(0, _markupString);
        }

        /// <inheritdoc/>
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            var markdown = File.ReadAllText(FilePath);
            _markupString = new MarkupString(markdown.GetHtmlFromMarkdown());
        }
    }
}
