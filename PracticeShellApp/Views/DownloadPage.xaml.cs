using Microsoft.Azure;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PracticeShellApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DownloadPage : ContentPage
    {
        public enum ContainerType
        {
            Image,
            Text
        }
        public DownloadPage()
        {
            InitializeComponent();
        }

        private async void downloadButton_Clicked(object sender, EventArgs e)
        {
            var uploadedFilename = "Gagana-Words.csv";
            if (!string.IsNullOrWhiteSpace(uploadedFilename))
            {
                activityIndicator.IsRunning = true;

                var byteData = await GetFileAsync(ContainerType.Text, uploadedFilename);
                var text = Encoding.UTF8.GetString(byteData);
                editor.Text = text;

                activityIndicator.IsRunning = false;
            }
        }

        CloudBlobContainer GetContainer(ContainerType containerType)
        {
            var account = Microsoft.Azure.Storage.CloudStorageAccount.Parse(Constants.StorageConnection);
            var client = account.CreateCloudBlobClient();

            return client.GetContainerReference("gagana-blob");

            //return client.GetContainerReference(containerType.ToString().ToLower());
        }

        public async Task<byte[]> GetFileAsync(ContainerType containerType, string name)
        {
            var container = GetContainer(containerType);

            var blob = container.GetBlobReference(name);
            if (await blob.ExistsAsync())
            {
                await blob.FetchAttributesAsync();
                byte[] blobBytes = new byte[blob.Properties.Length];

                await blob.DownloadToByteArrayAsync(blobBytes, 0);
                return blobBytes;
            }
            return null;
        }

    }
}