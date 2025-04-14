using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
