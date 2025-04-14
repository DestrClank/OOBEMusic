using System;
using System.IO;
using VGAudio.Containers.At9;
using VGAudio.Containers.Wave;
using System.Resources;
using System.Diagnostics;
using OOBEMusic;

public class At9Player : IVGAudioPlayer
{
    private FileStream at9Bytes;
    private MemoryStream memoryStream;
    private bool _disposed;
    private static readonly ResourceManager rm = new ResourceManager("OOBEMusic.Ressources.Messages", typeof(OOBEMusicPlayer).Assembly);

    /// <summary>
    /// Ouvre un fichier AT9, le décode en PCM, et le convertit en WAV.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public MemoryStream Open(string filePath)
    {
        return OpenAt9(filePath);
    }

    /// <summary>
    /// Ouvre un fichier AT9, le décode en PCM, et le convertit en WAV.
    /// </summary>
    /// <param name="at9FilePath">Chemin du fichier AT9 à ouvrir.</param>
    /// <returns>Un MemoryStream contenant les données WAV.</returns>
    public MemoryStream OpenAt9(string at9FilePath)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(At9Player), rm.GetString("ObjectDisposedAt9Player"));
        }

        if (!File.Exists(at9FilePath))
        {
            throw new FileNotFoundException(rm.GetString("FileNotFoundAt9"), at9FilePath);
        }

        try
        {
            // Ouvrir le fichier AT9
            at9Bytes = new FileStream(at9FilePath, FileMode.Open, FileAccess.Read);
            var at9Reader = new At9Reader();
            var audioFormat = at9Reader.Read(at9Bytes);

            // Convertir le format audio en WAV
            var wavWriter = new WaveWriter();
            var wavBytes = wavWriter.GetFile(audioFormat);

            // Créer un MemoryStream pour le WAV
            memoryStream = new MemoryStream(wavBytes);

            return memoryStream;
        }
        catch (Exception ex)
        {
            // Libérer les ressources en cas d'erreur
            Dispose();
            throw new Exception(string.Format(rm.GetString("ErrorOpeningAt9File"), ex.Message), ex);
        }
    }

    /// <summary>
    /// Libère les ressources utilisées par la classe.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Implémentation de la méthode Dispose pour libérer les ressources.
    /// </summary>
    /// <param name="disposing">Indique si les ressources managées doivent être libérées.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Libérer les ressources managées
                memoryStream?.Dispose();
                memoryStream = null;

                at9Bytes?.Dispose();
                at9Bytes = null;
            }

            // Libérer les ressources non managées si nécessaire (aucune ici)
            _disposed = true;
            Logging.EventLogger.LogToEventViewer(rm.GetString("At9PlayerResourceReleased"), EventLogEntryType.Information);
        }
    }

    /// <summary>
    /// Finaliseur pour garantir la libération des ressources non managées.
    /// </summary>
    ~At9Player()
    {
        Dispose(false);
    }
}
