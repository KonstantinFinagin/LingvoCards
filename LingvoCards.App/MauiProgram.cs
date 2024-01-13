using LingvoCards.App.ViewModels;
using LingvoCards.Dal.Repositories;
using LingvoCards.Dal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LingvoCards.App
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

            // Register DbContext
            builder.Services.AddDbContext<LearningCardContext>(options =>
            {
                options.UseSqlite($"Filename={GetDatabasePath()}", x => x.MigrationsAssembly(typeof(LearningCardContext).Assembly.FullName));
            });

            // Register repositories
            builder.Services.AddScoped<CardRepository>();
            builder.Services.AddScoped<TagRepository>();

            builder.Services.AddScoped<CardsViewModel>();
            builder.Services.AddScoped<TagViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static string GetDatabasePath()
        {
            var databasePath = "";
            var databaseName = "learningcards.db";

            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                databasePath = Path.Combine(FileSystem.AppDataDirectory, databaseName);
            }

            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                throw new NotImplementedException("iOS not supported yet");
            }

            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                return databaseName;
            }

            return databasePath;
        }
    }
}
