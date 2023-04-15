using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Threading;
using System.Text.Json;
using System.Net.Http.Json;
using static TestApp1.MainPage;
using System.Linq;

namespace TestApp1.ApiCalls
{
    public class SaveImageApi
    {
        public static async Task CallAPI(ObservableCollection<StoredFiles> storedFiles, string Url)
        {
            try
            {
                StreamContent conent = null;
                HttpClient httpClient = new HttpClient();
                foreach (StoredFiles storedFile in storedFiles)
                {


                    storedFile.FileStream.Position = 0;

                    //if (streamLength != null)
                    //{
                    //    stream.SetLength(streamLength.Value);
                    //}

                    Console.WriteLine($"API Call started for Stream length {storedFile.FileStream.Length}");

                    conent = new StreamContent(storedFile.FileStream);
                }

                conent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://webapplicationchipfree.azurewebsites.net/ScreenShot"))
                {
                    request.Content = conent;

                    var result = await httpClient.SendAsync(request).ConfigureAwait(false);

                    var rr = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Console.WriteLine(rr);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Api Call Error : " + ex);
            }
        }

    }
}

