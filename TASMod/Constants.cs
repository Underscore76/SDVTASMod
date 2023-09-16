using System;
using System.IO;

namespace TASMod
{
    public class Constants
    {
        public static readonly string SavesPath;
        public static readonly string BasePath;
        public static readonly string ScreenshotPath;
        public static readonly string ProfilingPath;
        public static readonly string ErrorLogsPath;
        public static readonly string DisconnectLogsPath;
        public static readonly string SaveStatePath;
        public static readonly string ScriptsPath;

        static Constants()
        {

            BasePath = mkdir(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "StardewTAS");
            SavesPath = mkdir(BasePath, "Saves");
            ScreenshotPath = mkdir(BasePath, "Screenshots");
            ProfilingPath = mkdir(BasePath, "Profiling");

            ErrorLogsPath = mkdir(BasePath, "ErrorLogs");
            DisconnectLogsPath = mkdir(BasePath, "DisconnectLogs");

            SaveStatePath = mkdir(BasePath, "SaveStates");
            ScriptsPath = mkdir(BasePath, "Scripts");
        }

        private static string mkdir(string basePath, string subDir)
        {
            string path = Path.Combine(basePath, subDir);
            Directory.CreateDirectory(path);
            return path;
        }
    }
}

