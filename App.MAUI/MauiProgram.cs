using Microsoft.Extensions.Logging;

namespace App.MAUI
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
                    fonts.AddFont("Avenir-Book.otf", "AvenirBook");
                    fonts.AddFont("Avenir-Medium.otf", "AvenirMedium");
                    fonts.AddFont("Avenir-Heavy.otf", "AvenirHeavy");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
