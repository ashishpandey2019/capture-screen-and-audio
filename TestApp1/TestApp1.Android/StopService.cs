using Android.App;
using Android.Content;
using Android.Media.Projection;
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
    [Service(ForegroundServiceType = Android.Content.PM.ForegroundService.TypeMediaProjection)]
    public class StopService : Service
    {
        private MediaProjection _mediaProjection;
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnTaskRemoved(Intent rootIntent)
        {
            base.OnTaskRemoved(rootIntent);
            _mediaProjection.Stop();
        }

    }
}