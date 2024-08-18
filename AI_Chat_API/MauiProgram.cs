using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using AI_Chat_API.Services;
using AI_Chat_API.ViewModels;
using AI_Chat_API.Views;

namespace AI_Chat_API
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

            // Register services and view models
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<ChatSessionViewModel>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ChatSessionPage>();

            return builder.Build();
        }
    }
}
