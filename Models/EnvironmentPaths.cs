using System;
using System.IO;

namespace PodRat.Models;

public static class EnvironmentPaths
{
    public static string ApplicationData { get; private set; }
    public static string Config { get; private set; }

    static EnvironmentPaths()
    {
        #if OS_WINDOWS
            ApplicationData = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PodRat");
            Config = ApplicationData;
        #elif OS_LINUX
            // Change folders if running in Flatpak, else stick to standard linux folder
            if (Environment.GetEnvironmentVariable("container") != null)
            {
                ApplicationData = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".var", "app", "io.github.jess_klost.podrat", "data");
                Config = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".var", "app", "io.github.jess_klost.podrat", "config");
            }
            else
            {
                ApplicationData = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".podrat");
                Config = ApplicationData;
            }
        #else
            ApplicationData = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PodRat");
            Config = ApplicationData;
        #endif
    }
}