using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using static Android.Media.ImageReader;
using Android.Media;
using Android.Content;
using System.Threading;
using System.Threading.Tasks;
using Java.Nio;
using Android.Graphics;
using System.IO;
using Java.Util.Logging;
using TestApp1.Interfaces;
using Xamarin.Forms;
using TestApp1.ApiCalls;
using System.Buffers;
using System.Runtime.InteropServices.ComTypes;
using TestApp1.Droid.CustomClass;

namespace TestApp1.Droid
{
    [Activity(Label = "TestApp1", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IOnImageAvailableListener
    {
        public static MainActivity mainActivity;
        public static Action OnRecordPermission;
        private static bool _isScreenLocked = false;
        private const string TAG = "ScreenCaptureError";

        public static Action<int, Android.App.Result, Intent> ImageNotAvailableAction;
        public static Action<int, Android.App.Result, Intent> OnActivityResultAction;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly RecordingSerivce _recordingSerivce = new RecordingSerivce();

        Android.Media.Image _image;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            RecordingSerivce.StartActivityForResult = StartActivityForResult;
            mainActivity = this;


            var filter = new IntentFilter(Intent.ActionScreenOff);
            filter.AddAction(Intent.ActionUserPresent);
            var receiver = new LockScreenReceiver();
            RegisterReceiver(receiver, filter);

        }

        public class LockScreenReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                //bool isMyServiceRunning = CheckForServiceRunning.IsServiceRunning(mainActivity, typeof(RecordingSerivce));
                //if (isMyServiceRunning)
                //{
                //    Console.WriteLine("Service is running");
                //}
                //else
                //{
                //    Console.WriteLine("Service Stopped");
                //}

                if (intent.Action == Intent.ActionScreenOff)
                {
                    // The screen is locked
                    _isScreenLocked = true;
                }
                else if (intent.Action == Intent.ActionUserPresent)
                {
                    // The screen is unlocked
                    _isScreenLocked = false;
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if (requestCode == ScreenCaptureFragment.REQUEST_RECORD_AUDIO_PERMISSION)
            {
                OnRecordPermission?.Invoke();
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            OnActivityResultAction?.Invoke(requestCode, resultCode, data);
            base.OnActivityResult(requestCode, resultCode, data);
        }

        public async void OnImageAvailable(ImageReader reader)
        {
            try
            {

                await _semaphoreSlim.WaitAsync();
                _image = reader.AcquireNextImage();
               
                if (_image is null)
                {
                    _recordingSerivce.ServiceStopedNotification();
                    return;
                }

                using var bitmap = toBitmap(_image, reader.Width, reader.Height);
                // var bytes = GetBytes(_image);


                //ByteBuffer buffer = _image.GetPlanes()[0].Buffer;
                //byte[] bytes = new byte[buffer.Capacity()];

                //MemoryStream _stream = new MemoryStream(bytes, 0, bytes.Length, true, true);

                SaveImage(bitmap);

                _image?.Close();
                await Task.Delay(10000);

            }
            catch (Exception ee)
            {
                LogHelper.LogError(TAG, ee.Message);
                Console.WriteLine(ee);
            }
            finally
            {
                _semaphoreSlim.Release();

            }
        }

        private Bitmap toBitmap(Android.Media.Image image, int width, int height)
        {
            var pl = image.GetPlanes();
            ByteBuffer buffer = image.GetPlanes()[0].Buffer;

            int pixelStride = image.GetPlanes()[0].PixelStride;
            int rowStride = image.GetPlanes()[0].RowStride;
            int rowPadding = rowStride - pixelStride * width;

            if (pixelStride != 0)
            {
                width += rowPadding / pixelStride;
            }
            Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);

            byte[] data = new byte[buffer.Remaining()];
            buffer.Get(data);


            bitmap.CopyPixelsFromBuffer(ByteBuffer.Wrap(data));
            return bitmap;
        }

        private void SaveImage(Bitmap bitmap)
        {
            var filePath = "";
            try
            {
                if (_isScreenLocked)
                {
                    return;
                }
                var path = Android.App.Application.Context.GetExternalFilesDir(string.Empty).AbsolutePath;

                filePath = System.IO.Path.Combine(path, System.IO.Path.GetRandomFileName() + ".jpeg");
                using var fileStream = System.IO.File.Create(filePath);

                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 60, fileStream);

                Console.WriteLine("Saving image at : " + filePath);



            }
            catch (Exception ee)
            {
                Console.WriteLine(ee);
            }
        }
    }
}