using Android.App;
using Android.Content;
using Android.Content.PM;
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
    [BroadcastReceiver(Enabled = true, Exported = true)]
    public class NotificationReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            // Call the StopForeground() method on the Service to stop the foreground service
            var serviceIntent = new Intent(context, typeof(RecordingSerivce));
            context.StopService(serviceIntent);
        }
    }
}