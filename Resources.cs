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
        using (Py.GIL()) {
            Console.WriteLine("Py.GIL() thread begins");
            builtins = Py.Import("builtins");
            pk = Py.Import("pickle");
            np = Py.Import("numpy");
            joblib = Py.Import("joblib");
            dynamic kerasModels = Py.Import("tensorflow.keras.models");
            model = kerasModels.load_model("machine_learning/model.h5");
            PyObject pyFile = builtins.open("machine_learning/test.pkl", "rb");
            glove = pk.load(pyFile);
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
