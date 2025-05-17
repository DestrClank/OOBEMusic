using AudioPlaybackLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicPlaylistBuilder
{
    public partial class Form1 : Form
    {
        private List<MusicItem> musicItems = new List<MusicItem>();

        private int lastSortedColumn = -1; // Pour mémoriser la dernière colonne triée
        private SortOrder lastSortOrder = SortOrder.None; // Pour mémoriser l'ordre de tri
        private string[] supportedTagLibFileTypes = { ".mp3", ".wav" };
        // Update the declaration of `currentMusicPlaying` to make it nullable.  
        private MusicItem? currentMusicPlaying;
        private bool playing = false; // Indique si un morceau est en cours de lecture
        private bool saved = false; // Indique si la playlist a été enregistrée
        private bool loaded = false; // Indique si la playlist a été chargée
        private string lastFile;

        private AudioPlayer audioPlayer = new AudioPlayer();

        public Form1()
        {
            InitializeComponent();
            listView1.ColumnClick += new ColumnClickEventHandler(listView1_ColumnClick);
            listView1.FullRowSelect = true;
            listView1.AllowDrop = true; // Activer le drag & drop
            listView1.DragEnter += listView1_DragEnter;
            listView1.DragDrop += listView1_DragDrop;
            listView1.DoubleClick += ListView1_DoubleClick;

            listView1.KeyDown += new KeyEventHandler(listView1_KeyDown); // Pour gérer la touche Suppr

            // Ajoutez cet événement dans le constructeur de Form1
            this.FormClosing += Form1_FormClosing;

            // Ajoutez ceci dans le constructeur de Form1 après InitializeComponent()
            listView1.Cursor = Cursors.Default;

        }

        private void PopulateListView()
        {
            listView1.Items.Clear();
            foreach (var musicItem in musicItems)
            {
                ListViewItem item = new ListViewItem(musicItem.Id.ToString());
                item.SubItems.Add(musicItem.Title);
                item.SubItems.Add(musicItem.Artist);
                item.SubItems.Add(musicItem.Album);
                item.SubItems.Add(musicItem.FileName);
                item.SubItems.Add(musicItem.FilePath);
                listView1.Items.Add(item);
            }
        }

        private void RemoveItemFromListView(int index)
        {
            // Vérifiez que l'index est valide
            if (index < 0 || index >= listView1.Items.Count || index >= musicItems.Count)
                return;

            // Supprimez l'élément de la ListView
            listView1.Items.RemoveAt(index);

            // Supprimez l'élément correspondant de la liste musicItems
            musicItems.RemoveAt(index);

            // Réassigner les IDs pour maintenir l'ordre séquentiel
            for (int i = 0; i < musicItems.Count; i++)
            {
                var musicItem = musicItems[i];
                musicItem.Id = i + 1; // Mettre à jour l'ID
                musicItems[i] = musicItem; // Réinsérer l'élément modifié
            }

            // Rafraîchir la ListView pour refléter les IDs mis à jour
            PopulateListView();
        }

        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayAudio();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Récupérez les indices des éléments sélectionnés, triés dans l'ordre décroissant
            var selectedIndices = listView1.SelectedIndices.Cast<int>().OrderByDescending(i => i).ToList();

            // Supprimez chaque élément en utilisant son index
            foreach (var index in selectedIndices)
            {
                RemoveItemFromListView(index);
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            // Check if the right mouse button was clicked
            if (e.Button == MouseButtons.Right)
            {
                // Get the item at the clicked position
                ListViewItem item = listView1.GetItemAt(e.X, e.Y);
                if (item != null)
                {
                    // Select the item
                    item.Selected = true;
                    // Show the context menu
                    contextMenuStrip1.Show(listView1, e.Location);
                }
            }
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                int index = listView1.SelectedItems[0].Index;
                MoveItem(index, true); // Déplace l'item vers le haut
            }
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                int index = listView1.SelectedItems[0].Index;
                MoveItem(index, false); // Déplace l'item vers le bas
            }
        }

        private void addFolderButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
            {
                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = folderBrowser.SelectedPath;
                    string[] musicFiles = System.IO.Directory.GetFiles(folderPath, "*.mp3");

                    foreach (string musicFile in musicFiles)
                    {
                        // Utilisez TagLib# pour lire les métadonnées  
                        var file = TagLib.File.Create(musicFile);

                        MusicItem musicItem = new MusicItem
                        {
                            Id = musicItems.Count + 1,
                            Title = file.Tag.Title ?? System.IO.Path.GetFileNameWithoutExtension(musicFile),
                            Artist = file.Tag.FirstPerformer ?? "Unknown Artist",
                            Album = file.Tag.Album ?? "Unknown Album",
                            FileName = System.IO.Path.GetFileName(musicFile),
                            FilePath = musicFile
                        };

                        musicItems.Add(musicItem);
                    }
                    PopulateListView();
                }
            }
        }

        private void addFilesButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Music Files|*.mp3;*.wav;*.at9;*.brstm|All Files|*.*";
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string musicFile in openFileDialog.FileNames)
                    {
                        // Utilisez TagLib# pour lire les métadonnées

                        MusicItem musicItem;

                        // Check if the file type is supported by TagLib
                        string fileExtension = System.IO.Path.GetExtension(musicFile).ToLower();
                        if (!supportedTagLibFileTypes.Contains(fileExtension))
                        {
                            musicItem = new MusicItem
                            {
                                Id = musicItems.Count + 1,
                                Title = System.IO.Path.GetFileNameWithoutExtension(musicFile),
                                Artist = "Unknown Artist",
                                Album = "Unknown Album",
                                FileName = System.IO.Path.GetFileName(musicFile),
                                FilePath = musicFile
                            };
                        }
                        else
                        {
                            // Utilisez TagLib# pour lire les métadonnées
                            var file = TagLib.File.Create(musicFile);
                            musicItem = new MusicItem
                            {
                                Id = musicItems.Count + 1,
                                Title = file.Tag.Title ?? System.IO.Path.GetFileNameWithoutExtension(musicFile),
                                Artist = file.Tag.FirstPerformer ?? "Unknown Artist",
                                Album = file.Tag.Album ?? "Unknown Album",
                                FileName = System.IO.Path.GetFileName(musicFile),
                                FilePath = musicFile
                            };
                        }
                        musicItems.Add(musicItem);
                    }
                    PopulateListView();
                }
            }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Déterminez l'ordre de tri (ascendant ou descendant)
            if (e.Column == lastSortedColumn)
            {
                // Inversez l'ordre si la même colonne est cliquée
                lastSortOrder = (lastSortOrder == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                // Par défaut, tri ascendant pour une nouvelle colonne
                lastSortOrder = SortOrder.Ascending;
            }

            lastSortedColumn = e.Column;

            // Appliquez le comparateur générique
            listView1.ListViewItemSorter = new GenericComparer(e.Column, lastSortOrder);
        }

        // 2. Gérer l'événement DragEnter pour accepter les fichiers
        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        // 3. Gérer l'événement DragDrop pour ajouter les fichiers à la playlist
        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                string extension = System.IO.Path.GetExtension(file).ToLower();
                MusicItem musicItem;

                if (!supportedTagLibFileTypes.Contains(extension))
                {
                    musicItem = new MusicItem
                    {
                        Id = musicItems.Count + 1,
                        Title = System.IO.Path.GetFileNameWithoutExtension(file),
                        Artist = "Unknown Artist",
                        Album = "Unknown Album",
                        FileName = System.IO.Path.GetFileName(file),
                        FilePath = file
                    };
                }
                else
                {
                    try
                    {
                        var tagFile = TagLib.File.Create(file);
                        musicItem = new MusicItem
                        {
                            Id = musicItems.Count + 1,
                            Title = tagFile.Tag.Title ?? System.IO.Path.GetFileNameWithoutExtension(file),
                            Artist = tagFile.Tag.FirstPerformer ?? "Unknown Artist",
                            Album = tagFile.Tag.Album ?? "Unknown Album",
                            FileName = System.IO.Path.GetFileName(file),
                            FilePath = file
                        };
                    }
                    catch
                    {
                        musicItem = new MusicItem
                        {
                            Id = musicItems.Count + 1,
                            Title = System.IO.Path.GetFileNameWithoutExtension(file),
                            Artist = "Unknown Artist",
                            Album = "Unknown Album",
                            FileName = System.IO.Path.GetFileName(file),
                            FilePath = file
                        };
                    }
                }
                musicItems.Add(musicItem);
            }
            PopulateListView();
        }
        private void clearButton_Click(object sender, EventArgs e)
        {
            // Clear the ListView and the music items list
            listView1.Items.Clear();
            musicItems.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Save the playlist to a file
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Playlist Files|*.m3u;*.m3u8|All Files|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    saved = true; // Indique que la playlist a été enregistrée
                    lastFile = filePath; // Mémoriser le dernier fichier enregistré
                    toolStripStatusLabel1.Text = "Playlist saved to: " + filePath;

                    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath))
                    {
                        foreach (var musicItem in musicItems)
                        {
                            writer.WriteLine(musicItem.FilePath);
                        }
                    }
                    MessageBox.Show("Playlist saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            // Récupérez les indices des éléments sélectionnés, triés dans l'ordre décroissant
            var selectedIndices = listView1.SelectedIndices.Cast<int>().OrderByDescending(i => i).ToList();

            // Supprimez chaque élément en utilisant son index
            foreach (var index in selectedIndices)
            {
                RemoveItemFromListView(index);
            }
        }

        private void ListView1_DoubleClick(object sender, EventArgs e)
        {
            PlayAudio();
        }

        private void SwapItems(int index1, int index2)
        {
            // Vérifiez que les indices sont valides
            if (index1 < 0 || index2 < 0 || index1 >= musicItems.Count || index2 >= musicItems.Count)
                return;

            // Échangez les éléments dans la liste musicItems
            var temp = musicItems[index1];
            musicItems[index1] = musicItems[index2];
            musicItems[index2] = temp;

            // Mettez à jour les IDs pour refléter les nouvelles positions
            var item1 = musicItems[index1];
            var item2 = musicItems[index2];

            item1.Id = index1 + 1;
            item2.Id = index2 + 1;

            musicItems[index1] = item1;
            musicItems[index2] = item2;

            // Rafraîchissez la ListView
            PopulateListView();

            // Sélectionnez les items échangés dans la ListView
            listView1.Items[index1].Selected = true;
            listView1.Items[index2].Selected = true;
        }

        private void MoveItem(int index, bool moveUp)
        {
            // Vérifiez que l'index est valide
            if (index < 0 || index >= musicItems.Count)
                return;

            // Déterminez le nouvel index
            int newIndex = moveUp ? index - 1 : index + 1;

            // Vérifiez que le nouvel index est dans les limites
            if (newIndex < 0 || newIndex >= musicItems.Count)
                return;

            // Échangez les éléments dans la liste musicItems
            var currentItem = musicItems[index];
            var targetItem = musicItems[newIndex];

            // Mettez à jour les IDs pour refléter les nouvelles positions
            currentItem.Id = newIndex + 1;
            targetItem.Id = index + 1;

            // Réinsérez les éléments modifiés dans la liste
            musicItems[index] = targetItem;
            musicItems[newIndex] = currentItem;

            // Rafraîchissez la ListView
            PopulateListView();

            // Sélectionnez l'item déplacé dans la ListView
            listView1.Items[newIndex].Selected = true;
            listView1.Items[newIndex].Focused = true;
            listView1.EnsureVisible(newIndex);
        }

        private void upToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Same as the button
            if (listView1.SelectedItems.Count > 0)
            {
                int index = listView1.SelectedItems[0].Index;
                MoveItem(index, true); // Déplace l'item vers le haut
            }
        }

        private void downToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Same as the button
            if (listView1.SelectedItems.Count > 0)
            {
                int index = listView1.SelectedItems[0].Index;
                MoveItem(index, false); // Déplace l'item vers le bas
            }
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            PlayAudio();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Dispose of the audio player when the form is closing
            audioPlayer.Stop();
        }

        private void PlayAudio()
        {

            // Check if an item is selected
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a music item to play.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int index = listView1.SelectedItems[0].Index;
            // Play audio with AudioPlayerLibrary
            MusicItem selectedMusicItem = musicItems[index];

            // Check if the selected item is already playing
            if (currentMusicPlaying?.FilePath == selectedMusicItem.FilePath)
            {
                MessageBox.Show("The selected music is already playing.", "Already Playing", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Get the selected music item


            if (playing)
            {
                // Stop the currently playing music
                audioPlayer.Stop();
            }

            currentMusicPlaying = musicItems[index];

            playing = true; // Mettre à jour l'état de lecture
            try
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    string musicPath = musicItems[index].FilePath;
                    // Utilisez AudioPlayer pour lire le fichier audio
                    // Joue le fichier audio
                    Task.Run(() =>
                    {
                        audioPlayer.PlayInLoop(musicPath);
                    });

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while playing the audio: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                audioPlayer.Stop();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // Update the line causing the error to assign null to the nullable type.  
        private void stopButton_Click(object sender, EventArgs e)
        {
            // Stop audio playback  
            audioPlayer.Stop();
            playing = false; // Mettre à jour l'état de lecture  
            currentMusicPlaying = null; // Réinitialiser l'élément de musique en cours  
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Playlists (*.m3u;*.m3u8)|*.m3u;*.m3u8|Tous les fichiers|*.*";
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string playlistPath = openFileDialog.FileName;
                    string[] lines = System.IO.File.ReadAllLines(playlistPath);
                    var newMusicItems = new List<MusicItem>();
                    int id = 1;

                    loaded = true; // Indique que la playlist a été chargée
                    lastFile = playlistPath; // Mémoriser le dernier fichier chargé
                    toolStripStatusLabel1.Text = "Playlist loaded from: " + playlistPath;

                    foreach (string line in lines)
                    {
                        string trimmed = line.Trim();
                        // Ignore commentaires et lignes vides
                        if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                            continue;

                        string filePath = trimmed;
                        if (!System.IO.File.Exists(filePath))
                            continue;

                        string extension = System.IO.Path.GetExtension(filePath).ToLower();
                        MusicItem musicItem;

                        if (!supportedTagLibFileTypes.Contains(extension))
                        {
                            musicItem = new MusicItem
                            {
                                Id = id++,
                                Title = System.IO.Path.GetFileNameWithoutExtension(filePath),
                                Artist = "Unknown Artist",
                                Album = "Unknown Album",
                                FileName = System.IO.Path.GetFileName(filePath),
                                FilePath = filePath
                            };
                        }
                        else
                        {
                            try
                            {
                                var tagFile = TagLib.File.Create(filePath);
                                musicItem = new MusicItem
                                {
                                    Id = id++,
                                    Title = tagFile.Tag.Title ?? System.IO.Path.GetFileNameWithoutExtension(filePath),
                                    Artist = tagFile.Tag.FirstPerformer ?? "Unknown Artist",
                                    Album = tagFile.Tag.Album ?? "Unknown Album",
                                    FileName = System.IO.Path.GetFileName(filePath),
                                    FilePath = filePath
                                };
                            }
                            catch
                            {
                                musicItem = new MusicItem
                                {
                                    Id = id++,
                                    Title = System.IO.Path.GetFileNameWithoutExtension(filePath),
                                    Artist = "Unknown Artist",
                                    Album = "Unknown Album",
                                    FileName = System.IO.Path.GetFileName(filePath),
                                    FilePath = filePath
                                };
                            }
                        }
                        newMusicItems.Add(musicItem);
                    }

                    musicItems = newMusicItems;
                    PopulateListView();
                    //MessageBox.Show("Playlist loaded successfully.", "Playlist Loaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void usePlaylistButton_Click(object sender, EventArgs e)
        {
            // avec oobesetup.exe, réglez le chemin de la playlist
            // vérifier si l'utilisateur a chargé ou enregistré une playlist
            if (musicItems.Count == 0)
            {
                MessageBox.Show("Please add music to the playlist before using it.", "No music on playlist", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Vérifiez si l'utilisateur a enregistré la playlist
            if (!saved)
            {
                DialogResult result = MessageBox.Show("You have not saved the playlist. Do you want to save it now?", "Unsaved Playlist", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    button1_Click(sender, e); // Appel de la méthode pour enregistrer la playlist
                }
                else if (result == DialogResult.Cancel)
                {
                    return; // Annuler l'utilisation de la playlist
                }
            }
            // utiliser oobesetup.exe pour définir le chemin de la playlist
            // avec lastfile
            if (string.IsNullOrEmpty(lastFile))
            {
                MessageBox.Show("No playlist file found. Please save the playlist first.", "No Playlist File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Vérifiez si le fichier existe
            if (!System.IO.File.Exists(lastFile))
            {
                MessageBox.Show("The playlist file does not exist. Please save the playlist again.", "Playlist File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Utilisez oobesetup.exe pour définir le chemin de la playlist
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "oobesetup.exe",
                    Arguments = $"/music-file \"{lastFile}\"",
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    Verb = "runas" // Demande l'élévation
                }
            };
            try
            {
                process.Start();
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                // L'utilisateur a peut-être refusé l'élévation
                MessageBox.Show("The operation needs administrative privileges.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            process.WaitForExit();
            if (process.ExitCode == 0)
            {
                MessageBox.Show("Playlist path set successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Error setting playlist path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Fonction qui permet d'appuyer sur Suppr et de supprimer l'élément sélectionné
        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                // Récupérez les indices des éléments sélectionnés, triés dans l'ordre décroissant
                var selectedIndices = listView1.SelectedIndices.Cast<int>().OrderByDescending(i => i).ToList();
                // Supprimez chaque élément en utilisant son index
                foreach (var index in selectedIndices)
                {
                    RemoveItemFromListView(index);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // C#
            var startInfo = new ProcessStartInfo
            {
                FileName = "OOBEMusicSettings.exe",
                UseShellExecute = true, // Obligatoire pour l'élévation
                Verb = "runas" // Demande l'élévation
            };

            try
            {
                Process.Start(startInfo);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                // L'utilisateur a peut-être refusé l'élévation
                MessageBox.Show("The operation needs administrative privileges.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start("WWAHost.exe");
        }
    }
}
