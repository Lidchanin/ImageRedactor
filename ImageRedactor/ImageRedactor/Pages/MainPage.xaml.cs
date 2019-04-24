using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Essentials;
using Plugin.Media.Abstractions;
using Plugin.Permissions.Abstractions;

namespace ImageRedactor.Pages
{
    public partial class MainPage
    {
        MediaFile _photo;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void CameraButton_OnClicked(object sender, EventArgs e)
        {
            if (!Plugin.Media.CrossMedia.Current.IsCameraAvailable)
            {
                //await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                await Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
            }

            _photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(
            new StoreCameraMediaOptions
            {
            });

            PhotoImage.Source = ImageSource.FromStream(() => _photo.GetStreamWithImageRotatedForExternalStorage());
        }

        private async void SSButton_OnClicked(object sender, EventArgs e) =>
            await Navigation.PushAsync(new SSRealizationPage(_photo?.GetStreamWithImageRotatedForExternalStorage()));

        private async void XFButton_Clicked(object sender, EventArgs e) =>
            await Navigation.PushAsync(new XFRealizationPage(_photo?.GetStreamWithImageRotatedForExternalStorage()));
    }
}