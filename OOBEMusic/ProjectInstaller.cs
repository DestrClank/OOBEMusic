using Microsoft.Win32;
using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Resources;
using System.ServiceProcess;
using System.Linq;
using System.Runtime.InteropServices;

namespace OOBEMusic
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        private const int HWND_BROADCAST = 0xffff;
        private const int WM_SETTINGCHANGE = 0x001A;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessageTimeout(
            IntPtr hWnd, uint Msg, UIntPtr wParam, string lParam,
            uint fuFlags, uint uTimeout, out UIntPtr lpdwResult);

        private void NotifyEnvironmentChanged()
        {
            UIntPtr result;
            SendMessageTimeout(
                (IntPtr)HWND_BROADCAST,
                WM_SETTINGCHANGE,
                UIntPtr.Zero,
                "Environment",
                0, 1000, out result);
        }
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

                // Supprimer la clé de registre
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\DestrClank", writable: true);
                if (key != null)
                {
                    key.DeleteSubKeyTree("OOBEMusic");
                }

                if (EventLog.SourceExists("OOBEMusic"))
                {
                    // Supprimer la source d'événements
                    EventLog.DeleteEventSource("OOBEMusic");
                }

            }
            catch (Exception ex)
            {
                Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("ServiceUninstallError"), ex.Message), EventLogEntryType.Error);
            }

            base.Uninstall(savedState);

            // --- Suppression du dossier d'installation dans le PATH système ---
            try
            {
                // Récupère le dossier d'installation (chemin du service installé)
                string installDir = Context.Parameters.ContainsKey("targetdir")
                    ? Context.Parameters["targetdir"]
                    : AppDomain.CurrentDomain.BaseDirectory;
                installDir = installDir.TrimEnd(Path.DirectorySeparatorChar);

                // Récupère la variable PATH système
                string path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
                var paths = path.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(p => !string.Equals(p.TrimEnd(Path.DirectorySeparatorChar), installDir, StringComparison.OrdinalIgnoreCase));
                string newPath = string.Join(";", paths);
                Environment.SetEnvironmentVariable("PATH", newPath, EnvironmentVariableTarget.Machine);
            }
            catch (Exception ex)
            {
                Logging.EventLogger.LogToEventViewer("Erreur lors du retrait du dossier d'installation du PATH : " + ex.Message, EventLogEntryType.Error);
            }
            // Notifier le changement d'environnement
            NotifyEnvironmentChanged();

        }
    }
}
