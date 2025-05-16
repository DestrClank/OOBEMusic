using System;
using System.IO;
using System.Threading;

namespace AudioPlaybackLibrary
{
    public class AudioPlayer : IDisposable
    {
        private IAudioPlayer player;
        private CancellationTokenSource cancellationTokenSource;
        private string[] playlistTracks;
        private int currentTrackIndex;
        private bool isLoopingPlaylist;
        private string currentFilePath;
        private readonly object syncLock = new object();

        public void PlayInLoop(string filePath)
        {
            lock (syncLock)
            {
                Stop();

                if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                    throw new FileNotFoundException("Le fichier ou la playlist spécifié est introuvable.");

                cancellationTokenSource?.Dispose();
                cancellationTokenSource = new CancellationTokenSource();

                if (IsPlaylist(filePath))
                {
                    try
                    {
                        var playlist = new M3UPlaylist();
                        playlist.Load(filePath);
                        playlistTracks = playlist.Tracks.ToArray();
                        currentTrackIndex = 0;
                        isLoopingPlaylist = true;
                        PlayTrack(playlistTracks[currentTrackIndex]);
                    }
                    catch (Exception ex)
                    {
                        // Log ou gestion d'erreur personnalisée
                        throw new InvalidOperationException("Erreur lors du chargement de la playlist.", ex);
                    }
                }
                else
                {
                    isLoopingPlaylist = false;
                    currentFilePath = filePath;
                    PlayTrack(currentFilePath);
                }
            }
        }

        private void PlayTrack(string filePath)
        {
            lock (syncLock)
            {
                DisposeAudioPlayer();
                string extension = Path.GetExtension(filePath).ToLowerInvariant();

                try
                {
                    switch (extension)
                    {
                        case ".wav":
                            player = new SoundPlayerClass();
                            player.PlaySound(filePath);
                            break;

                        case ".mp3":
                            using (var mp3Player = new NAudioWavConverter())
                            {
                                player = new CSCoreClass();
                                if (player is CSCoreClass cscoreMp3)
                                    cscoreMp3.PlaybackStopped += OnPlaybackStopped;
                                player.PlaySound(mp3Player.Open(filePath));
                            }
                            break;

                        default:
                            using (var vgmPlayer = new VGMStreamPlayer())
                            {
                                player = new CSCoreClass();
                                if (player is CSCoreClass cscoreVgm)
                                    cscoreVgm.PlaybackStopped += OnPlaybackStopped;
                                player.PlaySound(vgmPlayer.Open(filePath));
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // Log ou gestion d'erreur personnalisée
                    throw new InvalidOperationException($"Erreur lors de la lecture du fichier {filePath}.", ex);
                }
            }
        }

        private void OnPlaybackStopped(object sender, EventArgs e)
        {
            lock (syncLock)
            {
                if (isLoopingPlaylist && playlistTracks != null)
                {
                    currentTrackIndex = (currentTrackIndex + 1) % playlistTracks.Length;
                    Stop();
                    PlayTrack(playlistTracks[currentTrackIndex]);
                }
                else if (!isLoopingPlaylist && !string.IsNullOrEmpty(currentFilePath))
                {
                    Stop();
                    PlayTrack(currentFilePath); // Relance la même musique
                }
            }
        }

        private bool IsPlaylist(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension == ".m3u" || extension == ".m3u8";
        }

        public void Stop()
        {
            lock (syncLock)
            {
                DisposeAudioPlayer();
            }
        }

        public void DisposeAudioPlayer()
        {
            lock (syncLock)
            {
                if (player is CSCoreClass cscorePlayer)
                    cscorePlayer.PlaybackStopped -= OnPlaybackStopped;
                player?.Dispose();
                player = null;

                cancellationTokenSource?.Cancel();
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = null;
            }
        }

        public void Dispose()
        {
            DisposeAudioPlayer();
            GC.SuppressFinalize(this);
        }
    }
}
