using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using Python.Runtime;
using System.Text.RegularExpressions;

namespace ml_webapp.Pages;
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public double? predictionDisplay;
    public List<string>? unrecognizedTokens;

    public async Task<IActionResult> OnPostAsync(string review) {
        predictionDisplay = await Predict(review);
        return Page();
    }

    private static string tokenize(string text) {
        text = Regex.Replace(text, "'", "");
        string pattern = @"([^\w\s])";
        string replacement = " $1 ";
        text = Regex.Replace(text, pattern, replacement);
        text = Regex.Replace(text, "_", " _ ");
        text = Regex.Replace(text, @"\s+", " ");
        text = text.ToLower();
        return text;
    }

    private async Task<double> Predict(string review)
    {
        using (Py.GIL()) {
            if (String.IsNullOrWhiteSpace(review)) {
                review = "";
            }
            char[] charsToTrim = {' '};
            review = review.Trim(charsToTrim);

            review = tokenize(review);
            string[] allTokens = review.Split(' ');

            int maxTokenLength = 250;
            string[] tokens = new string[maxTokenLength];

            int tokenLength = Math.Min(maxTokenLength, allTokens.Length);
            for (int i = 0; i < tokenLength; i++) {
                tokens[i] = allTokens[i];
            }

            PyList pyList = new PyList();

            foreach (string token in tokens) {
                if (token != null && Resources.embedding_dictionary.InvokeMethod("__contains__", new PyObject[] { new PyString(token) }).As<bool>()) {
                    PyObject iterator = PyIter.GetIter(Resources.embedding_dictionary[token]);
                    PyObject item;
                    try {
                        while ((item = iterator.InvokeMethod("__next__")) != null) {
                            dynamic pythonFloat = Resources.np.float32(item).astype(Resources.np.float64);
                            double embedding = (double)pythonFloat;
                            pyList.Append(new PyFloat(embedding));
                        }
                    } catch (Python.Runtime.PythonException e) {
                        if (e.Type.ToString().Contains("StopIteration")) {
                            continue;
                        } else {
                            throw;
                        }
                    }
                } else {
                    if (token != null) {
                        if (unrecognizedTokens != null) {
                            unrecognizedTokens.Add(token);
                        } else {
                            unrecognizedTokens = new List<string>() { token };
                        }
                    }
                    for (int i = 0; i < 300; i++) {
                        pyList.Append(new PyFloat(0));
                    }
                }
            }

            dynamic npEmbeddings = Resources.np.array(pyList, Resources.np.float32);
            int embeddingLength = (int) pyList.Length();
            dynamic X = Resources.np.reshape(npEmbeddings, new int[] { 1, embeddingLength });

            Console.WriteLine(Resources.model.predict(X)); 
        }
        Console.WriteLine("Py.GIL() thread end");
        return 0.0;
    }

    public void OnGet() {}
}
