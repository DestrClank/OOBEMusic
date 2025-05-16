using System;
using System.IO;
using System.Linq;
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
            // Permettre le chargement des DLL managées depuis le dossier "Libs"
            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
            {
                var assemblyName = new System.Reflection.AssemblyName(eventArgs.Name).Name + ".dll";
                var probePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libs", assemblyName);
                if (File.Exists(probePath))
                    return System.Reflection.Assembly.LoadFrom(probePath);
                return null;
            };

            // Ajouter le dossier "Libs" au PATH pour les DLL natives
            var path = Environment.GetEnvironmentVariable("PATH");
            var libsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libs");
            if (Directory.Exists(libsPath) && !path.Contains(libsPath))
            {
                Environment.SetEnvironmentVariable("PATH", libsPath + ";" + path);
            }

            string[] args2 = Environment.GetCommandLineArgs();

            if (Environment.UserInteractive)
            {
                if (File.Exists("OOBEMusic.pdb") || args2.Contains("-debug"))
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
