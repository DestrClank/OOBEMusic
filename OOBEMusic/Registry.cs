using System;
using System.Resources;
using Microsoft.Win32;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace OOBEMusic
{
    public class RegHelper
    {
        const string keyName = @"SOFTWARE\DestrClank\OOBEMusic";
        private static readonly ResourceManager rm = new ResourceManager("OOBEMusic.Ressources.Messages", typeof(RegHelper).Assembly);

        // Méthode pour enregistrer le chemin du fichier .wav dans le registre
        public static void SaveKey(string chemin)
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName, true))
            {
                // Si la clé n'existe pas, la créer
                if (key == null)
                {
                    Logging.EventLogger.LogToEventViewer(rm.GetString("KeyNotFoundMessage"), EventLogEntryType.Warning);
                    using (RegistryKey newKey = Registry.LocalMachine.CreateSubKey(keyName))
                    {
                        try
                        {
                            newKey.SetValue("MusicFile", chemin, RegistryValueKind.String);
                            Logging.EventLogger.LogToEventViewer(rm.GetString("RegistryCreatedMessage"), EventLogEntryType.Information);
                        }
                        catch (Exception ex)
                        {
                            Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("RegistryCreationError"), ex.Message), EventLogEntryType.Error);
                        }
                    }
                }
                else
                {
                    string value = (string)key.GetValue("MusicFile");
                    Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("KeyAlreadyExistsMessage"), value), EventLogEntryType.Information);
                }
            }
        }

        public static string GetValue(string name)
        {
            string value = null;
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName, false))
            {
                if (key == null)
                {
                    string appPath = AppDomain.CurrentDomain.BaseDirectory;
                    string musicPath = appPath + "music.wav";
                    return musicPath;
                }
                else
                {
                    value = (string)key.GetValue(name);
                    key.Close();
                    key.Dispose();
                }
            }
            return value;
        }

        public static int CheckActivationState(string name, int defaultvalue = 1, bool logEnabled = true)
        {
            int Active = defaultvalue;

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName, false))
            {
                if (key == null)
                {
                    if (logEnabled)
                    {
                        Logging.EventLogger.LogToEventViewer(rm.GetString("KeyNotFoundForActivation"), EventLogEntryType.Warning);
                    }
                    return Active;
                }
                else
                {
                    try
                    {
                        if (key.GetValue(name) != null)
                        {
                            Active = Convert.ToInt32(key.GetValue(name));
                        }
                        else
                        {
                            if (defaultvalue == 1)
                            {
                                if (logEnabled)
                                {
                                    Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("ValueNotFoundDefaultActivation"), name), EventLogEntryType.Warning);
                                }
                            }
                            else
                            {
                                if (logEnabled)
                                {
                                    Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("ValueNotFound"), name), EventLogEntryType.Warning);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (logEnabled)
                        {
                            Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("ValueTypeError"), name, ex.Message), EventLogEntryType.Error);
                        }
                    }
                    finally
                    {
                        key.Close();
                        key.Dispose();
                    }
                }
                if (logEnabled)
                {
                    Logging.EventLogger.LogToEventViewer(string.Format(rm.GetString("ActivationState"), name, Active), EventLogEntryType.Information);
                }
                return Active;
            }
        }
    }
}
