using System.Globalization;
using System.ServiceProcess;
using System.Threading;

namespace OOBEMusic
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var service = new OOBEMusicPlayer();

            ServiceBase.Run(service); // Lancement du service

        }
    }
}
