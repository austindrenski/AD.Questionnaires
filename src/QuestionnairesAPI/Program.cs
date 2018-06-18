using System.IO;
using AD.ApiExtensions.Hosting;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;

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
        public static void Main([NotNull] [ItemNotNull] string[] args) => CreateWebHostBuilder(args).Build().Run();

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Pure]
        [NotNull]
        public static IWebHostBuilder CreateWebHostBuilder([NotNull] string[] args)
            => new WebHostBuilder()
              .UseContentRoot(Directory.GetCurrentDirectory())
              .UseStartup<Startup>(args)
              .UseHttpSys();
    }
}