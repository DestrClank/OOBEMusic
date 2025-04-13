using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Resources;
using System.Globalization;

namespace OOBEMusic
{
    public partial class OOBEMusicPlayer : ServiceBase, IHostedService
    {
        private static readonly ResourceManager rm = new ResourceManager("OOBEMusic.Ressources.Messages", typeof(OOBEMusicPlayer).Assembly);

        static string servicename = "OOBEMusic";

        public bool _stopRequested = false;

        private SoundPlayer player;

        private Thread _HookThread = null; // Thread for hooking into WWAHost process

        static string WWAHostKeyName = "ActivateWWAHostMusic";
        static string FirstLogonAnimKeyName = "ActivateFirstLogonMusic";
        static string OOBEHostAppKeyName = "ActivateOOBEHostMusic";
        static string SuperVerboseLogsKeyName = "EnableSuperVerboseLogs";
        static int SuperVerboseLogs = 0;

        static int WWAHostState = RegHelper.CheckActivationState(WWAHostKeyName);
        static int FirstLogonAnimState = RegHelper.CheckActivationState(FirstLogonAnimKeyName);
        static int OOBEShellState = RegHelper.CheckActivationState(OOBEHostAppKeyName);

        public OOBEMusicPlayer()
        {
            this.ServiceName = servicename;

            InitializeComponent();
        }

        ServiceBase service = new ServiceBase();

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

        protected override void OnStart(string[] args)
        {
            SuperVerboseLogs = RegHelper.CheckActivationState(SuperVerboseLogsKeyName, 0);

            if (WWAHostState != 1 && FirstLogonAnimState != 1 && OOBEShellState != 1)
            {
                Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("ServiceStoppedBothKeysInactive"), WWAHostKeyName, FirstLogonAnimKeyName, OOBEHostAppKeyName), EventLogEntryType.Information);
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

            // Check if the registry keys are set to 1

            player = new SoundPlayer();

            while (!_stopRequested)
            {
                // Check if the processes are running and play the sound if they are
                var (processeslist, exists) = Checkifprocessexists(WWAHostState, FirstLogonAnimState, OOBEShellState);
                if (exists && !musicPlaying)
                {
                    PlaySound(player, true, out musicPlaying, out musicPath);

                    if (processeslist.Length > 0)
                    {
                        foreach (string process in processeslist)
                        {
                            processLogsStrings += process + ",";
                        }
                        processLogsStrings = processLogsStrings.TrimEnd(',');
                    } else
                    {
                        processLogsStrings = processeslist[0];
                    }

                    if (SuperVerboseLogs == 1)
                    {
                        Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("MusicPlayedVerbose"), processLogsStrings, musicPath), EventLogEntryType.Information);
                    }
                    else
                    {
                        Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("MusicPlayed"), processLogsStrings, musicPath), EventLogEntryType.Information);
                    }


                }
                else if (!exists && musicPlaying)
                {

                    if (processeslist.Length > 0)
                    {
                        foreach (string process in processeslist)
                        {
                            processLogsStrings += process + ",";
                        }
                        processLogsStrings = processLogsStrings.TrimEnd(',');
                    }
                    else
                    {
                        processLogsStrings = processeslist[0];
                    }

                    if (SuperVerboseLogs == 1)
                    {
                        Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("MusicStoppedVerbose"), processLogsStrings), EventLogEntryType.Information);
                    }
                    else
                    {
                        Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("MusicStopped"), processLogsStrings), EventLogEntryType.Information);

                    }

                    musicPlaying = false;
                    player.Stop();
                }

                Thread.Sleep(1000);
            }
        }

        static private void PlaySound(SoundPlayer soundplayer, bool musicPlaying, out bool Playing, out string musicPath)
        {
            Playing = musicPlaying;
            musicPath = RegHelper.GetValue("MusicFile");

            try
            {
                soundplayer.SoundLocation = musicPath;
                soundplayer.PlayLooping();
            }
            catch (Exception ex)
            {
                if (SuperVerboseLogs == 1)
                {
                    Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("MusicErrorVerbose"), ex.Message), EventLogEntryType.Error);
                }
                else
                {
                    Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("MusicError"), ex.Message), EventLogEntryType.Error);
                }
            }
        }

        private (string[] processeslist, bool exists) Checkifprocessexists(int WWAHostSetting, int FirstLogonSetting, int OobeShellSetting)
        {
            string wwahost = "WWAHost";
            string firstbootanim = "FirstLogonAnim";
            string oobehost = "OobeShellHost";

            Process[] wwahostlist = Process.GetProcessesByName(wwahost);
            Process[] firstlogonlist = Process.GetProcessesByName(firstbootanim);
            Process[] oobehostlist = Process.GetProcessesByName(oobehost);

            string[] processeslists = new string[3];

            bool exists = false;

            if (wwahostlist.Length > 0 && WWAHostSetting == 1)
            {

                bool interrupted = CheckProcesses(wwahostlist);

                if (!interrupted)
                {
                    processeslists[0] = wwahostlist[0].ProcessName;
                    exists = true;
                }
            }

            if (firstlogonlist.Length > 0 && FirstLogonSetting == 1)
            {

                bool interrupted = CheckProcesses(firstlogonlist);

                if (!interrupted)
                {
                    processeslists[1] = firstlogonlist[0].ProcessName;
                    exists = true;
                }

            }

            if (oobehostlist.Length > 0 && OobeShellSetting == 1)
            {
                bool interrupted = CheckProcesses(oobehostlist);
                if (!interrupted)
                {
                    processeslists[2] = oobehostlist[0].ProcessName;
                    exists = true;
                }
            }

            return (processeslists, exists);
        }

        static bool CheckProcesses(Process[] processes)
        {
            foreach (Process process in processes)
            {
                bool isSuspended = process.Threads.Cast<ProcessThread>()
                    .Any(thread => thread.ThreadState == System.Diagnostics.ThreadState.Wait && thread.WaitReason == ThreadWaitReason.Suspended);
                if (!isSuspended)
                {
                    return false; // Si un processus n'est pas suspendu, retourne false
                } //
            }
            return true; // Si tous les processus sont suspendus, retourne true
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

            player?.Stop();
            player?.Dispose();

            base.OnStop();
        }
    }
}
