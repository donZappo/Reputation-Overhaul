﻿using System;
using System.IO;
using System.Reflection;

namespace Reputation_Overhaul
{
    public static class Logger
    {
        internal static string LogFilePath =>
            Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName + "\\Reputation_Overhaul.txt";

        public static void Error(Exception ex)
        {
            using (var writer = new StreamWriter(LogFilePath, true))
            {
                writer.WriteLine($"Message: {ex.Message}");
                writer.WriteLine($"StackTrace: {ex.StackTrace}");
                writer.WriteLine($"Source: {ex.Source}");
                writer.WriteLine($"Data: {ex.Data}");
            }
        }

        public static void LogDebug(string line)
        {
            if (!Core.Settings.Debug) return;
            using (var writer = new StreamWriter(LogFilePath, true))
            {
                writer.WriteLine(line);
            }
        }

        public static void Log(string line)
        {
            using (var writer = new StreamWriter(LogFilePath, true))
            {
                writer.WriteLine(line);
            }
        }

        public static void Clear()
        {
            if (!Core.Settings.Debug) return;
            using (var writer = new StreamWriter(LogFilePath, false))
            {
                writer.WriteLine($"{DateTime.Now.ToLongTimeString()} Reputation Overhaul Init");
            }
        }
    }
}
