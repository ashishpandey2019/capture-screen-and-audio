using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestApp1.Droid
{
    public class CheckForServiceRunning
    {
        public static bool IsServiceRunning(Context context ,Type serviceType)
        {
            var manager = (ActivityManager)context.GetSystemService(Context.ActivityService);
            var runningServices = manager.GetRunningServices(int.MaxValue);

            foreach (var service in runningServices)
            {
                if (service.Service.ClassName.Equals(serviceType.FullName))
                {
                    return true;
                }
            }

            return false;
        }

    }
}