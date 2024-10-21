using Python.Runtime;
class Program {
    static void Main(string[] args) {
        Runtime.PythonDLL = "/Library/Frameworks/Python.framework/Versions/3.12/bin/python3";
        PythonEngine.Initialize();
        var m_threadState = PythonEngine.BeginAllowThreads();
        try {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Resources.initResources();
            watch.Stop();
            Console.WriteLine($"Init complete - {watch.ElapsedMilliseconds} milliseconds");

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Logging.AddConsole();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        } finally {
            Console.WriteLine("Shutting down from Program.cs");
            AppContext.SetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", true);
            PythonEngine.Shutdown();
            AppContext.SetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", false);
            Console.WriteLine("Shut down");
        }

    }
}
