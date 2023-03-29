using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
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
    }
}