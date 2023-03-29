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

namespace TestApp1.Droid
{
    [Activity(Label = "TestApp1", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IOnImageAvailableListener
    {
        public static MainActivity mainActivity;
        public static Action OnRecordPermission;
        public static Action<int, Android.App.Result, Intent> OnActivityResultAction;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        Image _image;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            RecordingSerivce.StartActivityForResult = StartActivityForResult;
            mainActivity = this;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            //if (requestCode == ScreenCaptureFragment.REQUEST_RECORD_AUDIO_PERMISSION)
            //{
            //    OnRecordPermission?.Invoke();
            //}
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
                    return;
                }

                using var bitmap = toBitmap(_image, reader.Width, reader.Height);
                //  var bytes = GetBytes(_image);
                SaveImage(bitmap);


                _image?.Close();

                await Task.Delay(10000);

            }
            catch (Exception ee)
            {
                Console.WriteLine(ee);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private Bitmap toBitmap(Image image, int width, int height)
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
            try
            {
                var path = Android.App.Application.Context.GetExternalFilesDir(string.Empty).AbsolutePath;
                
                var filePath = System.IO.Path.Combine(path, System.IO.Path.GetRandomFileName() + ".jpeg");
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