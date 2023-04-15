using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TestApp1.Droid.CustomClass;
using TestApp1.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(GetImagePathDroid))]
namespace TestApp1.Droid.CustomClass
{
    public class GetImagePathDroid : IGetFile
    {
        public string GetAudioFiles()
        {
            var folderPath = Android.App.Application.Context.GetExternalFilesDir(string.Empty).AbsolutePath + "/Audio";
            return folderPath;

        }

        public string GetErrorFiles()
        {
            return LogHelper.GetErrorLogContents();
        }

        public string GetImagePath()
        {
            var path = Android.App.Application.Context.GetExternalFilesDir(string.Empty).AbsolutePath;
            return path;
        }

    }

    public static class LogHelper
    {
        public static void LogError(string tag, string message)
        {
            try
            {
                Log.Error(tag, message);

                // Save log to a file
                string logFilePath = Path.Combine(
                    Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath,
                    "error_log.txt");

                using (StreamWriter writer = System.IO.File.AppendText(logFilePath))
                {
                    writer.WriteLine($"[{DateTime.Now}] [{tag}] {message}");
                }
            }
            catch (Exception ex)
            {
                Log.Error(tag, $"Error saving log to file: {ex.Message}");
            }
        }

        public static string GetErrorLogContents()
        {
            string logFilePath = Path.Combine(
                     Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath,
                     "error_log.txt");

            if (!System.IO.File.Exists(logFilePath))
            {
                return "Error log file does not exist.";
            }

            using (StreamReader reader = new StreamReader(logFilePath))
            {
                return reader.ReadToEnd();
            }
        }
    }
}