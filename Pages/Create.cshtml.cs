using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace issuehelper.Pages;

[BindProperties]
public class CreateModel : PageModel
{
    public required string BuildUrl { get; init; }
    public required string ErrorMatchString { get; set; }
    public string? SampleText { get; set; }
    public bool RegexMatch { get; set; }

    private readonly ILogger<IndexModel> _logger;

    public CreateModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }

    public void OnPost()
    {
        if (!string.IsNullOrEmpty(SampleText) && !string.IsNullOrEmpty(ErrorMatchString))
        {
            if (RegexMatch)
            {
                var regex = new Regex(ErrorMatchString);
                var match = regex.Match(SampleText);
                if (!match.Success)
                {
                    ViewData["matchError"] = "Match failure";
                    return;
                }
            }
            else
            {
                if (!SampleText.Contains(ErrorMatchString))
                {
                    ViewData["matchError"] = "Match failure";
                    return;
                }
            }
        }
        var matchString = JsonEncodedText.Encode(ErrorMatchString, JavaScriptEncoder.UnsafeRelaxedJsonEscaping);
        var newBody = $$"""
## Build Information
Build: {{BuildUrl}}
Build error leg or test failing: N/A
Pull request:
<!-- Error message template  -->
## Error Message

Fill the error message using [step by step known issues guidance](https://github.com/dotnet/arcade/blob/main/Documentation/Projects/Build%20Analysis/KnownIssues.md#how-to-fill-out-a-known-issue-error-section).

<!-- Use ErrorMessage for String.Contains matches. Use ErrorPattern for regex matches (single line/no backtracking). Set BuildRetry to `true` to retry builds with this error. Set ExcludeConsoleLog to `true` to skip helix logs analysis. -->

```json
{
  "ErrorMessage": "{{ (RegexMatch ? "" : matchString) }}",
  "ErrorPattern": "{{ (RegexMatch ? matchString : "") }}",
  "BuildRetry": false,
  "ExcludeConsoleLog": false
}
```
""";
        Request.HttpContext.Response.Redirect($"http://github.com/dotnet/runtime/issues/new?body={Uri.EscapeDataString(newBody)}");
    }
}
