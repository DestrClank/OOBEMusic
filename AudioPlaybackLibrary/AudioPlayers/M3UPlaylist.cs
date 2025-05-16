using System.Collections.Generic;
using System.IO;

namespace AudioPlaybackLibrary
{
    public class M3UPlaylist
    {
        public List<string> Tracks { get; private set; } = new List<string>();

        public void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Le fichier M3U est introuvable.", filePath);

            using (var reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!line.StartsWith("#")) // Ignore les métadonnées
                    {
                        Tracks.Add(line.Trim());
                    }
                }
            }
        }
    }
}
