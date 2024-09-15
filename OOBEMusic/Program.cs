using Microsoft.Extensions.Logging;
using System.ServiceProcess;

namespace OOBEMusic
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddEventLog());

            var service = new OOBEMusicPlayer(loggerFactory.CreateLogger<OOBEMusicPlayer>());

            ServiceBase.Run(service); // Lancement du service

        }
    }
}
