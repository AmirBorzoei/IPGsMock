using Microsoft.AspNetCore.Mvc;
using Markdig;

namespace IPGsMock.Home;

[ApiExplorerSettings(IgnoreApi = true)]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var parentDir = Directory.GetParent(currentDir)?.FullName;
        if (parentDir == null)
        {
            return NotFound("در ریشه درایو هستیم و فولدر بالاتری وجود نداره!");
        }

        var readmePath = Path.Combine(parentDir, "README.md");
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