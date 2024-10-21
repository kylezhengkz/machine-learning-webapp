using Python.Runtime;
using System.IO;
using System.Text.RegularExpressions;

static class Resources {
    public static dynamic? builtins;
    public static dynamic? pk;
    public static dynamic? np;
    public static dynamic? joblib;
    public static dynamic? glove;
    public static dynamic? model;
    public static void initResources() {
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
    }
}
