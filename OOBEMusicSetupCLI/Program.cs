using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using OOBEMusic;
using OOBEMusic.Utils;

namespace OOBEMusicSetupCLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string musicFile = null;
            bool? enableWwaHost = null;
            bool? enableOobeHost = null;
            bool? enableWinDeploy = null;
            bool? enableVerboseLogs = null;
            int? threadTimeout = null;
            bool? enableFirstLogonAnim = null;
            bool startService = false;
            bool restartService = false;
            bool stopService = false;
            bool enableService = false;
            bool disableService = false;
            bool testApp = false;
            bool reset = false;
            bool eventViewer = false;

            string servicename = "OOBEMusicService";
            ServiceControllerStatus serviceState = ServiceHelper.GetServiceStatus(servicename);

            if (args.Length == 0)
            {
                Console.Error.WriteLine("No arguments provided. Use /help for usage information.");
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i].ToLowerInvariant();
                switch (arg)
                {
                    case "/music-file":
                        if (i + 1 < args.Length)
                        {
                            musicFile = args[++i];
                        }
                        break;
                    case "/enable-wwahost":
                        if (i + 1 < args.Length && bool.TryParse(args[++i], out bool wwaHost))
                        {
                            enableWwaHost = wwaHost;
                        }
                        break;
                    case "/enable-oobehost":
                        if (i + 1 < args.Length && bool.TryParse(args[++i], out bool oobeHost))
                        {
                            enableOobeHost = oobeHost;
                        }
                        break;
                    case "/enable-windeploy":
                        if (i + 1 < args.Length && bool.TryParse(args[++i], out bool winDeploy))
                        {
                            enableWinDeploy = winDeploy;
                        }
                        break;
                    case "/enable-verbose-logs":
                        if (i + 1 < args.Length && bool.TryParse(args[++i], out bool verboseLogs))
                        {
                            enableVerboseLogs = verboseLogs;
                        }
                        break;
                    case "/thread-timeout":
                        if (i + 1 < args.Length && int.TryParse(args[++i], out int timeout) && timeout >= 100 && timeout <= 5000)
                        {
                            threadTimeout = timeout;
                        }
                        break;
                    case "/enable-firstlogonanim":
                        if (i + 1 < args.Length && bool.TryParse(args[++i], out bool firstLogonAnim))
                        {
                            enableFirstLogonAnim = firstLogonAnim;
                        }
                        break;
                    case "/start-service":
                        startService = true;
                        break;
                    case "/restart-service":
                        restartService = true;
                        break;
                    case "/stop-service":
                        stopService = true;
                        break;
                    case "/enable-service":
                        enableService = true;
                        break;
                    case "/disable-service":
                        disableService = true;
                        break;
                    case "/testapp":
                        testApp = true;
                        break;
                    case "/reset":
                        reset = true;
                        break;
                    case "/event-viewer":
                        eventViewer = true;
                        break;
                    case "/debug":
                        // Debug mode
                        break;
                    case "/verbose":
                        // Verbose mode
                        break;
                    case "/taskmgr":
                        // Open Task Manager
                        Process.Start("taskmgr.exe");
                        break;
                    case "/services":
                        // Open Services
                        Process.Start("services.msc");
                        break;
                    case "/regedit":
                        // Open Registry Editor
                        Process.Start("regedit.exe");
                        break;
                    case "/cmd":
                        // Open Command Prompt
                        Process.Start("cmd.exe");
                        break;
                    case "/powershell":
                        // Open PowerShell
                        Process.Start("powershell.exe");
                        break;
                    case "/powershell-ise":
                        // Open PowerShell ISE
                        Process.Start("powershell_ise.exe");
                        break;
                    case "/powershell-x86":
                        // Open PowerShell x86
                        Process.Start("C:\\Windows\\SysWOW64\\WindowsPowerShell\\v1.0\\powershell.exe");
                        break;
                    case "/powershell-x64":
                        // Open PowerShell x64
                        Process.Start("C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe");
                        break;
                    case "/powershell-x64-ise":
                        // Open PowerShell x64 ISE
                        Process.Start("C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell_ise.exe");
                        break;
                    case "/powershell-x86-ise":
                        // Open PowerShell x86 ISE
                        Process.Start("C:\\Windows\\SysWOW64\\WindowsPowerShell\\v1.0\\powershell_ise.exe");
                        break;
                    case "/taskkill":
                        // Kill a process
                        if (i + 1 < args.Length)
                        {
                            string processName = args[++i];
                            Process[] processes = Process.GetProcessesByName(processName);
                            foreach (Process process in processes)
                            {
                                process.Kill();
                            }
                        }
                        break;
                    case "/notepad":
                        // Open Notepad
                        Process.Start("notepad.exe");
                        break;
                    case "-h":
                    case "--help":
                    case "/help":
                        Console.WriteLine("Usage: oobesetup.exe [options]");
                        Console.WriteLine("Options:");
                        Console.WriteLine("/music-file <path> : Specify the path to the music file.");
                        Console.WriteLine("/enable-wwahost <true|false> : Enable or disable WWAHost music.");
                        Console.WriteLine("/enable-oobehost <true|false> : Enable or disable OOBEHost music.");
                        Console.WriteLine("/enable-windeploy <true|false> : Enable or disable WinDeploy music.");
                        Console.WriteLine("/enable-verbose-logs <true|false> : Enable or disable verbose logs.");
                        Console.WriteLine("/thread-timeout <milliseconds> : Set the thread timeout (100-5000 ms).");
                        Console.WriteLine("/enable-firstlogonanim <true|false> : Enable or disable first logon animation.");
                        Console.WriteLine("/start-service : Start the OOBEMusic service.");
                        Console.WriteLine("/restart-service : Restart the OOBEMusic service.");
                        Console.WriteLine("/stop-service : Stop the OOBEMusic service.");
                        Console.WriteLine("/enable-service : Enable the OOBEMusic service.");
                        Console.WriteLine("/disable-service : Disable the OOBEMusic service.");
                        Console.WriteLine("/testapp : Launch WWAHost.exe for testing.");
                        Console.WriteLine("/reset : Reset registry keys to default values.");
                        Console.WriteLine("/event-viewer : Open Event Viewer.");
                        Console.WriteLine("/all-help : Show all help options.");
                        return;
                    case "/version":
                        Console.WriteLine("OOBEMusicSetup CLI Version 1.0.5");
                        return;
                    case "/about":
                        Console.WriteLine("OOBEMusicSetup CLI - A command line interface for OOBEMusic.");
                        Console.WriteLine("Version 1.0.5");
                        Console.WriteLine("Developed by DestrClank Studios.");
                        return;
                    case "/all-help":
                        Console.WriteLine("All help options:");
                        Console.WriteLine("/help : Show this help message.");
                        Console.WriteLine("/version : Show the version of the application.");
                        Console.WriteLine("/about : Show information about the application.");
                        Console.WriteLine("/taskmgr : Open Task Manager.");
                        Console.WriteLine("/services : Open Services.");
                        Console.WriteLine("/regedit : Open Registry Editor.");
                        Console.WriteLine("/cmd : Open Command Prompt.");
                        Console.WriteLine("/notepad : Open Notepad.");
                        Console.WriteLine("/taskkill <process_name> : Kill a process by name.");
                        Console.WriteLine("/powershell : Open PowerShell.");
                        Console.WriteLine("/powershell-ise : Open PowerShell ISE.");
                        Console.WriteLine("/powershell-x86 : Open PowerShell x86.");
                        Console.WriteLine("/powershell-x64 : Open PowerShell x64.");
                        Console.WriteLine("/powershell-x64-ise : Open PowerShell x64 ISE.");
                        Console.WriteLine("/powershell-x86-ise : Open PowerShell x86 ISE.");
                        return;
                    default:
                        Console.Error.WriteLine($"Unknown argument: {arg}");
                        break;
                }
            }
            try
            {
                // Implémentation de la logique réelle avec OOBEMusic
                if (musicFile != null)
                {
                    RegHelper.WriteKey("MusicFile", musicFile);
                }

                if (enableWwaHost.HasValue)
                {
                    RegHelper.WriteKey("EnableWwaHost", enableWwaHost.Value ? 1 : 0);
                }

                if (enableOobeHost.HasValue)
                {
                    RegHelper.WriteKey("EnableOobeHost", enableOobeHost.Value ? 1 : 0);
                }

                if (enableWinDeploy.HasValue)
                {
                    RegHelper.WriteKey("EnableWinDeploy", enableWinDeploy.Value ? 1 : 0);
                }

                if (enableVerboseLogs.HasValue)
                {
                    RegHelper.WriteKey("EnableVerboseLogs", enableVerboseLogs.Value ? 1 : 0);
                }

                if (threadTimeout.HasValue)
                {
                    RegHelper.WriteKey("ThreadTimeout", threadTimeout.Value);
                }

                if (enableFirstLogonAnim.HasValue)
                {
                    RegHelper.WriteKey("EnableFirstLogonAnim", enableFirstLogonAnim.Value ? 1 : 0);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error writing registry keys: {ex.Message}");
                return;
            }

            if (startService)
            {
                if (CheckIfEverythingDisabled())
                {
                    Console.Error.WriteLine("The service won't start because every music playback option is disabled, enable at least one of them and try again.");
                    return;
                }

                if (ServiceHelper.IsServiceDisabled(servicename))
                {
                    Console.Error.WriteLine("The service is disabled, please enable it first.");
                    return;
                }

                serviceState = ServiceHelper.GetServiceStatus(servicename);

                if (serviceState == ServiceControllerStatus.Running)
                {
                    Console.WriteLine("Service is already running.");
                }
                else
                {
                    ServiceHelper.StartService(servicename);
                }
            }
            if (restartService)
            {
                if (CheckIfEverythingDisabled())
                {
                    Console.Error.WriteLine("The service won't restart because every music playback option is disabled, enable at least one of them and try again.");
                    return;
                }

                serviceState = ServiceHelper.GetServiceStatus(servicename);
                if (serviceState == ServiceControllerStatus.Running)
                {
                    ServiceHelper.RestartService(servicename);

                    // Wait for the service to restart  
                    var stopwatch = Stopwatch.StartNew();
                    while (ServiceHelper.GetServiceStatus(servicename) != ServiceControllerStatus.Running)
                    {
                        if (stopwatch.ElapsedMilliseconds > 5000)
                        {
                            Console.Error.WriteLine("Le service n'a pas pu redémarrer dans les 5 secondes.");
                            return;
                        }
                        System.Threading.Thread.Sleep(100);
                    }
                }
                else
                {
                    Console.Error.WriteLine("Service is not running.");
                }
            }
            if (stopService)
            {
                serviceState = ServiceHelper.GetServiceStatus(servicename);
                if (serviceState == ServiceControllerStatus.Stopped)
                {
                    Console.WriteLine("Service is already stopped.");
                }
                else
                {

                    ServiceHelper.StopService(servicename);

                    // Wait for the service to stop
                    var stopwatch = Stopwatch.StartNew();
                    while (ServiceHelper.GetServiceStatus(servicename) != ServiceControllerStatus.Stopped)
                    {
                        if (stopwatch.ElapsedMilliseconds > 5000)
                        {
                            Console.Error.WriteLine("Le service n'a pas pu s'arrêter dans les 5 secondes.");
                            return;
                        }
                        System.Threading.Thread.Sleep(100);
                    }


                }
            }
            if (enableService)
            {
                try
                {
                    ServiceHelper.EnableService(servicename);
                    Console.WriteLine("Service enabled successfully.");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error enabling service: " + ex.Message);
                }
            }
            if (disableService)
            {
                try
                {
                    ServiceHelper.DisableService(servicename);
                    Console.WriteLine("Service disabled successfully.");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error disabling service: " + ex.Message);
                    return;
                }
            }
            if (testApp)
            {
                Process.Start("WWAHost.exe");
            }
            if (reset)
            {
                try
                {
                    RegHelper.ResetRegistryKeys();
                    // Reset the checkboxes to their default state

                    if (serviceState == ServiceControllerStatus.Running)
                    {
                        ServiceHelper.RestartService(servicename);

                        // Wait for the service to restart  
                        var stopwatch = Stopwatch.StartNew();
                        while (ServiceHelper.GetServiceStatus(servicename) != ServiceControllerStatus.Running)
                        {
                            if (stopwatch.ElapsedMilliseconds > 5000)
                            {
                                Console.Error.WriteLine("The service failed to restart in a timely fashion.");
                                return;
                            }
                            System.Threading.Thread.Sleep(100);
                        }
                    }

                    Console.WriteLine("Registry keys reset successfully.");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error resetting registry keys: {ex.Message}");
                }
            }
            if (eventViewer)
            {
                Process.Start("eventvwr.msc");
            }
        }

        private static Dictionary<string, int> LoadProcesses()
        {
            var dynamicProcesses = new Dictionary<string, int>();

            // Exemple : Charger les données depuis le registre
            var keys = new List<string> { "ActivateWWAHostMusic", "ActivateOOBEHostMusic", "ActivateFirstLogonMusic", "ActivateWinDeployMusic" };
            foreach (var key in keys)
            {
                int state = RegHelper.CheckActivationState(key);
                dynamicProcesses[key] = state;
            }

            return dynamicProcesses;
        }

        private static bool CheckIfEverythingDisabled()
        {
            Dictionary<string, int> processesp = LoadProcesses();
            foreach (var process in processesp)
            {
                if (process.Value == 1)
                {
                    return false; // At least one process is enabled
                }
            }
            return true; // All processes are disabled
        }

    }
}
