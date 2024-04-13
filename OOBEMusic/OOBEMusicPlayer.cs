using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32;

namespace OOBEMusic
{
    public partial class OOBEMusicPlayer : ServiceBase, IHostedService
    {
        private readonly ILogger<OOBEMusicPlayer> _logger; // Ajoutez un champ pour le logger

        public OOBEMusicPlayer(ILogger<OOBEMusicPlayer> logger) // Injectez le logger dans le constructeur
        {
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
            Thread Hook = new Thread(() => HookIntoWWA(_logger));
            Hook.Start();
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string musicPath = appPath + "music.wav";
            RegHelper.SaveKey(musicPath,_logger);      
            _logger.LogInformation("Ta mere la pute la salope. \nEn language sérieux : Le service OOBEMusic.exe a été lancé avec succès.");
        }

        static void HookIntoWWA(ILogger logger)
        {
            bool musicPlaying = false;
            string musicPath = RegHelper.GetValue("MusicFile");
            var player = new SoundPlayer(musicPath);
            while (true)
            { 
                if (Checkifprocessexists() && !musicPlaying)
                {
                    musicPlaying = PlaySound(player, logger, true);
                    logger.LogInformation($"La musique a été jouée, normalement OnO.\nEn language sérieux : Le processus WWAHost.exe a été détécté et le programme lance la musique.\nEmplacement du fichier : {musicPath}");

                } else if (!Checkifprocessexists() && musicPlaying)
                {
                    logger.LogInformation("La musique s'est arrêté, normalement WALLAH c'est arrêté.\nEn language sérieux : Le processus WWAHost.exe a été fermé, la musique va s'arrêter.");
                    musicPlaying = false;
                    player.Stop();
                }

                Thread.Sleep(1000);

            }

        }

        static private bool PlaySound(SoundPlayer soundplayer, ILogger logger, bool musicPlaying)
        {
            try
            {
                soundplayer.PlayLooping();
            }
            catch (Exception ex)
            {
                logger.LogError($"TA GRAND MERE LA REINE DES PUTES TU T4APPELLES INES OU QUOIII TA DARONNE SUCE DES BITES : {ex.Message} \nEn language sérieux : Une erreur est survenue dans l'application empêchant la lecture de la musique, vérifiez que le fichier est bien un fichier .wav et que l'emplacement indiqué dans le registre Windows à la clé HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\DestrClank\\OOBEMusic\\MusicFile indique un emplacement de fichier valide à cette adresse et que le fichier existe et est accessible. \nCode d'erreur : {ex.Message}");
            }
            return musicPlaying;
        }



        static bool Checkifprocessexists()
        {
            string wwahost = "WWAHost";
            string firstbootanim = "FirstLogonAnim";

            Process[] wwahostlist = Process.GetProcessesByName(wwahost);
            Process[] firstlogonlist = Process.GetProcessesByName(firstbootanim);

            bool exists = false;

            if (wwahostlist.Length > 0)
            {
                exists = true;
            }

            if (firstlogonlist.Length > 0)
            {
                exists = true;
            }

            return exists;

        }

        protected override void OnStop()
        {
            _logger.LogWarning("POURQUOI TU VEUX ME TUERR WSHH T PAS GENTIL FRERO §§§§ :((((((( JE VAIS CHIALLERRR JE VEUX PAS MOURRRIRRRRRR NOOOOOOOOOOOOOOOOOOOOOOONNNNNNNNNNNNNNNNNNNNNNNNNNNN \nEn language sérieux : Le processus a recu une demande d'arrêt du système. Le processus va s'arrêter.");
            Process.GetCurrentProcess().Kill();
        }
    }
}
