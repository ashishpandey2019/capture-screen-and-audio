using Android.App;
using Android.Content;
using Android.Database;
using Android.Graphics;
using Android.Hardware.Display;
using Android.Media;
using Android.Media.Projection;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Interop;
using Java.IO;
using Java.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Java.Util.Jar.Attributes;
using AndroidApp = Android.App.Application;
using Console = System.Console;
namespace TestApp1.Droid
{
    [Service(ForegroundServiceType = Android.Content.PM.ForegroundService.TypeMediaProjection)]
    public class RecordingSerivce : Service
    {
        private string _channelName = "running";
        private string _channelId = "234";
        private const string TAG = "ScreenCaptureFragment";
        private MediaProjectionManager mediaProjectionManager;
        private VirtualDisplay virtualDisplay;
        private const int REQUEST_MEDIA_PROJECTION = 1;
        public static Action<Intent, int> StartActivityForResult;


        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {

            try
            {
                
                
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

        public void ServiceStopedNotification()
        {
            var notification = new Notification.Builder(this, _channelId)
             .SetContentTitle("Service Stopped.")
             .SetContentText("Foreground service has stopped.")
             .SetSmallIcon(Resource.Drawable.close)
             .Build();
            StartForeground(1, notification);
        }

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

                            Task.Run(() => SetAudio(mediaProjection));
                            SetUpVirtualDisplay(mediaProjection);
                        }
                    }
                }
                catch (Exception ee)
                {
                    Console.WriteLine(ee);
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

                virtualDisplay = mediaProjection.CreateVirtualDisplay(
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


        private void SetAudio(MediaProjection mediaProjection)
        {
            try
            {
                const int sampleRate = 48000;

                var audioPlaybackCaptureConfiguration = new AudioPlaybackCaptureConfiguration.Builder(mediaProjection)
                    .AddMatchingUsage(AudioUsageKind.Media)
                     .Build();

                var bufferSizeInBytes = AudioRecord.GetMinBufferSize(sampleRate, ChannelIn.Mono, Android.Media.Encoding.Pcm16bit);

                var audioFormat = new AudioFormat.Builder()
                    .SetEncoding(Android.Media.Encoding.Pcm16bit)
                    .SetChannelMask(ChannelOut.Mono)
                    .SetSampleRate(sampleRate)
                    .Build();

                var audioRecord = new AudioRecord.Builder()
                     .SetAudioPlaybackCaptureConfig(audioPlaybackCaptureConfiguration)
                     .SetBufferSizeInBytes(bufferSizeInBytes)
                     .SetAudioFormat(audioFormat)
                     //     .SetAudioSource(AudioSource.Mic)
                     .Build();

                var filePath = GetFilePath();
                var audioFile = new Java.IO.File(filePath);
                var outputStream = new FileOutputStream(audioFile);


                audioRecord.StartRecording();

                byte[] audioBuffer = new byte[bufferSizeInBytes];

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    var bytesRead = audioRecord.Read(audioBuffer, 0, bufferSizeInBytes);

                    if (bytesRead > 0)
                    {
                        outputStream.Write(audioBuffer, 0, bytesRead);
                    }
                }
                Console.WriteLine("Audio file is : "+ audioFile);
                audioRecord.Release();
                audioRecord.Dispose();
                outputStream.Flush();
                outputStream.Close();
                outputStream.Dispose();


                // mediaProjection.Stop();
                // mediaProjection.Dispose();
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee);
            }
        }

        private string GetFilePath()
        {
            //string folderPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/MyApp";

            var folderPath = Android.App.Application.Context.GetExternalFilesDir(string.Empty).AbsolutePath + "/Audio";
            Directory.CreateDirectory(folderPath);
            return System.IO.Path.Combine(folderPath, $"audio{System.IO.Path.GetRandomFileName()}");
        }
    }
}