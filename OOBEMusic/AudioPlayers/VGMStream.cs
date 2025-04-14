using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Resources;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OOBEMusic;

public class VGMStreamPlayer : IVGAudioPlayer
{
    private MemoryStream memoryStream;
    private Process process;
    private bool disposed = false;
    private static readonly ResourceManager rm = new ResourceManager("OOBEMusic.Ressources.Messages", typeof(OOBEMusicPlayer).Assembly);

    public MemoryStream Open(string musicPath)
    {
        if (disposed)
        {
            throw new ObjectDisposedException(nameof(VGMStreamPlayer), rm.GetString("ObjectDisposedVGMStream"));
        }

        if (!File.Exists(musicPath))
        {
            throw new FileNotFoundException(rm.GetString("FileNotFoundVGMStream"), musicPath);
        }

        try 
        {
            process = new Process();
            process.StartInfo.FileName = "Libs/vgmstream/vgmstream-cli.exe"; // Chemin vers vgmstream
            process.StartInfo.Arguments = $"-p \"{musicPath}"; // Arguments pour vgmstream
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            // Lecture ou traitement de la sortie ici

            memoryStream = new MemoryStream();

            using (var outputStream = process.StandardOutput.BaseStream)
            {
                outputStream.CopyTo(memoryStream);
            }

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception(rm.GetString("VGMStreamError"));
            }

            memoryStream.Position = 0; // Réinitialiser la position du flux

            return memoryStream;

        }
        catch (Exception ex)
        {
            Dispose();
            throw new Exception(string.Format(rm.GetString("ErrorOpeningVGMStream"), ex.Message), ex);
        }
    }

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
                process?.Dispose();
                process = null;
            }

            // Libérer les ressources non managées si nécessaire (aucune ici)
            disposed = true;
            Logging.EventLogger.LogToEventViewer(rm.GetString("BrstmPlayerResourceReleased"), EventLogEntryType.Information);
        }
    }

    /// <summary>
    /// Finaliseur pour garantir la libération des ressources non managées.
    /// </summary>
    ~VGMStreamPlayer()
    {
        Dispose(false);
    }

}
