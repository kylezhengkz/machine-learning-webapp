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
        if (String.IsNullOrWhiteSpace(description)) {
            description = "";
        }
        if (String.IsNullOrWhiteSpace(supplier)) {
            supplier = "";
        }
        using (Py.GIL()) {
            Console.WriteLine("Py.GIL() thread begins");
        }
        Console.WriteLine("Py.GIL() thread end");
        Console.WriteLine(description);
        Console.WriteLine(supplier);
        return (description.Length + supplier.Length).ToString();
    }

    public void OnGet() {}
}
