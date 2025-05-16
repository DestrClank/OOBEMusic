using System;
using System.Diagnostics;
using System.IO;
using VGAudio.Containers.NintendoWare;
using VGAudio.Containers.Wave;

public class BrstmPlayer : IVGAudioPlayer
{
    private FileStream brstmBytes;
    private MemoryStream memoryStream;
    private bool disposed = false;

    /// <summary>
    /// Ouvre un fichier BRSTM, le décode en PCM, et le convertit en WAV.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public MemoryStream Open(string filePath)
    {
        return OpenBrstm(filePath);
    }

    /// <summary>
    /// Ouvre un fichier BRSTM, le décode en PCM, et le convertit en WAV.
    /// </summary>
    /// <param name="brstmFilePath">Chemin du fichier BRSTM à ouvrir.</param>
    /// <returns>Un MemoryStream contenant les données WAV.</returns>
    public MemoryStream OpenBrstm(string brstmFilePath)
    {
        if (disposed)
        {
            throw new ObjectDisposedException(nameof(BrstmPlayer), "The BrstmPlayer object has been disposed.");
        }

        if (!File.Exists(brstmFilePath))
        {
            throw new FileNotFoundException("The specified BRSTM file was not found.", brstmFilePath);
        }

        try
        {
            // Ouvrir le fichier BRSTM
            brstmBytes = new FileStream(brstmFilePath, FileMode.Open, FileAccess.Read);
            var brstmReader = new BrstmReader();
            var audioFormat = brstmReader.Read(brstmBytes);

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
            throw new Exception($"An error occurred while opening the BRSTM file: {ex.Message}", ex);
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
        if (!disposed)
        {
            if (disposing)
            {
                // Libérer les ressources managées
                memoryStream?.Dispose();
                memoryStream = null;

                brstmBytes?.Dispose();
                brstmBytes = null;
            }

            // Libérer les ressources non managées si nécessaire (aucune ici)
            disposed = true;
            Logging.EventLogger.LogToEventViewer("BrstmPlayer resources have been released.", EventLogEntryType.Information);
        }
    }

    /// <summary>
    /// Finaliseur pour garantir la libération des ressources non managées.
    /// </summary>
    ~BrstmPlayer()
    {
        Dispose(false);
    }
}
