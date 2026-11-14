using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using StudentPerformanceAnalyzer.Services;
using StudentPerformanceAnalyzer.Strategies;

namespace StudentPerformanceAnalyzer
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<IXmlDataService, XmlDataService>();

            builder.Services.AddTransient<SaxSearchStrategy>();
            builder.Services.AddTransient<DomSearchStrategy>();
            builder.Services.AddTransient<LinqSearchStrategy>();

            return builder.Build();
        }
    }
}