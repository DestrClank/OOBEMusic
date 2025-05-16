using System;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
                        try
                        {
                            sc.Start(); // Démarre le service
                            sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10)); // Attend que le service démarre
                            Logging.EventLogger.LogToEventViewer(rm.GetString("ServiceStartedUponInstall"), EventLogEntryType.Information);
                        }
                        catch (InvalidOperationException ex)
                        {
                            Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("ServiceOperationError"), ex.Message), EventLogEntryType.Error);
                        }
                        catch (System.TimeoutException ex)
                        {
                            Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("ServiceTimeoutError"), ex.Message), EventLogEntryType.Error);
                        }
                        catch (Exception ex)
                        {
                            Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("ServiceUnexpectedError"), ex.Message), EventLogEntryType.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("ServiceStartError"), ex.Message), EventLogEntryType.Error);
                }
            }

            // --- Ajout du dossier d'installation dans le PATH système ---
            try
            {
                // Récupère le dossier d'installation (chemin du service installé)
                string installDir = Context.Parameters.ContainsKey("targetdir")
                    ? Context.Parameters["targetdir"]
                    : AppDomain.CurrentDomain.BaseDirectory;
                installDir = installDir.TrimEnd(Path.DirectorySeparatorChar);

                // Récupère la variable PATH système
                string path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
                if (!path.Split(';').Contains(installDir, StringComparer.OrdinalIgnoreCase))
                {
                    string newPath = path + ";" + installDir;
                    Environment.SetEnvironmentVariable("PATH", newPath, EnvironmentVariableTarget.Machine);
                }
                NotifyEnvironmentChanged();
            }
            catch (Exception ex)
            {
                Logging.EventLogger.LogToEventViewer("Erreur lors de l'ajout du dossier d'installation au PATH : " + ex.Message, EventLogEntryType.Error);
            }

        }
    }
}