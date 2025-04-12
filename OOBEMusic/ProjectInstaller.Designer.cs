using System;
using System.Configuration.Install;
using System.ServiceProcess;

namespace OOBEMusic
{
    partial class ProjectInstaller
    {
        // Add the missing event handler method for serviceProcessInstaller1_AfterInstall  
        private void serviceProcessInstaller1_AfterInstall(object sender, System.Configuration.Install.InstallEventArgs e)
        {
            // Implementation for the event handler can be added here if needed.  
        }

        // Add the missing event handler method for serviceInstaller1_AfterInstall  
        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            using (var sc = new ServiceController("OOBEMusicService"))
            {
                try
                {
                    if (sc.Status == ServiceControllerStatus.Stopped)
                    {
                        sc.Start(); // Démarre le service
                        sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10)); // Attend que le service démarre
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors du démarrage du service : {ex.Message}");
                }
            }
        }

    }
}