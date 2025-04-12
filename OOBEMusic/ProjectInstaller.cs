using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Resources;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace OOBEMusic
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        private static readonly ResourceManager rm = new ResourceManager("OOBEMusic.Ressources.Messages", typeof(ProjectInstaller).Assembly);

        public ProjectInstaller()
        {
            var processInstaller = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem
            };

            var serviceInstaller = new ServiceInstaller
            {
                ServiceName = "OOBEMusicService",
                DisplayName = "OOBE Music Player",
                Description = rm.GetString("ServiceDescription"),
                StartType = ServiceStartMode.Automatic
            };

            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);

            serviceInstaller.AfterInstall += new InstallEventHandler(serviceInstaller1_AfterInstall);

        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            try
            {
                // Supprimer le service Windows
                using (var sc = new ServiceController("OOBEMusicService"))
                {
                    if (sc.Status != ServiceControllerStatus.Stopped)
                    {
                        sc.Stop(); // Arrête le service s'il est en cours d'exécution
                    }
                }

                // Supprimer le service du système
                System.Diagnostics.Process.Start("sc.exe", "delete OOBEMusicService");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la désinstallation du service : {ex.Message}");
            }
        }

    }
}
