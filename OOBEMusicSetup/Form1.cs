using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using OOBEMusic;
using OOBEMusic.Utils;

namespace OOBEMusicSetup
{
    public partial class Form1 : Form
    {
        static string WWAHostKeyName = "ActivateWWAHostMusic";
        static string FirstLogonAnimKeyName = "ActivateFirstLogonMusic";
        static string OOBEHostAppKeyName = "ActivateOOBEHostMusic";
        static string SuperVerboseLogsKeyName = "EnableSuperVerboseLogs";

        int WWAHostState = RegHelper.CheckActivationState(WWAHostKeyName);
        int FirstLogonAnimState = RegHelper.CheckActivationState(FirstLogonAnimKeyName);
        int OOBEShellState = RegHelper.CheckActivationState(OOBEHostAppKeyName);
        int ThreadTimeout = RegHelper.CheckThreadTimeout(); // Sleep time for the thread in milliseconds
        int SuperVerboseLogs = RegHelper.CheckActivationState(SuperVerboseLogsKeyName);

        string musicPath = RegHelper.GetValue("MusicFile").Replace("\"", string.Empty);

        public static string servicename = "OOBEMusicService";

        public ServiceControllerStatus serviceState;

        public Form1()
        {
            InitializeComponent();
            // Set the initial state of the checkboxes based on the registry values
            WWAHostCheck.Checked = WWAHostState == 1;
            firstLogonCheck.Checked = FirstLogonAnimState == 1;
            OOBEHostAppCheck.Checked = OOBEShellState == 1;
            superVerboseCheck.Checked = SuperVerboseLogs == 0;

            // Check if the thread timeout value is less than 100ms
            if (ThreadTimeout < 100)
            {
                MessageBox.Show("Thread timeout value cannot be less than 100ms.", "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                threadTimeoutNumber.Value = 100;
                ThreadTimeout = 100;
                RegHelper.WriteKey("ThreadTimeout", 100);
            }

            if (ThreadTimeout > 5000)
            {
                MessageBox.Show("Thread timeout value cannot be greater than 5000ms.", "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                threadTimeoutNumber.Value = 5000;
                ThreadTimeout = 5000;
                RegHelper.WriteKey("ThreadTimeout", 5000);
            }

            threadTimeoutNumber.Value = ThreadTimeout;

            if (!string.IsNullOrEmpty(musicPath))
            {
                filePathBox.Text = musicPath;
            }
            else
            {
                MessageBox.Show("Error", "An error occured retrieving the music file path.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //Check if the service is running
            serviceState = ServiceHelper.GetServiceStatus(servicename);

            if (serviceState == ServiceControllerStatus.Running)
            {
                serviceStatusLabel.Text = "Service is running";
                serviceStatusLabel.ForeColor = Color.Green;
            }
            else
            {
                serviceStatusLabel.Text = "Service is not running";
                serviceStatusLabel.ForeColor = Color.Red;
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            Console.WriteLine("DragEnter event triggered"); // Debug line
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop); // Récupère les fichiers déposés

                if (files.Length > 1)
                {
                    MessageBox.Show("Veuillez déposer un seul fichier.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Annule l'action si plusieurs fichiers sont déposés
                }

                string file = files[0]; // Récupère le seul fichier déposé
                MessageBox.Show($"Fichier déposé : {file}", "Drag and Drop", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Ajoutez ici le traitement du fichier (par exemple, l'afficher dans un TextBox)
                filePathBox.Text = file;
                RegHelper.WriteKey("MusicFile", "\"" + file + "\"");
            }
        }

        private void WWAHostCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (WWAHostCheck.Checked)
            {
                RegHelper.WriteKey(WWAHostKeyName, 1);
                WWAHostState = 1;
            }
            else
            {
                RegHelper.WriteKey(WWAHostKeyName, 0);
                WWAHostState = 0;
            }
        }

        private void firstLogonCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (firstLogonCheck.Checked)
            {
                RegHelper.WriteKey(FirstLogonAnimKeyName, 1);
                FirstLogonAnimState = 1;
            }
            else
            {
                RegHelper.WriteKey(FirstLogonAnimKeyName, 0);
                FirstLogonAnimState = 0;
            }
        }

        private void OOBEHostAppCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (OOBEHostAppCheck.Checked)
            {
                RegHelper.WriteKey(OOBEHostAppKeyName, 1);
                OOBEShellState = 1;
            }
            else
            {
                RegHelper.WriteKey(OOBEHostAppKeyName, 0);
                OOBEShellState = 0;
            }
        }

        private void superVerboseCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (superVerboseCheck.Checked)
            {
                // Set the registry key to enable super verbose logs
                RegHelper.WriteKey(SuperVerboseLogsKeyName, 1);
                SuperVerboseLogs = 1;

            }
            else
            {
                RegHelper.WriteKey(SuperVerboseLogsKeyName, 0);
                SuperVerboseLogs = 0;
            }
        }

        private void fileSelectButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Audio files|*.at9;*.brstm;*.wav;*.mp3|All files (*.*)|*.*";
                openFileDialog.Title = "Select an audio file";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePathBox.Text = openFileDialog.FileName;
                    RegHelper.WriteKey("MusicFile", "\"" + openFileDialog.FileName + "\"");
                }
            }
        }

