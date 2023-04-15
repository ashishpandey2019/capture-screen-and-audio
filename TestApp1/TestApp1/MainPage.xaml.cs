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
using System.Collections.Generic;
using static TestApp1.MainPage;
using TestApp1.ApiCalls;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Shapes;
using Xamarin.Essentials;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Timers;

namespace TestApp1
{
    public partial class MainPage : ContentPage
    {
        private const string ScreenshotUrl = "https://webapplicationchipfree.azurewebsites.net/ScreenShot";
        private const string AudioUrl = "https://webapplicationchipfree.azurewebsites.net/ScreenShot/PostAudio";
        private bool _isFirstTime = true;

        public MainPage()
        {
            InitializeComponent();
            StartTimer();
            try
            {
                var errorfiles = DependencyService.Get<IGetFile>().GetErrorFiles();
                if(errorfiles != null)
                {
                    timerLabel.Text = errorfiles;
                }
            }
            catch (Exception)
            {

                throw;
            }
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

        private void Stop_Button_Clicked(object sender, EventArgs e)
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
        ObservableCollection<StoredFiles> storedFiles = new ObservableCollection<StoredFiles>();
        ObservableCollection<StoredFiles> storedAudioFiles = new ObservableCollection<StoredFiles>();
        public class StoredFiles
        {
            public string FileName { get; set; }
            public Stream FileStream { get; set; }
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            try
            {
                storedFiles.Clear();

                var path = DependencyService.Get<IGetFile>().GetImagePath();

                var files = Directory.GetFiles(path);

                var data = files.Select(x => new { date = new FileInfo(x).CreationTime, source = ImageSource.FromFile(x) })
                 .OrderByDescending(x => x.date)
                 .ToList();

                DataList.ItemsSource = data;
                filecountlbl.Text = data.Count.ToString();  

                string[] fileNames = Directory.GetFiles(path);

                if (!fileNames.Any())
                {
                    return;
                }


                foreach (string filePath in fileNames)
                {
                    string fileName = System.IO.Path.GetFileName(filePath);
                    Stream fileStream = File.OpenRead(filePath);
                    storedFiles.Add(new StoredFiles { FileName = fileName, FileStream = fileStream });
                }

                Console.WriteLine("Stored Files Are: " + storedFiles.Count);

                //var response = SaveImageApi.CallAPI(storedFiles, ScreenshotUrl);


            }
            catch (Exception ee)
            {

            }
        }




        private void Delete_Button_Clicked_2(object sender, EventArgs e)
        {
            try
            {
                var path = DependencyService.Get<IGetFile>().GetImagePath();
                var files = Directory.GetFiles(path);
                if (files.Any())
                {
                    Directory.Delete(path, true);
                    Console.WriteLine("File path deleted");
                }
            }
            catch (Exception ee)
            {

            }
        }

        private void SaveAudio(object sender, EventArgs e)
        {
            try
            {
                storedAudioFiles.Clear();
                var _audioFolderPath = DependencyService.Get<IGetFile>().GetAudioFiles();
                string[] fileNames = Directory.GetFiles(_audioFolderPath);
                if (!fileNames.Any())
                {
                    return;
                }
                foreach (string filePath in fileNames)
                {
                    string fileName = System.IO.Path.GetFileName(filePath);
                    Stream fileStream = File.OpenRead(filePath);
                    storedAudioFiles.Add(new StoredFiles { FileName = fileName, FileStream = fileStream });
                }
                Console.WriteLine("Stored Audio Files Are: " + storedAudioFiles.Count);

                //var response = SaveImageApi.CallAPI(storedAudioFiles, AudioUrl);
                //if (response != null)
                //{
                //    Directory.Delete(_audioFolderPath, true);
                //}

            }


            catch (Exception ee)
            {

            }
        }

        private System.Timers.Timer _timer;

        public void StartTimer()
        {
            _timer = new System.Timers.Timer(300000);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (storedAudioFiles.Count > 0)
            {
                var response = SaveImageApi.CallAPI(storedAudioFiles, AudioUrl);
                var _audioFolderPath = DependencyService.Get<IGetFile>().GetAudioFiles();
                Directory.Delete(_audioFolderPath, true);
            }

            if (storedFiles.Count > 0)
            {
                var response = SaveImageApi.CallAPI(storedFiles, ScreenshotUrl);

                var path = DependencyService.Get<IGetFile>().GetImagePath();
                Directory.Delete(path, true);
            }
        }

    }
}
