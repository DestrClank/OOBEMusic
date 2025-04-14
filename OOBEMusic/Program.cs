using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Resources;
using System.ServiceProcess;
using System.Threading;

namespace OOBEMusic
{
    public static class Program
    {
        private static readonly ResourceManager rm = new ResourceManager("OOBEMusic.Ressources.Messages", typeof(OOBEMusicPlayer).Assembly);
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                if (File.Exists("OOBEMusic.pdb"))
                {
                    // Mode console pour le débogage
                    var service = new OOBEMusicPlayer();
                    service.StartAsync(new CancellationToken()).Wait();

                    Console.WriteLine(rm.GetString("ServiceInteractiveDebugMode"));
                }
                else
                {
                    var service = new OOBEMusicPlayer();
                    service.RunAsConsole();
                }
            }
            else
            {
                // Mode service Windows
                ServiceBase.Run(new OOBEMusicPlayer());
            }
        }
    }
}
