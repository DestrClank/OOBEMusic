using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Resources;
using System.IO;
using OOBEMusic.Utils;

namespace OOBEMusic
{
    public partial class OOBEMusicPlayer : ServiceBase, IHostedService
    {
        private static readonly ResourceManager rm = new ResourceManager("OOBEMusic.Ressources.Messages", typeof(OOBEMusicPlayer).Assembly);

        static string servicename = "OOBEMusic";

        public bool _stopRequested = false;

        // Audio API enum
        IAudioPlayer player;

        ProcessUtils processUtils = new ProcessUtils(); // Instance of ProcessUtils for process management

        private Thread _HookThread = null; // Thread for hooking into WWAHost process
   
        static int SuperVerboseLogs = 0;

        static int ThreadTimeout = RegHelper.CheckThreadTimeout(); // Sleep time for the thread in milliseconds

        public OOBEMusicPlayer()
        {
            this.ServiceName = servicename;

            InitializeComponent();
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
            Logging.EventLogger.LogToEventViewer(rm.GetString("CannotRunInConsoleMode"), EventLogEntryType.Error);
        }

        protected override void OnStart(string[] args)
        {

            if (!processUtils.CheckIfAnyOptionIsEnabled())
            {
                Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("ServiceStoppedBothKeysInactive"), "ActivateWWAHostMusic", "ActivateFirstLogonMusic", "ActivateOOBEHostMusic"), EventLogEntryType.Information);
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
                Logging.EventLogger.LogToEventViewer(rm.GetString("ServiceStartedVerbose"), EventLogEntryType.Information);
            }
            else
            {
                Logging.EventLogger.LogToEventViewer(rm.GetString("ServiceStarted"), EventLogEntryType.Information);
            }

            // Logique pour démarrer le service

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
                            PlayAudioFile(musicPath);
                            processLogsStrings = processUtils.ParseProcesses(processeslist);

                            Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("MusicPlayed"), processLogsStrings, musicPath), EventLogEntryType.Information);
                            
                        }
                        catch (Exception ex)
                        {
                            Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("MusicCrash"), ex.Message), EventLogEntryType.Error);
                        }
                    }
                    else if (!exists && musicPlaying)
                    {
                        musicPlaying = false;

                        DisposeAudioPlayers();

                        if (SuperVerboseLogs == 1)
                        {
                            Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("MusicStoppedVerbose"), processLogsStrings), EventLogEntryType.Information);
                        }
                        else
                        {
                            Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("MusicStopped"), processLogsStrings), EventLogEntryType.Information);
                        }
                    }

                    Thread.Sleep(ThreadTimeout);
                }
            }
            catch (Exception ex)
            {
                Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("CriticalErrorInHookIntoWWA"), ex.Message), EventLogEntryType.Error);

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
                player?.Dispose();
            }
            catch (Exception ex)
            {
                Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("ErrorReleasingAudioResources"), ex.Message), EventLogEntryType.Error);
            }
        }

        private void PlayAudioFile(string musicPath)
        {
            string extension = Path.GetExtension(musicPath).ToLowerInvariant();

            // Déterminer l'API audio en fonction de l'extension
            switch (extension)
            {
                case ".wav":
                    player = new SoundPlayerClass();
                    player.PlaySound(musicPath);
                    break;

                case ".brstm":
                    using (var brstmPlayer = new BrstmPlayer())
                    {
                        using (var memoryStream = brstmPlayer.OpenBrstm(musicPath))
                        {
                            player = new SoundPlayerClass();
                            player.PlaySound(memoryStream);
                        }
                    }
                    break;

                case ".at9":
                    using (var at9Player = new At9Player())
                    {
                        using (var memoryStream = at9Player.OpenAt9(musicPath))
                        {
                            player = new SoundPlayerClass();
                            player.PlaySound(memoryStream);
                        }
                    }
                    break;

                case ".mp3":
                    player = new NAudioClass();
                    player.PlaySound(musicPath);
                    break;

                case ".at3":
                    using (var vgmPlayer = new VGMStreamPlayer())
                    {
                        using (var memoryStream = vgmPlayer.Open(musicPath))
                        {
                            player = new SoundPlayerClass();
                            player.PlaySound(memoryStream);
                        }
                    }
                    break;

                default:
                    // Par défaut, utiliser NAudio pour les formats non pris en charge

                    using (var vgmPlayer = new VGMStreamPlayer())
                    {
                        using (var memoryStream = vgmPlayer.Open(musicPath))
                        {
                            player = new SoundPlayerClass();
                            player.PlaySound(memoryStream);
                        }
                    }
                    break;

     
            }
        }

        protected override void OnStop()
        {
            _stopRequested = true;

            if (SuperVerboseLogs == 1)
            {
                Logging.EventLogger.LogToEventViewer(rm.GetString("ServiceStopRequestVerbose"), EventLogEntryType.Information);
            }
            else
            {
                Logging.EventLogger.LogToEventViewer(rm.GetString("ServiceStopRequest"), EventLogEntryType.Information);
            }

            if (_HookThread != null && _HookThread.IsAlive)
            {
                _HookThread.Join();
            }

            DisposeAudioPlayers();

            base.OnStop();
        }
    }
}
