using Microsoft.Extensions.Logging;
using OOBEMusic;

public class Logging
{
    public readonly ILogger<OOBEMusicPlayer> _logger; // Ajoutez un champ pour le logger
    public void Info (string message)
    {
        _logger.LogInformation(message);
    }
    public void Warn (string message)
    {
        _logger.LogWarning(message);
    }
    public void Error (string message)
    {
        _logger.LogError(message);
    }
    public void Fatal (string message)
    {
        _logger.LogCritical(message);
    }
}