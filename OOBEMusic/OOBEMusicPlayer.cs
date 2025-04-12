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

        // Fix for CS1519, CS1001, CS0106, CS1520, IDE1007: Move culture initialization to the constructor
        public OOBEMusicPlayer() // Injectez le logger dans le constructeur
        {
            this.ServiceName = servicename;

            InitializeComponent();
        }

        static string WWAHostKeyName = "ActivateWWAHostMusic";
        static string FirstLogonAnimKeyName = "ActivateFirstLogonMusic";
        static string OOBEHostAppKeyName = "ActivateOOBEHostMusic";
        static string SuperVerboseLogsKeyName = "EnableSuperVerboseLogs";
        static int SuperVerboseLogs = 0;

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
            int WWAHostState = RegHelper.CheckActivationState(WWAHostKeyName);
            int FirstLogonState = RegHelper.CheckActivationState(FirstLogonAnimKeyName);
            int OOBEHostAppState = RegHelper.CheckActivationState(OOBEHostAppKeyName);
            SuperVerboseLogs = RegHelper.CheckActivationState(SuperVerboseLogsKeyName, 0);

            if (WWAHostState != 1 && FirstLogonState != 1 && OOBEHostAppState != 1)
            {
                Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("ServiceStoppedBothKeysInactive"), WWAHostKeyName, FirstLogonAnimKeyName), EventLogEntryType.Information);
                service.Stop();
            }
            else
            {
                Thread Hook = new Thread(() => HookIntoWWA());
                Hook.Start();
            }
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string musicPath = appPath + "music.wav";
            RegHelper.SaveKey(musicPath);
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

        static void HookIntoWWA()
        {
            bool musicPlaying = false;
            var player = new SoundPlayer();
            string musicPath;

            // Check if the registry keys are set to 1

            int WWAHostActivate = RegHelper.CheckActivationState(WWAHostKeyName, default, false);
            int FirstLogonAnimActivate = RegHelper.CheckActivationState(FirstLogonAnimKeyName, default, false);
            int OOBEShellActivate = RegHelper.CheckActivationState(OOBEHostAppKeyName, default, false);

            while (true)
            {
                if (Checkifprocessexists(WWAHostActivate, FirstLogonAnimActivate, OOBEShellActivate) && !musicPlaying)
                {
                    PlaySound(player, true, out musicPlaying, out musicPath);

                    if (SuperVerboseLogs == 1)
                    {
                        Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("MusicPlayedVerbose"), musicPath), EventLogEntryType.Information);
                    }
                    else
                    {
                        Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("MusicPlayed"), musicPath), EventLogEntryType.Information);
                    }


                }
                else if (!Checkifprocessexists(WWAHostActivate, FirstLogonAnimActivate, OOBEShellActivate) && musicPlaying)
                {
                    if (SuperVerboseLogs == 1)
                    {
                        Logging.EventLogger.LogToEventViewer(rm.GetString("MusicStoppedVerbose"), EventLogEntryType.Information);
                    }
                    else
                    {
                        Logging.EventLogger.LogToEventViewer(rm.GetString("MusicStopped"), EventLogEntryType.Information);

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

        static bool Checkifprocessexists(int WWAHostSetting, int FirstLogonSetting, int OobeShellSetting)
        {
            string wwahost = "WWAHost";
            string firstbootanim = "FirstLogonAnim";
            string oobehost = "OobeShellHost";

            Process[] wwahostlist = Process.GetProcessesByName(wwahost);
            Process[] firstlogonlist = Process.GetProcessesByName(firstbootanim);
            Process[] oobehostlist = Process.GetProcessesByName(oobehost);

            bool exists = false;

            if (wwahostlist.Length > 0 && WWAHostSetting == 1)
            {

                bool interrupted = CheckProcesses(wwahostlist);

                if (!interrupted)
                {
                    exists = true;
                }
            }

            if (firstlogonlist.Length > 0 && FirstLogonSetting == 1)
            {

                bool interrupted = CheckProcesses(firstlogonlist);

                if (!interrupted)
                {
                    exists = true;
                }

            }

            if (oobehostlist.Length > 0 && OobeShellSetting == 1)
            {
                bool interrupted = CheckProcesses(oobehostlist);
                if (!interrupted)
                {
                    exists = true;
                }
            }

            return exists;
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
            if (SuperVerboseLogs == 1)
            {
                Logging.EventLogger.LogToEventViewer(rm.GetString("ServiceStopRequestVerbose"), EventLogEntryType.Information);
            }
            else
            {
                Logging.EventLogger.LogToEventViewer(rm.GetString("ServiceStopRequest"), EventLogEntryType.Information);
            }

            service.Stop();
        }
    }
}
