using StyledBlazor;

namespace BlazorBlog.Web.Components
{
    public record PageTitle() : Styled.H2("font-serif font-bold text-3xl lg:font-extrabold lg:text-5xl leading-snug");

    public record PillLink() : Styled.A(
        "text-xs md:text-sm py-0.5 px-1 text-blue-900 bg-blue-50 border border-blue-200 hover:bg-blue-100 hover:text-blue-800 hover:border-blue-300 rounded transition-colors");
}
