using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace SongCollector
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkitMediaElement()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("fontello.ttf", "Icons");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            var dbName = "db.db3";
            var dbPath = DeviceInfo.Platform == DevicePlatform.iOS ?
                    Path.Combine(Environment
                    .GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", dbName) :
                    Path.Combine(Environment
                    .GetFolderPath(Environment.SpecialFolder.LocalApplicationData), dbName);
            builder.Services.AddSingleton(new Database(dbPath));
            builder.Services.AddTransientWithShellRoute<MainPage, MainViewModel>(nameof(MainPage));
            return builder.Build();
        }
    }
}
