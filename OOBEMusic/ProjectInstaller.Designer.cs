using System;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Diagnostics;

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
        }

    }
}