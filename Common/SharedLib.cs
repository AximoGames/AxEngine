using System.Globalization;
using System.Text;
using Serilog;

namespace Aximo
{

    public static class SharedLib
    {
        public static CultureInfo LocaleInvariant = CultureInfo.InvariantCulture;

        public static void EnableLogging()
        {
            Serilog.Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] <{SourceContext}> {Message:lj}{NewLine}{Exception}")
                .MinimumLevel.Verbose()
                .CreateLogger();
        }

    }
}