        private void serviceStartButton_Click(object sender, EventArgs e)
        {

            if (CheckIfEverythingDisabled())
            {
                MessageBox.Show("The service won't start because every music playback option is disabled, enable at least one of them and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (ServiceHelper.IsServiceDisabled(servicename))
            {
                MessageBox.Show("The service is disabled, please enable it first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            serviceState = ServiceHelper.GetServiceStatus(servicename);

            if (serviceState == ServiceControllerStatus.Running)
            {
                MessageBox.Show("Service is already running.", "Service Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                ServiceHelper.StartService(servicename);
                serviceStatusLabel.Text = "Service is running";
                serviceStatusLabel.ForeColor = Color.Green;
            }
        }

        public void restartServiceButton_Click(object sender, EventArgs e)
        {
            if (CheckIfEverythingDisabled())
            {
                MessageBox.Show("The service won't restart because every music playback option is disabled, enable at least one of them and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            serviceState = ServiceHelper.GetServiceStatus(servicename);
            if (serviceState == ServiceControllerStatus.Running)
            {
                ServiceHelper.RestartService(servicename);

                // Wait for the service to restart  
                var stopwatch = Stopwatch.StartNew();
                while (ServiceHelper.GetServiceStatus(servicename) != ServiceControllerStatus.Running)
                {
                    if (stopwatch.ElapsedMilliseconds > 5000)
                    {
                        MessageBox.Show("Le service n'a pas pu redémarrer dans les 5 secondes.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    System.Threading.Thread.Sleep(100);
                }

                serviceStatusLabel.Text = "Service is running";
                serviceStatusLabel.ForeColor = Color.Green;
            }
            else
            {
                MessageBox.Show("Service is not running.", "Service Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void disableServiceButton_Click(object sender, EventArgs e)
        {
            try
            {
                ServiceHelper.DisableService(servicename);
                serviceStatusLabel.Text = "Service is disabled";
                serviceStatusLabel.ForeColor = Color.Red;
                MessageBox.Show("Service disabled successfully.", "Service Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error disabling service: " + ex.Message, "Service Status", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void stopServiceButton_Click(object sender, EventArgs e)
        {
            serviceState = ServiceHelper.GetServiceStatus(servicename);
            if (serviceState == ServiceControllerStatus.Stopped)
            {
                MessageBox.Show("Service is already stopped.", "Service Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {

                ServiceHelper.StopService(servicename);

                // Wait for the service to stop
                var stopwatch = Stopwatch.StartNew();
                while (ServiceHelper.GetServiceStatus(servicename) != ServiceControllerStatus.Stopped)
                {
                    if (stopwatch.ElapsedMilliseconds > 5000)
                    {
                        MessageBox.Show("Le service n'a pas pu s'arrêter dans les 5 secondes.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    System.Threading.Thread.Sleep(100);
                }

                serviceStatusLabel.Text = "Service is not running";
                serviceStatusLabel.ForeColor = Color.Red;
            }
        }

        private void threadTimeoutNumber_ValueChanged(object sender, EventArgs e)
        {
            int newValue = (int)threadTimeoutNumber.Value;
            if (newValue < 100)
            {
                MessageBox.Show("Thread timeout value cannot be less than 100ms.", "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                threadTimeoutNumber.Value = 100;
            }
            else
            {
                RegHelper.WriteKey("ThreadTimeout", newValue);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ServiceHelper.EnableService(servicename);
                MessageBox.Show("Service enabled successfully.", "Service Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error enabling service: " + ex.Message, "Service Status", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool CheckIfEverythingDisabled()
        {

            if (WWAHostState == 0 && OOBEShellState == 0 && FirstLogonAnimState == 0)
            {
                return true;
            } else
            {
                return false;
            }
        }
        
        //start service
        private void button2_Click(object sender, EventArgs e)
        {

            if (CheckIfEverythingDisabled())
            {
                MessageBox.Show("The service won't start because every music playback option is disabled, enable at least one of them and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (ServiceHelper.IsServiceDisabled(servicename))
            {
                MessageBox.Show("The service is disabled, please enable it first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                serviceState = ServiceHelper.GetServiceStatus(servicename);

                if (ServiceHelper.GetServiceStatus(servicename) == ServiceControllerStatus.Running)
                {
                    MessageBox.Show("Service is already running.", "Service Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    ServiceHelper.StartService(servicename);

                    // Wait for the service to start
                    var stopwatch = Stopwatch.StartNew();
                    while (ServiceHelper.GetServiceStatus(servicename) != ServiceControllerStatus.Running)
                    {
                        if (stopwatch.ElapsedMilliseconds > 5000)
                        {
                            MessageBox.Show("The service failed to restart in a timely fashion.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        System.Threading.Thread.Sleep(100);
                    }

                    serviceStatusLabel.Text = "Service is running";
                    serviceStatusLabel.ForeColor = Color.Green;
                }
            } catch (Exception ex)
            {
                MessageBox.Show($"Error starting service : {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //open event viewer
            Process.Start("eventvwr.msc");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (File.Exists("WWAHost.exe"))
            {
                Process.Start("WWAHost.exe");
            } else { MessageBox.Show("WWAHost.exe not found in the current directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult response = MessageBox.Show("Are you sure you want to reset the registry keys?", "Reset Registry Keys", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (response == DialogResult.Yes)
            {
                try
                {
                    RegHelper.ResetRegistryKeys();
                    // Reset the checkboxes to their default state

                    WWAHostState = RegHelper.CheckActivationState(WWAHostKeyName);
                    FirstLogonAnimState = RegHelper.CheckActivationState(FirstLogonAnimKeyName);
                    OOBEShellState = RegHelper.CheckActivationState(OOBEHostAppKeyName);
                    SuperVerboseLogs = RegHelper.CheckActivationState(SuperVerboseLogsKeyName);
                    ThreadTimeout = RegHelper.CheckThreadTimeout();

                    WWAHostCheck.Checked = WWAHostState == 1;
                    firstLogonCheck.Checked = FirstLogonAnimState == 1;
                    OOBEHostAppCheck.Checked = OOBEShellState == 1;
                    superVerboseCheck.Checked = SuperVerboseLogs == 0;
                    threadTimeoutNumber.Value = ThreadTimeout;

                    if (serviceState == ServiceControllerStatus.Running)
                    {
                        ServiceHelper.RestartService(servicename);

                        // Wait for the service to restart  
                        var stopwatch = Stopwatch.StartNew();
                        while (ServiceHelper.GetServiceStatus(servicename) != ServiceControllerStatus.Running)
                        {
                            if (stopwatch.ElapsedMilliseconds > 5000)
                            {
                                MessageBox.Show("The service failed to restart in a timely fashion.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            System.Threading.Thread.Sleep(100);
                        }

                        serviceStatusLabel.Text = "Service is running";
                        serviceStatusLabel.ForeColor = Color.Green;
                    }

                    musicPath = RegHelper.GetValue("MusicFile").Replace("\"", string.Empty);
                    filePathBox.Text = musicPath;

                    MessageBox.Show("Registry keys reset successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error resetting registry keys: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Ajoutez une méthode utilitaire pour convertir les valeurs 0 et 1 en booléens.
        private bool ConvertToBool(int value)
        {
            return value == 1;
        }

        // Ajoutez une méthode utilitaire pour convertir les booléens en valeurs 0 et 1.
        private int ConvertToInt(bool value)
        {
            return value ? 1 : 0;
        }
    }
}
