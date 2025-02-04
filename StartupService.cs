using System.IO;

namespace Neshangar;

public class StartupService
{
    public static void AddToStartup()
    {
        string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        string shortcutPath = Path.Combine(startupFolder, "Neshangar.bat");
        string? appPath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;

        if (appPath is not null)
        {
            File.WriteAllText(shortcutPath, $"@echo off\nstart \"\" \"{appPath}\"");
        }
    }

    public static void RemoveFromStartup()
    {
        string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        string shortcutPath = Path.Combine(startupFolder, "Neshangar.bat");

        if (File.Exists(shortcutPath))
        {
            File.Delete(shortcutPath);
        }
    }

    public static bool IsInStartup()
    {
        string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        string shortcutPath = Path.Combine(startupFolder, "Neshangar.bat");

        if (File.Exists(shortcutPath))
        {
            return true;
        }

        return false;
    }
}