using Python.Runtime;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

class Resources : IHostedService {
    public static dynamic? builtins;
    public static dynamic? np;
    public static dynamic? embedding_dictionary;
    public static dynamic? model;

    private static void initResources() {
        using (Py.GIL()) {
            Console.WriteLine("Py.GIL() thread begins");
            builtins = Py.Import("builtins");
            np = Py.Import("numpy");
            dynamic kerasModels = Py.Import("tensorflow.keras.models");
            model = kerasModels.load_model("ml/models/test_model.h5");
            PyObject pyFile = builtins.open("ml/glove/embedding_dictionary.pkl", "rb");
            dynamic pk = Py.Import("pickle");
            embedding_dictionary = pk.load(pyFile);
        }
        Console.WriteLine("Py.GIL() thread end");
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        Runtime.PythonDLL = "/Library/Frameworks/Python.framework/Versions/3.12/bin/python3";
        PythonEngine.Initialize();
        var m_threadState = PythonEngine.BeginAllowThreads();
        initResources();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        Console.WriteLine("Shutting down from Resources.cs");
        AppContext.SetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", true);
        PythonEngine.Shutdown();
        AppContext.SetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", false);
        Console.WriteLine("Shut down");
        return Task.CompletedTask;
    }
}
