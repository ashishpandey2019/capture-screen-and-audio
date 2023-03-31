using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp1.Interfaces;
using Xamarin.Forms;

namespace TestApp1
{
    public partial class MainPage : ContentPage
    {
        private bool _isFirstTime = true;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                var hh = await new Xamarin.Essentials.Permissions.StorageRead().RequestAsync();
                var ftjh = await new Xamarin.Essentials.Permissions.StorageWrite().RequestAsync();
                var ht = new Xamarin.Essentials.Permissions.Media().RequestAsync();

                DependencyService.Get<IScreen>()?.StartWo();
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee);  
            }

        }

        private void Stop_Button_Clicked(object sender,EventArgs e)
        {
            try
            {
                DependencyService.Get<IScreen>().Stop();
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee);
            }
        }

        private async void Record_Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                var hh = await new Xamarin.Essentials.Permissions.StorageRead().RequestAsync();
                var ftjh = await new Xamarin.Essentials.Permissions.StorageWrite().RequestAsync();
                var ht = new Xamarin.Essentials.Permissions.Media().RequestAsync();

                DependencyService.Get<IScreen>()?.StartWorkForAudio();
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee);
            }

        }

    }
}
