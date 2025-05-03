using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace OOBEMusic.Utils
{
    public static class ServiceHelper
    {
        public static ServiceControllerStatus GetServiceStatus(string serviceName)
        {
            try
            {
                using (ServiceController service = new ServiceController(serviceName))
                {
                    return service.Status;
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new InvalidOperationException($"Failed to get the status of the service '{serviceName}'.", ex);
            }
        }

        public static bool StartService(string serviceName, int timeoutMilliseconds = 30000)
        {
            try
            {
                using (ServiceController service = new ServiceController(serviceName))
                {
                    if (service.Status == ServiceControllerStatus.Running)
                        return true;

                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(timeoutMilliseconds));
                    return service.Status == ServiceControllerStatus.Running;
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new InvalidOperationException($"Failed to start the service '{serviceName}'.", ex);
            }
        }

        public static bool StopService(string serviceName, int timeoutMilliseconds = 30000)
        {
            try
            {
                using (ServiceController service = new ServiceController(serviceName))
                {
                    if (service.Status == ServiceControllerStatus.Stopped)
                        return true;

                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(timeoutMilliseconds));
                    return service.Status == ServiceControllerStatus.Stopped;
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new InvalidOperationException($"Failed to stop the service '{serviceName}'.", ex);
            }
        }

        public static bool PauseService(string serviceName, int timeoutMilliseconds = 30000)
        {
            try
            {
                using (ServiceController service = new ServiceController(serviceName))
                {
                    if (service.Status == ServiceControllerStatus.Paused)
                        return true;

                    service.Pause();
                    service.WaitForStatus(ServiceControllerStatus.Paused, TimeSpan.FromMilliseconds(timeoutMilliseconds));
                    return service.Status == ServiceControllerStatus.Paused;
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new InvalidOperationException($"Failed to pause the service '{serviceName}'.", ex);
            }
        }

        public static void DisableService(string serviceName)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "sc";
                process.StartInfo.Arguments = "config " + serviceName + " start= disabled";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new InvalidOperationException($"Failed to disable the service '{serviceName}'.", ex);
            }
        }

        public static void EnableService(string serviceName)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "sc";
                process.StartInfo.Arguments = "config " + serviceName + " start= auto";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new InvalidOperationException($"Failed to enable the service '{serviceName}'.", ex);
            }
        }

        public static bool IsServiceDisabled(string serviceName)
        {
            try
            {
                // Accéder à la clé de registre du service
                string registryPath = $@"SYSTEM\CurrentControlSet\Services\{serviceName}";
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath))
                {
                    if (key != null)
                    {
                        // Lire la valeur "Start"
                        object startValue = key.GetValue("Start");
                        if (startValue != null && (int)startValue == 4)
                        {
                            // 4 signifie que le service est désactivé
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Gérer les exceptions si nécessaire
                Console.WriteLine($"Erreur lors de la vérification du service : {ex.Message}");
            }

            return false; // Retourne false si le service n'est pas désactivé
        }

        public static bool RestartService(string serviceName, int timeoutMilliseconds = 30000)
        {
            try
            {
                if (StopService(serviceName, timeoutMilliseconds))
                {
                    return StartService(serviceName, timeoutMilliseconds);
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new InvalidOperationException($"Failed to restart the service '{serviceName}'.", ex);
            }
        }
    }
}
