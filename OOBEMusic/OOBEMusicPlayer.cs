using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace OOBEMusic
{
    public partial class OOBEMusicPlayer : ServiceBase, IHostedService
    {
        private readonly ILogger<OOBEMusicPlayer> _logger; // Ajoutez un champ pour le logger

        static string servicename = "OOBEMusic";

        static string WWAHostKeyName = "ActivateWWAHostMusic";
        static string FirstLogonAnimKeyName = "ActivateFirstLogonMusic";
        static string SuperVerboseLogsKeyName = "EnableSuperVerboseLogs";
        static int SuperVerboseLogs = 0;

        ServiceBase service = new ServiceBase();

        public OOBEMusicPlayer(ILogger<OOBEMusicPlayer> logger) // Injectez le logger dans le constructeur
        {
            this.ServiceName = servicename;

            InitializeComponent();
            _logger = logger;
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

        protected override void OnStart(string[] args)
        {
            int WWAHostState = RegHelper.CheckActivationState(WWAHostKeyName, _logger);
            int FirstLogonState = RegHelper.CheckActivationState(FirstLogonAnimKeyName, _logger);
            SuperVerboseLogs = RegHelper.CheckActivationState(SuperVerboseLogsKeyName, _logger, 0);

            if (WWAHostState != 1 && FirstLogonState != 1)
            {
                _logger.LogInformation($"Les deux valeurs {WWAHostKeyName} et {FirstLogonAnimKeyName} ne sont pas réglés sur 1. Le programme va s'arrêter.");
                service.Stop();
            } else
            {
                Thread Hook = new Thread(() => HookIntoWWA(_logger));
                Hook.Start();
            }
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string musicPath = appPath + "music.wav";
            RegHelper.SaveKey(musicPath, _logger);
            if (SuperVerboseLogs == 1)
            {
                _logger.LogInformation("uiiiii ca marcheeee \nEn language sérieux : Le service OOBEMusic.exe a été lancé avec succès.");
            } else
            {
                _logger.LogInformation("Le service OOBEMusic.exe a été lancé avec succès.");
            }

        }

        static void HookIntoWWA(ILogger logger)
        {
            bool musicPlaying = false;
            var player = new SoundPlayer();
            string musicPath;

            int WWAHostActivate = RegHelper.CheckActivationState(WWAHostKeyName, logger);
            int FirstLogonAnimActivate = RegHelper.CheckActivationState(FirstLogonAnimKeyName, logger);

            while (true)
            {
                if (Checkifprocessexists(WWAHostActivate, FirstLogonAnimActivate, logger) && !musicPlaying)
                {
                    PlaySound(player, logger, true, out musicPlaying, out musicPath);

                    if (SuperVerboseLogs == 1)
                    {
                        logger.LogInformation($"La musique a été jouée, normalement OnO.\nEn language sérieux : Le processus WWAHost.exe a été détécté et le programme lance la musique.\nEmplacement du fichier : {musicPath}");
                    } else
                    {
                        logger.LogInformation($"Le processus WWAHost.exe a été détécté et le programme lance la musique.\nEmplacement du fichier : {musicPath}");
                    }


                } else if (!Checkifprocessexists(WWAHostActivate, FirstLogonAnimActivate, logger) && musicPlaying)
                {
                    if (SuperVerboseLogs == 1)
                    {
                        logger.LogInformation("La musique s'est arrêté, normalement c'est arrêté.\nEn language sérieux : Le processus WWAHost.exe a été fermé, la musique va s'arrêter.");
                    } else
                    {
                        logger.LogInformation("Le processus WWAHost.exe a été fermé, la musique va s'arrêter.");
                    }

                    musicPlaying = false;
                    player.Stop();
                }

                Thread.Sleep(1000);

            }

        }

        static private void PlaySound(SoundPlayer soundplayer, ILogger logger, bool musicPlaying, out bool Playing, out string musicPath)
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
                    logger.LogError($"naaaahhhh ca marche pluuu: {ex.Message} \nEn language sérieux : Une erreur est survenue dans l'application empêchant la lecture de la musique, vérifiez que le fichier est bien un fichier .wav et que l'emplacement indiqué dans le registre Windows à la clé HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\DestrClank\\OOBEMusic\\MusicFile indique un emplacement de fichier valide à cette adresse et que le fichier existe et est accessible. \nCode d'erreur : {ex.Message}");
                } else
                {
                    logger.LogError($"Une erreur est survenue dans l'application empêchant la lecture de la musique, vérifiez que le fichier est bien un fichier .wav et que l'emplacement indiqué dans le registre Windows à la clé HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\DestrClank\\OOBEMusic\\MusicFile indique un emplacement de fichier valide à cette adresse et que le fichier existe et est accessible. \nCode d'erreur : {ex.Message}");
                }
            }
        }

        static bool Checkifprocessexists(int WWAHostSetting, int FirstLogonSetting, ILogger logger)
        {
            string wwahost = "WWAHost";
            string firstbootanim = "FirstLogonAnim";

            Process[] wwahostlist = Process.GetProcessesByName(wwahost);
            Process[] firstlogonlist = Process.GetProcessesByName(firstbootanim);

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
                _logger.LogWarning("JE VEUX PAS MOURRRIRRRRRR NOOOOOOOOOOOOOOOOOOOOOOONNNNNNNNNNNNNNNNNNNNNNNNNNNN \nEn language sérieux : Le processus a recu une demande d'arrêt du système. Le processus va s'arrêter.");
            } else
            {
                _logger.LogWarning("Le processus a recu une demande d'arrêt du système. Le processus va s'arrêter.");
            }
            
            service.Stop();
        }
    }
}
