using ProductManager.Blazor.Configuration;
namespace ProductManager.Blazor;

public class Program
{
    public static HttpClient HttpClient { get; private set; } = default!;
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        builder.Services.AddRazorComponents().AddInteractiveServerComponents();

        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
            .AddEnvironmentVariables();
        var apiUrl = builder.Configuration.GetSection("API_URL").Value ?? "https://*:8080";
        HttpClient = new HttpClient
        {
            BaseAddress = new Uri(apiUrl)
        };
        builder.Services.AddBootstrapBlazor();
        builder.Services.AddBlazoredLocalStorage();

        builder.Services.AddSingleton<WeatherForecastService>();
        builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

        builder.Services.AddTableDemoDataService();

        builder.Services.Configure<HubOptions>(option => option.MaximumReceiveMessageSize = null);

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseResponseCompression();
        }

        app.UseStaticFiles();

        app.UseAntiforgery();

        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

        app.Run();
    }
}
