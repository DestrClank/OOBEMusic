using Microsoft.Extensions.Hosting;
using OOBEMusic.Utils;
using System;
using System.Diagnostics;
using System.Resources;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using AudioPlaybackLibrary;

namespace OOBEMusic
{
    public partial class OOBEMusicPlayer : ServiceBase, IHostedService
    {
        private static readonly ResourceManager rm = new ResourceManager("OOBEMusic.Ressources.Messages", typeof(OOBEMusicPlayer).Assembly);

        static string servicename = "OOBEMusic";

        public bool _stopRequested = false;

        private AudioPlayer audioPlayer = new AudioPlayer(); // Instance de la classe AudioPlayer

        private bool enableConsoleWrite = false;

        ProcessUtils processUtils = new ProcessUtils(); // Instance de ProcessUtils pour la gestion des processus

        private Thread _HookThread = null; // Thread pour surveiller le processus WWAHost

        static int SuperVerboseLogs = 0;

        static int ThreadTimeout = RegHelper.CheckThreadTimeout(); // Temps de pause pour le thread en millisecondes

        public OOBEMusicPlayer()
        {
            this.ServiceName = servicename;

            InitializeComponent();

            // Initialiser l'instance de AudioPlayer
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            OnStart(new string[] { });
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            OnStop();
            return Task.CompletedTask;
        }

        public void RunAsConsole()
        {
            Console.WriteLine(rm.GetString("CannotRunInConsoleMode"));
            enableConsoleWrite = true;
            Logging.EventLogger.LogToEventViewer(rm.GetString("CannotRunInConsoleMode"), EventLogEntryType.Error);
        }

        protected override void OnStart(string[] args)
        {
            if (!processUtils.CheckIfAnyOptionIsEnabled())
            {
                string message = string.Format(rm.GetString("ServiceStoppedBothKeysInactive"), "ActivateWWAHostMusic", "ActivateFirstLogonMusic", "ActivateOOBEHostMusic");
                Logging.EventLogger.LogToEventViewer(message, EventLogEntryType.Information);
                if (enableConsoleWrite)
                    Console.WriteLine(message);
                base.Stop();
            }
            else
            {
                _stopRequested = false;
                _HookThread = new Thread(() => HookIntoWWA());
                _HookThread.Start();

                string appPath = AppDomain.CurrentDomain.BaseDirectory;
                string musicPath = appPath + "music.wav";
                RegHelper.SaveKey(musicPath);
            }

            if (SuperVerboseLogs == 1)
            {
                string message = rm.GetString("ServiceStartedVerbose");
                Logging.EventLogger.LogToEventViewer(message, EventLogEntryType.Information);
                if (enableConsoleWrite)
                    Console.WriteLine(message);
            }
            else
            {
                string message = rm.GetString("ServiceStarted");
                Logging.EventLogger.LogToEventViewer(message, EventLogEntryType.Information);
                if (enableConsoleWrite)
                    Console.WriteLine(message);
            }
        }

        private void HookIntoWWA()
        {
            bool musicPlaying = false;
            string musicPath;
            string processLogsStrings = string.Empty;

            try
            {
                while (!_stopRequested)
                {
                    var (processeslist, exists) = processUtils.CheckIfProcessExists();
                    if (exists && !musicPlaying)
                    {
                        musicPath = RegHelper.GetValue("MusicFile").Replace("\"", string.Empty);
                        musicPlaying = true;

                        try
                        {
                            audioPlayer.PlayInLoop(musicPath);
                            processLogsStrings = processUtils.ParseProcesses(processeslist);

                            string message = string.Format(rm.GetString("MusicPlayed"), processLogsStrings, musicPath);
                            Logging.EventLogger.LogToEventViewer(message, EventLogEntryType.Information);
                            if (enableConsoleWrite)
                                Console.WriteLine(message);
                        }
                        catch (Exception ex)
                        {
                            string message = string.Format(rm.GetString("MusicCrash"), ex.Message);
                            Logging.EventLogger.LogToEventViewer(message, EventLogEntryType.Error);
                            if (enableConsoleWrite)
                                Console.WriteLine(message);
                        }
                    }
                    else if (!exists && musicPlaying)
                    {
                        musicPlaying = false;

                        DisposeAudioPlayers();

                        string message;
                        if (SuperVerboseLogs == 1)
                        {
                            message = string.Format(rm.GetString("MusicStoppedVerbose"), processLogsStrings);
                        }
                        else
                        {
                            message = string.Format(rm.GetString("MusicStopped"), processLogsStrings);
                        }
                        Logging.EventLogger.LogToEventViewer(message, EventLogEntryType.Information);
                        if (enableConsoleWrite)
                            Console.WriteLine(message);
                    }

                    Thread.Sleep(ThreadTimeout);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format(rm.GetString("CriticalErrorInHookIntoWWA"), ex.Message);
                Logging.EventLogger.LogToEventViewer(message, EventLogEntryType.Error);
                if (enableConsoleWrite)
                    Console.WriteLine(message);
            }
            finally
            {
                DisposeAudioPlayers();
            }
        }

        private void DisposeAudioPlayers()
        {
            try
            {
                audioPlayer.Stop();
            }
            catch (Exception ex)
            {
                string message = string.Format(rm.GetString("ErrorReleasingAudioResources"), ex.Message);
                Logging.EventLogger.LogToEventViewer(message, EventLogEntryType.Error);
                if (enableConsoleWrite)
                    Console.WriteLine(message);
            }
        }

        protected override void OnStop()
        {
            _stopRequested = true;

            string message;
            if (SuperVerboseLogs == 1)
            {
                message = rm.GetString("ServiceStopRequestVerbose");
            }
            else
            {
                message = rm.GetString("ServiceStopRequest");
            }
            Logging.EventLogger.LogToEventViewer(message, EventLogEntryType.Information);
            if (enableConsoleWrite)
                Console.WriteLine(message);

            if (_HookThread != null && _HookThread.IsAlive)
            {
                _HookThread.Join();
            }

            DisposeAudioPlayers();

            base.OnStop();
        }
    }
}
