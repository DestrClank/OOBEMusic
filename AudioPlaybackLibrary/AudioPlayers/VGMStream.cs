using System;
using System.Diagnostics;
using System.IO;

public class VGMStreamPlayer : IVGAudioPlayer
{
    private Process process;
    private bool disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="VGMStreamPlayer"/> class.
    /// </summary>
    /// <param name="musicPath"></param>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="Exception"></exception>
    public MemoryStream Open(string musicPath)
    {
        if (disposed)
            throw new ObjectDisposedException(nameof(VGMStreamPlayer), "The VGMStreamPlayer object has been disposed.");

        if (!File.Exists(musicPath))
            throw new FileNotFoundException("The specified file was not found.", musicPath);

        try
        {
            process = new Process();

            string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libs", "vgmstream", "vgmstream-cli.exe");
            if (File.Exists(exePath))
            {
                process.StartInfo.FileName = exePath;
            }
            else
            {
                process.StartInfo.FileName = "vgmstream-cli.exe";
            }

            process.StartInfo.Arguments = $"-p \"{musicPath}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            var memoryStream = new MemoryStream();
            using (var outputStream = process.StandardOutput.BaseStream)
            {
                outputStream.CopyTo(memoryStream);
            }

            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new Exception("An error occurred while processing the audio file with VGMStream.");

            memoryStream.Position = 0;
            return memoryStream;
        }
        catch (Exception ex)
        {
            Dispose();
            throw new Exception($"An error occurred while opening the audio file: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Releases the resources used by the VGMStreamPlayer.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                process?.Dispose();
                process = null;
            }
            disposed = true;
            Logging.EventLogger.LogToEventViewer("VGMStreamPlayer resources have been released.", EventLogEntryType.Information);
        }
    }

    ~VGMStreamPlayer()
    {
        Dispose(false);
    }
}
