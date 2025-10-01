using Microsoft.AspNetCore.Mvc;
using Markdig;

namespace IPGsMock.Home;

[ApiExplorerSettings(IgnoreApi = true)]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var readmePath = Path.Combine(currentDir, "README.md");
        if (!System.IO.File.Exists(readmePath))
        {
            return NotFound("فایل README پیدا نشد!");
        }

        var markdown = System.IO.File.ReadAllText(readmePath);

        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        var html = Markdown.ToHtml(markdown, pipeline);

        ViewBag.ReadmeHtml = html;
        return View("Home/Views/Index.cshtml");
    }
}