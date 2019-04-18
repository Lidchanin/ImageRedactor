using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace ImageRedactor.Pages
{
    public partial class MainPage
    {
        private Stream _photoStream;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void CameraButton_OnClicked(object sender, EventArgs e)
        {
            if (!Plugin.Media.CrossMedia.Current.IsCameraAvailable)
            {
                DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                return;
            }

            var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(
            new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                AllowCropping = true,
                SaveMetaData = false
            });

            PhotoImage.Source = ImageSource.FromStream(() => photo.GetStream());
            _photoStream = photo.GetStream();
        }

        private async void SSButton_OnClicked(object sender, EventArgs e) =>
            await Navigation.PushAsync(new SSRealizationPage(_photoStream));

        private async void XFButton_Clicked(object sender, EventArgs e) =>
            await Navigation.PushAsync(new XFRealizationPage(_photoStream));
    }
}