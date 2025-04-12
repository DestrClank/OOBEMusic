using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace OOBEMusicInstaller
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string serviceName = "OOBEMusicService";
            string executablePath = args.FirstOrDefault(arg => arg.StartsWith("/path="))?.Substring(6)
                                    ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OOBEMusic.exe");

            if (!File.Exists(executablePath))
            {
                Console.WriteLine($"Erreur : Le fichier {executablePath} est introuvable.");
                return;
            }

            if (args.Contains("/silent"))
            {
                InstallService(serviceName, executablePath);
                return;
            }

            Console.WriteLine("Voulez-vous installer le service Windows 'OOBEMusic' sur cet ordinateur ? (O/N)");
            string response = Console.ReadLine()?.Trim().ToUpper();

            if (response == "O")
            {
                InstallService(serviceName, executablePath);
            }
            else
            {
                Console.WriteLine("Installation annulée.");
            }
        }

        private static void InstallService(string serviceName, string executablePath)
        {
            try
            {
                if (ServiceController.GetServices().Any(s => s.ServiceName == serviceName))
                {
                    Console.WriteLine($"Le service '{serviceName}' est déjà installé.");
                    return;
                }

                Console.WriteLine("Installation du service...");

                // Ajout des paramètres pour le titre et la description du service
                string serviceTitle = "OOBE Background Music Player";
                string serviceDescription = "Service Windows qui lit de la musique dans l'OOBE.";

                // Installation du service
                ManagedInstallerClass.InstallHelper(new[] { executablePath });

                // Configuration du titre, de la description et du compte utilisateur après installation
                using (ServiceController sc = new ServiceController(serviceName))
                {
                    using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Services\{serviceName}", true))
                    {
                        if (key != null)
                        {
                            key.SetValue("DisplayName", serviceTitle);
                            key.SetValue("Description", serviceDescription);
                            key.SetValue("ObjectName", "LocalSystem"); // Définit le service pour s'exécuter en tant que "Système"
                            key.SetValue("Start", 2); // Définit le service pour démarrer automatiquement
                            key.SetValue("ErrorControl", 1); // Définit le contrôle d'erreur
                            key.SetValue("Type", 10); // Définit le type de service
                        }
                    }
                }

                Console.WriteLine("Service installé avec succès.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'installation du service : {ex.Message}");
            }
        }
    }
}
