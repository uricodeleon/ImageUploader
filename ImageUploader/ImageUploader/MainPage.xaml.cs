using Microsoft.WindowsAzure.Storage;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ImageUploader
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

        //key
        //u0K1pcdk+OWM6TJdFPdBUyVJNbqkndbFxcZdYfQ4E8wuA7M8GseUlItLUBnJG3LeEkDrSoFzfzR9OgX5fNHrEQ==
        //connection
        //DefaultEndpointsProtocol=https;AccountName=mikkouploadedimages;AccountKey=u0K1pcdk+OWM6TJdFPdBUyVJNbqkndbFxcZdYfQ4E8wuA7M8GseUlItLUBnJG3LeEkDrSoFzfzR9OgX5fNHrEQ==;EndpointSuffix=core.windows.net

        private async void SelectImageButton_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
                
            if(!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Error", "File type not supported!", "Ok");
                return; 
            }
            var mediaOptions = new PickMediaOptions()
            {
                PhotoSize = PhotoSize.Medium
            };

            var selectedImageFile = await CrossMedia.Current.PickPhotoAsync();
            
            if(selectedImageFile == null)
            {
                await DisplayAlert("Error", "Error while trying to get image.", "Ok");
                return;
            }
            selectImage.Source = ImageSource.FromStream(() => selectedImageFile.GetStream());

            UploadImageFile(selectedImageFile.GetStream());
        }

        private async void UploadImageFile(Stream stream)
        {
            var account = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=mikkouploadedimages;AccountKey=u0K1pcdk+OWM6TJdFPdBUyVJNbqkndbFxcZdYfQ4E8wuA7M8GseUlItLUBnJG3LeEkDrSoFzfzR9OgX5fNHrEQ==;EndpointSuffix=core.windows.net");
            var client = account.CreateCloudBlobClient();
            var container = client.GetContainerReference("mikkoimagecontainer");
            await container.CreateIfNotExistsAsync();

            var name = Guid.NewGuid().ToString();
            var blockBlob = container.GetBlockBlobReference($"{name}.jpg");

            await blockBlob.UploadFromStreamAsync(stream);

            string url = blockBlob.Uri.OriginalString;
        }
    }
}
