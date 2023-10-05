using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace issuehelper.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }

    public void OnPost(string issueUrl)
    {
        var uri = new Uri(issueUrl);
        var queryData = HttpUtility.ParseQueryString(uri.Query);
        var body = queryData["body"];
        if (body is null)
        {
            ViewData["Confirmation"] = "No body found in query string";
            return;
        }
        var buildUrl = Regex.Match(body, "^Build: (?<buildUrl>.*)$", RegexOptions.Multiline | RegexOptions.IgnoreCase)
            .Groups["buildUrl"].Value;
        ViewData["Confirmation"] = $@"Confirmation:
        Host: {uri.Host}
        Path: {uri.AbsolutePath}
        Build URL: {buildUrl}
";
        if (!string.IsNullOrWhiteSpace(buildUrl))
        {
            Response.Redirect($"/Create?buildUrl={Uri.EscapeDataString(buildUrl)}");
        }
    }
}
