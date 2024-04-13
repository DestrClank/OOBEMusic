using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace OOBEMusic
{
    public class RegHelper
    {
        const string keyName = @"SOFTWARE\DestrClank\OOBEMusic";
        // Méthode pour enregistrer le chemin du fichier .wav dans le registre
        public static void SaveKey(string chemin, ILogger logger)
        {
            // Chemin de la clé du registre où nous allons stocker la valeur  

            // Tenter d'ouvrir la clé du registre
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName, true))
            {
                // Si la clé n'existe pas, la créer
                if (key == null)
                {
                    logger.LogInformation("La clé n'existe pas, création.");
                    using (RegistryKey newKey = Registry.LocalMachine.CreateSubKey(keyName))
                    {
                        try
                        {
                            newKey.SetValue("MusicFile", chemin, RegistryValueKind.String);
                            logger.LogInformation("La clé de registre a été crée.");
                        } catch (Exception ex)
                        {
                            logger.LogCritical($"Une erreur est survenue lors de la création de la clé de registre. \n Code d'erreur : {ex.Message}");
                        }
                        // Enregistrer la valeur dans la nouvelle clé du registre

                    }
                }
                else
                {
                    // La clé existe déjà, vous pouvez choisir de ne pas écraser la valeur existante ici
                    // Ou bien vous pouvez mettre à jour la valeur existante si vous le souhaitez
                    // Dans cet exemple, nous écrasons simplement la valeur existante
                    string value = (string)key.GetValue("MusicFile");
                    logger.LogInformation($"La clé a déja été crée (emplacement) : {value}");
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
                    value = appPath + "music.wav";
                } else
                {
                    value = (string)key.GetValue(name);
                }
            }
            return value;
        }
    }
}
