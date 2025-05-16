using System;
using System.IO;

public interface IVGAudioPlayer : IDisposable
{
    MemoryStream Open(string filePath);
}

public interface IAudioPlayer : IDisposable
{
    void PlaySound(string filePath);
    void PlaySound(MemoryStream memoryStream);
    void StopAudioPlayback();
}
