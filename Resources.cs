using Python.Runtime;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

class Resources : IHostedService {
    public static dynamic? builtins;
    public static dynamic? pk;
    public static dynamic? np;
    public static dynamic? joblib;
    public static dynamic? glove;
    public static dynamic? model;

    private static void initResources() {
        Console.WriteLine("Reached line 2");
        using (Py.GIL()) {
            builtins = Py.Import("builtins");
            pk = Py.Import("pickle");
            np = Py.Import("numpy");
            joblib = Py.Import("joblib");
            dynamic kerasModels = Py.Import("tensorflow.keras.models");
            model = kerasModels.load_model("model.h5");
            PyObject pyFile = builtins.open("test.pkl", "rb");
            glove = pk.load(pyFile);
        }
        Console.WriteLine("Py.GIL() thread end");
        Console.WriteLine("Reached line 3");
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        Console.WriteLine("Reached line 1");
        Runtime.PythonDLL = "/Library/Frameworks/Python.framework/Versions/3.12/bin/python3";
        PythonEngine.Initialize();
        var m_threadState = PythonEngine.BeginAllowThreads();
        initResources();
        Console.WriteLine("Reached line 4");
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
