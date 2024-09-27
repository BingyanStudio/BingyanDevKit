using System.IO;
using System.Text;
using UnityEngine;

public static class SimpleLogger
{
    public const string LOG_PATH = "last_log.log";

    private static StringBuilder buffer = new();
    private static FileStream fs;

    public static void Enable() => Enable(LOG_PATH);
    public static void Enable(string filePath)
    {
        fs?.Flush();
        fs?.Close();

        buffer.Clear();
        var path = $"{Application.persistentDataPath}/{filePath}";
        if (!File.Exists(path))
        {
            Directory.CreateDirectory(path.Substring(0, path.LastIndexOf('/') + 1));
            File.Create(path);
        }
        fs = File.OpenWrite(path);
        Application.logMessageReceived += HandleLog;
        Application.quitting += EndLog;
    }

    public static void Disable()
    {
        Application.logMessageReceived -= HandleLog;
        Application.quitting -= EndLog;
        fs?.Flush();
        fs?.Close();
        fs = null;
    }

    private static void HandleLog(string msg, string stackTrace, LogType type)
    {
        buffer.AppendLine($"[{System.DateTime.Now}]");
        buffer.AppendLine(msg);
        buffer.AppendLine(stackTrace);
        buffer.AppendLine();
        fs.Write(Encoding.UTF8.GetBytes(buffer.ToString()));
        buffer.Clear();
    }

    private static void EndLog()
    {
        fs?.Flush();
        fs?.Close();
        fs.Dispose();
    }
}