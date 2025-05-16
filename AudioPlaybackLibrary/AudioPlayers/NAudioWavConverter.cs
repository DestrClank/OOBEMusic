using NAudio.Wave;
using System.IO;

public class NAudioWavConverter : IVGAudioPlayer
{
    public MemoryStream Open(string inputFilePath)
    {
        using (var reader = new AudioFileReader(inputFilePath))
        using (var pcmStream = new MemoryStream())
        {
            // Convertit en WAV PCM 16 bits
            WaveFileWriter.WriteWavFileToStream(pcmStream, reader);
            pcmStream.Position = 0; // Revenir au début du stream
            return new MemoryStream(pcmStream.ToArray()); // Retourne une copie pour usage externe
        }
    }

    //dispose
    public void Dispose()
    {
        // Libérer les ressources si nécessaire
        // Dans ce cas, il n'y a pas de ressources à libérer
    }
}

