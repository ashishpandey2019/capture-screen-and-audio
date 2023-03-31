using TestApp1.iOS;

using AVFoundation;

using CoreGraphics;

using CoreMedia;

using Foundation;

using MediaPlayer;

using ReplayKit;

using System;
using System.IO;
using System.Timers;

using TestApp1.Interfaces;
using UIKit;

namespace TestApp1.iOS
{
    internal class ScreenRecording : UIViewController, IScreen
    {
        private readonly object _lockObject = new object();
        private bool _isAllowed;
        public void StartWo()
        {
            Timer timer = new Timer(5000)
            {
                AutoReset = true
            };
            timer.Elapsed += Timer_Elapsed;

            MPMediaLibrary.RequestAuthorization(h =>
            {
                try
                {
                    if (h != MPMediaLibraryAuthorizationStatus.Authorized)
                    {
                        return;
                    }

                    var bundle = NSBundle.MainBundle.GetUrlForResource("Extension12", "appex");

                    var frame = new CGRect(100, 100, 60, 60);
                    var broadcastPicker = new RPSystemBroadcastPickerView(frame);
                    var bundle2 = new NSBundle(bundle);
                    broadcastPicker.PreferredExtension = bundle2.BundleIdentifier;
                    View.AddSubview(broadcastPicker);



                    var extensionContext = new NSExtensionContext();
                    extensionContext.LoadBroadcastingApplicationInfo(LoadBroadcastingHandler);

                    extensionContext.CompleteRequest(null, null);



                    Console.WriteLine("Run RPSystemBroadcastPickerView");
                }
                catch (Exception ee)
                {
                }

            });
        }


        public void LoadBroadcastingHandler(string bundleID, string displayName, UIImage appIcon)
        {

        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _isAllowed = true;
        }

        public void StartWorkForAudio()
        {
            throw new NotImplementedException();
        }
        private NSTimer _timer;
        public void Stop()
        {
            _timer?.Invalidate();
            _timer = null;
        }

    }
}