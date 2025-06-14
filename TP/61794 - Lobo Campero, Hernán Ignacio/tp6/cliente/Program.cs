using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using cliente;
using Cliente.Services;
using Cliente.Constants;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.Configure<ApiSettings>(options =>
{
    options.BaseUrl = "http://localhost:5184";
    options.Timeout = AppConstants.HTTP_TIMEOUT_SECONDS;
    options.RetryAttempts = AppConstants.RETRY_ATTEMPTS;
});

builder.Services.Configure<AppSettings>(options =>
{
    options.ProductosPorPagina = AppConstants.PRODUCTOS_POR_PAGINA_DEFAULT;
    options.ToastDuration = new ToastDurationSettings();
});


builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("http://localhost:5184"),
    Timeout = TimeSpan.FromSeconds(AppConstants.HTTP_TIMEOUT_SECONDS)
});


builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<CarritoService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<ValidationService>();
builder.Services.AddScoped<DebounceService>();


builder.Logging.SetMinimumLevel(LogLevel.Information);

await builder.Build().RunAsync();
