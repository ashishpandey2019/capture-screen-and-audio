using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware.Display;
using Android.Media;
using Android.Media.Projection;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndroidApp = Android.App.Application;
using Console = System.Console;
namespace TestApp1.Droid
{
    [Service(ForegroundServiceType = Android.Content.PM.ForegroundService.TypeMediaProjection)]
    public class RecordingSerivce : Service
    {
        private const string TAG = "ScreenCaptureFragment";
        private MediaProjectionManager mediaProjectionManager;
        public override IBinder OnBind(Intent intent)
        {
           return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {

            try
            {
                string _channelName = "running";
                string _channelId = "234";

                var _manager = (NotificationManager)AndroidApp.Context.GetSystemService(AndroidApp.NotificationService);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var channelNameJava = new Java.Lang.String(_channelName);
                    var channel = new NotificationChannel(_channelId, channelNameJava, NotificationImportance.Default)
                    {
                        Description = "service rexcon"
                    };
                    _manager.CreateNotificationChannel(channel);
                }


                var notification = new Notification.Builder(this, _channelId)
                .SetContentTitle("Foreground Service")
                .SetContentText("This is a foreground service.")
                .SetSmallIcon(Resource.Drawable.icon)
                .Build();

                StartForeground(1, notification);
                Work();
            }
            catch (Exception ee)
            {
            }
            return StartCommandResult.Sticky;

        }

        private const int REQUEST_MEDIA_PROJECTION = 1;
        public static Action<Intent, int> StartActivityForResult;

        private void Work()
        {
            mediaProjectionManager = (MediaProjectionManager)GetSystemService(Context.MediaProjectionService);


            MainActivity.OnActivityResultAction = (requestCode, result, intent) =>
            {
                try
                {
                    if (requestCode == REQUEST_MEDIA_PROJECTION)
                    {
                        if (result == Result.Ok)
                        {
                            var mediaProjection = mediaProjectionManager.GetMediaProjection((int)result, intent);

                            // Task.Run(()=>   SetAudio(mediaProjection));
                            SetUpVirtualDisplay(mediaProjection);
                        }
                    }
                }
                catch (Exception ee)
                {
                }
            };

            StartActivityForResult(mediaProjectionManager.CreateScreenCaptureIntent(), REQUEST_MEDIA_PROJECTION);
        }

        private void SetUpVirtualDisplay(MediaProjection mediaProjection)
        {
            try
            {

                var screenDensity = (int)Resources.DisplayMetrics.DensityDpi;

                var height = Resources.DisplayMetrics.HeightPixels;
                var width = Resources.DisplayMetrics.WidthPixels;


                var imageReader = ImageReader.NewInstance(width, height, (ImageFormatType)Format.Rgba8888, 2);


                imageReader.SetOnImageAvailableListener(MainActivity.mainActivity, null);

                Log.Info(TAG, "Setting up a VirtualDisplay: " + imageReader.Width + "x" + imageReader.Height + " (" + screenDensity + ")");

                var virtualDisplay = mediaProjection.CreateVirtualDisplay(
                    "ScreenCapture",
                     imageReader.Width,
                     imageReader.Height,
                     screenDensity,
                     (DisplayFlags)VirtualDisplayFlags.AutoMirror,
                     imageReader.Surface,
                     null,
                     null);
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee);
            }
        }
    }
}