using System.IO;
using AD.ApiExtensions.Configuration;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Primitives;

namespace QuestionnairesApi
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public static class Program
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        public static void Main([NotNull] [ItemNotNull] string[] args)
        {
            BuildWebHost(args).Run();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [Pure]
        [NotNull]
        public static IWebHost BuildWebHost(StringValues args)
        {
            return
                new WebHostBuilder()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseStartup<Startup>(args)
                    .UseHttpSys()
                    .Build();
        }
    }
}