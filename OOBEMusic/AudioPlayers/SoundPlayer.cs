using OOBEMusic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Numerics;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

public class SoundPlayerClass : IAudioPlayer
{
    private static readonly ResourceManager rm = new ResourceManager("OOBEMusic.Ressources.Messages", typeof(OOBEMusicPlayer).Assembly);
    private SoundPlayer player;

    private AudioDeviceWatcher deviceWatcher;

    private string music = string.Empty;

    public SoundPlayerClass()
    {
        deviceWatcher = new AudioDeviceWatcher();
        deviceWatcher.DefaultAudioDeviceChanged += OnAudioDeviceChanged;
    }

    private void OnAudioDeviceChanged()
    {
        // Log the event of audio device change
        player?.Stop();
        player?.PlayLooping();
        Logging.EventLogger.LogToEventViewer(rm.GetString("AudioDeviceChanged"), EventLogEntryType.Information);
    }

    private bool disposed = false;

    /// <summary>
    /// Joue un fichier audio en boucle à partir d'un chemin de fichier.
    /// </summary>
    /// <param name="musicPath">Chemin du fichier audio à lire.</param>
    public void PlaySound(string musicPath)
    {
        if (disposed)
        {
            throw new ObjectDisposedException(nameof(SoundPlayerClass), rm.GetString("ObjectDisposedSoundPlayer"));
        }

        if (string.IsNullOrWhiteSpace(musicPath))
        {
            throw new ArgumentException(rm.GetString("AudioPathNullOrEmpty"), nameof(musicPath));
        }

        try
        {
            // Vérifier si le fichier existe
            music = musicPath;
            player = new SoundPlayer
            {
                SoundLocation = musicPath
            };
            player.PlayLooping();
        }
        catch (Exception ex)
        {
            // Journaliser l'erreur et arrêter la lecture
            Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("MusicError"), ex.Message), EventLogEntryType.Error);
            StopAudioPlayback();
            throw; // Relancer l'exception pour que l'appelant puisse la gérer
        }
    }

    /// <summary>
    /// Joue un fichier audio en boucle à partir d'un flux en mémoire.
    /// </summary>
    /// <param name="memoryStream">Flux en mémoire contenant les données audio.</param>
    public void PlaySound(MemoryStream memoryStream)
    {
        if (disposed)
        {
            throw new ObjectDisposedException(nameof(SoundPlayerClass), rm.GetString("ObjectDisposedSoundPlayer"));
        }

        if (memoryStream == null)
        {
            throw new ArgumentNullException(nameof(memoryStream), rm.GetString("MemoryStreamNull"));
        }

        try
        {
            player = new SoundPlayer(memoryStream);
            player.PlayLooping();
        }
        catch (Exception ex)
        {
            // Journaliser l'erreur et arrêter la lecture
            Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("MusicError"), ex.Message), EventLogEntryType.Error);
            StopAudioPlayback();
            throw; // Relancer l'exception pour que l'appelant puisse la gérer
        }
    }

    /// <summary>
    /// Arrête la lecture audio et libère les ressources associées.
    /// </summary>
    public void StopAudioPlayback()
    {
        try
        {
            player?.Stop();
        }
        catch (Exception ex)
        {
            Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("ErrorStoppingAudioPlayback"), ex.Message), EventLogEntryType.Error);
        }
        finally
        {
            if (player != null)
            {
                player.Dispose();
                player = null;
            }
        }
    }

    /// <summary>
    /// Libère les ressources utilisées par la classe.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        deviceWatcher.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Implémentation de la méthode Dispose pour libérer les ressources.
    /// </summary>
    /// <param name="disposing">Indique si les ressources managées doivent être libérées.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // Libérer les ressources managées
                StopAudioPlayback();
            }

            // Libérer les ressources non managées si nécessaire (aucune ici)
            disposed = true;
            Logging.EventLogger.LogToEventViewer(rm.GetString("SoundPlayerResourceReleased"), EventLogEntryType.Information);
        }
    }

    /// <summary>
    /// Finaliseur pour garantir la libération des ressources non managées.
    /// </summary>
    ~SoundPlayerClass()
    {
        Dispose(false);
    }
}
