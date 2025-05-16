using System;
using System.Diagnostics;
using System.IO;
using System.Media;

public class SoundPlayerClass : IAudioPlayer
{
    private SoundPlayer player;
    private AudioDeviceWatcher deviceWatcher;
    private string music = string.Empty;
    private bool disposed = false;
    private MemoryStream currentStream;

    public SoundPlayerClass()
    {
        deviceWatcher = new AudioDeviceWatcher();
        deviceWatcher.DefaultAudioDeviceChanged += OnAudioDeviceChanged;
    }

    private void OnAudioDeviceChanged()
    {
        // Log the event of audio device change
        player?.Stop();
        player?.PlaySync();
        Logging.EventLogger.LogToEventViewer("The audio device has been changed.", EventLogEntryType.Information);
    }

    public void PlaySound(string musicPath)
    {
        if (disposed)
        {
            throw new ObjectDisposedException(nameof(SoundPlayerClass), "The SoundPlayerClass instance has been disposed.");
        }

        if (string.IsNullOrWhiteSpace(musicPath))
        {
            throw new ArgumentException("The audio path cannot be null or empty.", nameof(musicPath));
        }

        try
        {
            music = musicPath;
            player = new SoundPlayer
            {
                SoundLocation = musicPath
            };

            player.Play();

        }
        catch (Exception ex)
        {
            Logging.EventLogger.LogToEventViewer($"An error occurred while playing the audio: {ex.Message}", EventLogEntryType.Error);
            StopAudioPlayback();
            throw;
        }
    }

    public void PlaySound(MemoryStream memoryStream)
    {
        if (disposed)
        {
            throw new ObjectDisposedException(nameof(SoundPlayerClass), "The SoundPlayerClass instance has been disposed.");
        }

        if (memoryStream == null)
        {
            throw new ArgumentNullException(nameof(memoryStream), "The memory stream cannot be null.");
        }

        try
        {
            currentStream = memoryStream; // Garde le stream vivant
            player = new SoundPlayer(memoryStream);
            player.Play();
        }
        catch (Exception ex)
        {
            Logging.EventLogger.LogToEventViewer($"An error occurred while playing the audio: {ex.Message}", EventLogEntryType.Error);
            StopAudioPlayback();
            throw;
        }
    }

    public void StopAudioPlayback()
    {
        try
        {
            player?.Stop();
        }
        catch (Exception ex)
        {
            Logging.EventLogger.LogToEventViewer($"Error stopping audio playback: {ex.Message}", EventLogEntryType.Error);
        }
        finally
        {
            player?.Dispose();
            player = null;
            currentStream?.Dispose();
            currentStream = null;
        }
    }


    public void Dispose()
    {
        Dispose(true);
        deviceWatcher?.Dispose();
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                StopAudioPlayback();
            }

            disposed = true;
            Logging.EventLogger.LogToEventViewer("The SoundPlayerClass instance has been disposed.", EventLogEntryType.Information);
        }
    }

    ~SoundPlayerClass()
    {
        Dispose(false);
    }
}
