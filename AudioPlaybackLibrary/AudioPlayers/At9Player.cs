using System;
using System.Diagnostics;
using System.IO;
using System.Resources;
using VGAudio.Containers.At9;
using VGAudio.Containers.Wave;

public class At9Player : IVGAudioPlayer
{
    private FileStream at9Bytes;
    private MemoryStream memoryStream;
    private bool _disposed;
    private static readonly ResourceManager rm = new ResourceManager("AudioPlaybackLibrary.Ressources.Messages", typeof(At9Player).Assembly);

    /// <summary>
    /// Opens an AT9 file, decodes it to PCM, and converts it to WAV.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public MemoryStream Open(string filePath)
    {
        return OpenAt9(filePath);
    }

    /// <summary>
    /// Opens an AT9 file, decodes it to PCM, and converts it to WAV.
    /// </summary>
    /// <param name="at9FilePath">Path to the AT9 file to open.</param>
    /// <returns>A MemoryStream containing the WAV data.</returns>
    public MemoryStream OpenAt9(string at9FilePath)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(At9Player), "The At9Player object has been disposed.");
        }

        if (!File.Exists(at9FilePath))
        {
            throw new FileNotFoundException("The specified AT9 file was not found.", at9FilePath);
        }

        try
        {
            // Open the AT9 file
            at9Bytes = new FileStream(at9FilePath, FileMode.Open, FileAccess.Read);
            var at9Reader = new At9Reader();
            var audioFormat = at9Reader.Read(at9Bytes);

            // Convert the audio format to WAV
            var wavWriter = new WaveWriter();
            var wavBytes = wavWriter.GetFile(audioFormat);

            // Create a MemoryStream for the WAV
            memoryStream = new MemoryStream(wavBytes);

            return memoryStream;
        }
        catch (Exception ex)
        {
            // Release resources in case of error
            Dispose();
            throw new Exception($"An error occurred while opening the AT9 file: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Releases the resources used by the class.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Implementation of the Dispose method to release resources.
    /// </summary>
    /// <param name="disposing">Indicates whether managed resources should be released.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Release managed resources
                memoryStream?.Dispose();
                memoryStream = null;

                at9Bytes?.Dispose();
                at9Bytes = null;
            }

            // Release unmanaged resources if necessary (none here)
            _disposed = true;
            Logging.EventLogger.LogToEventViewer("At9Player resources have been released.", EventLogEntryType.Information);
        }
    }

    /// <summary>
    /// Finalizer to ensure the release of unmanaged resources.
    /// </summary>
    ~At9Player()
    {
        Dispose(false);
    }
}
