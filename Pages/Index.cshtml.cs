using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using Python.Runtime;

namespace ml_webapp.Pages;
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public string? PredictionResult { get; set; }

    public async Task<IActionResult> OnPostAsync(string description, string supplier)
    {
        PredictionResult = await Predict(description, supplier);

        return Page();
    }

    private async Task<string> Predict(string description, string supplier)
    {
        Console.WriteLine("Reached method");
        using (Py.GIL()) {
            Console.WriteLine("Reached this line");
        }
        Console.WriteLine(description);
        Console.WriteLine(supplier);
        return (description.Length + supplier.Length).ToString();
    }

    public void OnGet() {}
}
