using NAudio.Wave;
using System;
using System.Diagnostics;
using System.IO;
using System.Resources;

public class NAudioClass : IAudioPlayer
{
    private static readonly ResourceManager rm = new ResourceManager("AudioPlaybackLibrary.Ressources.Messages", typeof(NAudioClass).Assembly);
    private WaveOutEvent outputDevice;
    private AudioFileReader audioFile;
    private WaveFileReader waveFileReader;
    private MemoryStream currentStream;
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
        Logging.EventLogger.LogToEventViewer("The audio device has been changed.", EventLogEntryType.Information);
    }

    /// <summary>
    /// Joue un fichier audio en boucle.
    /// </summary>
    /// <param name="filePath">Chemin du fichier audio à lire.</param>
    public void PlaySound(string filePath)
    {
        if (disposed)
        {
            throw new ObjectDisposedException(nameof(NAudioClass), "The NAudioClass object has been disposed.");
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("The specified audio file does not exist.", filePath);
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
            Logging.EventLogger.LogToEventViewer($"An error occurred while playing the audio: {ex.Message}", EventLogEntryType.Error);
            StopAudioPlayback();
            throw; // Relancer l'exception pour que l'appelant puisse la gérer
        }
    }
    /// <summary>
    /// Joue un fichier audio à partir d'un MemoryStream.
    /// </summary>
    /// <param name="memoryStream"></param>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public void PlaySound(MemoryStream memoryStream)
    {
        if (disposed)
            throw new ObjectDisposedException(nameof(NAudioClass), "The NAudioClass object has been disposed.");
        if (memoryStream == null || memoryStream.Length == 0)
            throw new ArgumentException("The memory stream cannot be null or empty.", nameof(memoryStream));

        try
        {
            StopAudioPlayback(); // Toujours arrêter/disposer avant de relancer

            memoryStream.Position = 0;
            currentStream = memoryStream; // Garde le stream vivant tant que la lecture n'est pas terminée
            waveFileReader = new WaveFileReader(currentStream);
            LoopStream loopStream = new LoopStream(waveFileReader);
            outputDevice = new WaveOutEvent();
            outputDevice.Init(loopStream);
            outputDevice.Play();
        }
        catch (Exception ex)
        {
            Logging.EventLogger.LogToEventViewer($"An error occurred while playing the audio: {ex.Message}", EventLogEntryType.Error);
            StopAudioPlayback();
            throw;
        }
    }

    /// <summary>
    /// Arrête la lecture audio et libère les ressources.
    /// </summary>
    public void StopAudioPlayback()
    {
        try
        {
            outputDevice?.Stop();
        }
        catch (Exception ex)
        {
            Logging.EventLogger.LogToEventViewer($"An error occurred while stopping audio playback: {ex.Message}", EventLogEntryType.Error);
        }
        finally
        {
            outputDevice?.Dispose();
            outputDevice = null;

            audioFile?.Dispose();
            audioFile = null;

            waveFileReader?.Dispose();
            waveFileReader = null;

            currentStream?.Dispose();
            currentStream = null;
        }
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
            Logging.EventLogger.LogToEventViewer("The NAudioClass resources have been released.", EventLogEntryType.Information);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finaliseur pour garantir la libération des ressources non managées.
    /// </summary>
    ~NAudioClass()
    {
        Dispose(false);
    }
}
