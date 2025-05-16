using CSCore;
using CSCore.Codecs;
using CSCore.Codecs.WAV;
using CSCore.SoundOut;
using System;
using System.IO;

public class CSCoreClass : IAudioPlayer
{
    private ISoundOut soundOut;
    private IWaveSource waveSource;
    private bool disposed = false;
    public event EventHandler PlaybackStopped;
    private MemoryStream currentStream;

    public void PlaySound(string filePath)
    {
        if (disposed)
            throw new ObjectDisposedException(nameof(CSCoreClass));

        StopAudioPlayback();

        waveSource = CodecFactory.Instance.GetCodec(filePath)
            .ToSampleSource()
            .ToWaveSource();

        soundOut = new WasapiOut();
        soundOut.Initialize(waveSource);
        soundOut.Play();
        soundOut.Stopped += OnPlaybackStopped;
    }

    private void OnPlaybackStopped(object sender, PlaybackStoppedEventArgs e)
    {
        PlaybackStopped?.Invoke(this, EventArgs.Empty);

        // Utilise le SynchronizationContext du thread UI si disponible
        var syncContext = System.Threading.SynchronizationContext.Current;
        if (syncContext != null)
        {
            syncContext.Post(_ => StopAudioPlayback(), null);
        }
        else
        {
            // Sinon, fallback sur ThreadPool
            System.Threading.ThreadPool.QueueUserWorkItem(_ => StopAudioPlayback());
        }
    }

    public void PlaySound(MemoryStream memoryStream)
    {
        if (disposed)
            throw new ObjectDisposedException(nameof(CSCoreClass));
        if (memoryStream == null || memoryStream.Length == 0)
            throw new ArgumentException("The memory stream cannot be null or empty.", nameof(memoryStream));

        StopAudioPlayback();

        currentStream = memoryStream;
        currentStream.Position = 0;

        waveSource = new WaveFileReader(currentStream)
            .ToSampleSource()
            .ToWaveSource();

        soundOut = new WasapiOut();
        soundOut.Initialize(waveSource);
        soundOut.Play();
        soundOut.Stopped += OnPlaybackStopped;
    }

    public void StopAudioPlayback()
    {
        try
        {
            soundOut?.Stop();
        }
        catch { /* Ignorer les erreurs d'arrêt */ }
        finally
        {
            if (soundOut != null)
            {
                soundOut.Stopped -= OnPlaybackStopped;
            }

            soundOut?.Dispose();
            soundOut = null;

            waveSource?.Dispose();
            waveSource = null;

            currentStream?.Dispose();
            currentStream = null;
        }
    }

    public void Dispose()
    {
        // Toujours déléguer la libération des ressources
        System.Threading.ThreadPool.QueueUserWorkItem(_ =>
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        });
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
        }
    }

    ~CSCoreClass()
    {
        Dispose(false);
    }
}
