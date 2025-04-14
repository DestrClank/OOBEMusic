using Microsoft.Extensions.Logging;
using OOBEMusic;
using System.Diagnostics;
using System.Resources;

public class Logging
{
    public class EventLogger
    {
        private static readonly ResourceManager rm = new ResourceManager("OOBEMusic.Ressources.Messages", typeof(OOBEMusicPlayer).Assembly);
        public static void LogToEventViewer(string message, EventLogEntryType type)
        {
            string source = "OOBEMusic";
            string logName = "Application";

            try
            {
                if (!EventLog.SourceExists(source))
                {
                    // Créer la source d'événements
                    EventLog.CreateEventSource(new EventSourceCreationData(source, logName));
                }
                // Écrire dans le journal des événements
                EventLog.WriteEntry(source, message, type);
            }
            catch (System.Exception ex)
            {
                // En cas d'erreur, écrire dans le journal par défaut
                EventLog.WriteEntry(source, string.Format(rm.GetString("ErrorCantLogInEventViewer"), ex.Message), EventLogEntryType.Error);
            }
        }
    }
}