using NAudio.Wave;
using OOBEMusic;
using System;
using System.Diagnostics;
using System.IO;
using System.Resources;

public class NAudioClass : IAudioPlayer
{
    private static readonly ResourceManager rm = new ResourceManager("OOBEMusic.Ressources.Messages", typeof(OOBEMusicPlayer).Assembly);
    private WaveOutEvent outputDevice;
    private AudioFileReader audioFile;
    private bool disposed = false;

    private AudioDeviceWatcher deviceWatcher;

    public NAudioClass()
    {
        deviceWatcher = new AudioDeviceWatcher();
        deviceWatcher.DefaultAudioDeviceChanged += OnAudioDeviceChanged;
    }

    private void OnAudioDeviceChanged()
    {
        // Log the event of audio device change
        outputDevice?.Stop();
        outputDevice?.Play();
        Logging.EventLogger.LogToEventViewer(rm.GetString("AudioDeviceChanged"), EventLogEntryType.Information);
    }

    /// <summary>
    /// Joue un fichier audio en boucle.
    /// </summary>
    /// <param name="filePath">Chemin du fichier audio à lire.</param>
    public void PlaySound(string filePath)
    {
        if (disposed)
        {
            throw new ObjectDisposedException(nameof(NAudioClass), rm.GetString("ObjectDisposedNAudio"));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(rm.GetString("FileDosentExist"), filePath);
        }

        try
        {
            // Initialiser les objets nécessaires pour la lecture
            audioFile = new AudioFileReader(filePath);
            LoopStream loopStream = new LoopStream(audioFile); // Lecture en boucle
            outputDevice = new WaveOutEvent();

            // Configurer et démarrer la lecture
            outputDevice.Init(loopStream);
            outputDevice.Play();
        }
        catch (Exception ex)
        {
            // Journaliser l'erreur et arrêter la lecture
            Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("MusicError"), ex.Message), EventLogEntryType.Error);
            StopAudioPlayback();
            throw; // Relancer l'exception pour que l'appelant puisse la gérer
        }
    }

    public void PlaySound(MemoryStream memoryStream)
    {
        throw new NotImplementedException("Not implemented.");
    }

    /// <summary>
    /// Arrête la lecture audio et libère les ressources associées.
    /// </summary>
    public void StopAudioPlayback()
    {
        try
        {
            outputDevice?.Stop();
        }
        catch (Exception ex)
        {
            Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("ErrorStoppingAudioPlaybackNAudio"), ex.Message), EventLogEntryType.Error);
        }
        finally
        {
            // Libérer les ressources
            if (outputDevice != null)
            {
                outputDevice.Dispose();
                outputDevice = null;
                Logging.EventLogger.LogToEventViewer(rm.GetString("OutputDeviceResourceReleased"), EventLogEntryType.Information);
            }

            if (audioFile != null)
            {
                audioFile.Dispose();
                audioFile = null;
                Logging.EventLogger.LogToEventViewer(rm.GetString("AudioFileResourceReleased"), EventLogEntryType.Information);
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
            Logging.EventLogger.LogToEventViewer(rm.GetString("NAudioResourceReleased"), EventLogEntryType.Information);
        }
    }

    /// <summary>
    /// Finaliseur pour garantir la libération des ressources non managées.
    /// </summary>
    ~NAudioClass()
    {
        Dispose(false);
    }
}
