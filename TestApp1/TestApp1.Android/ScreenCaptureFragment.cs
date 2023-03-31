using Android;
using Android.App;
using Android.Content;
using Android.Media.Projection;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TestApp1.Droid;
using TestApp1.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(ScreenCaptureFragment))]
namespace TestApp1.Droid
{
    public class ScreenCaptureFragment : IScreen
    {
        public void StartWo()
        {
            StartForegroundServiceCompat<RecordingSerivce>(Xamarin.Essentials.Platform.AppContext);
        }

        public static void StartForegroundServiceCompat<T>(Context context, Bundle args = null) where T : Service
        {
            try
            {
                var intent = new Intent(context, typeof(T));
                if (args != null)
                {
                    intent.PutExtras(args);
                }
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                {
                    context.StartForegroundService(intent);
                }
                else
                {
                    context.StartService(intent);
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee);
            }
        }

        public const int REQUEST_RECORD_AUDIO_PERMISSION = 200;
        public void StartWorkForAudio()
        {
            MainActivity.OnRecordPermission = () => StartForegroundServiceCompat<RecordingSerivce>(Xamarin.Essentials.Platform.AppContext);
            ActivityCompat.RequestPermissions(Xamarin.Essentials.Platform.CurrentActivity, new string[] { Manifest.Permission.RecordAudio }, REQUEST_RECORD_AUDIO_PERMISSION);
        }

        public void Stop()
        {
            try
            {
                Context context = Android.App.Application.Context;
                Intent serviceIntent = new Intent(context, typeof(RecordingSerivce));
                PendingIntent pendingIntent = PendingIntent.GetService(context, 0, serviceIntent, PendingIntentFlags.UpdateCurrent);
                AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);
                alarmManager.Cancel(pendingIntent);
                context.StopService(serviceIntent);

                Console.WriteLine("Service Stopped.");
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee);
            }
        }
    }
}