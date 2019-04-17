using System;
using System.IO;
using Xamarin.Forms;

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
            var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(
                new Plugin.Media.Abstractions.StoreCameraMediaOptions());

            if (photo == null) return;

            PhotoImage.Source = ImageSource.FromStream(() => photo.GetStream());
            _photoStream = photo.GetStream();

            SSButton.IsVisible = true;
            XFButton.IsVisible = true;
        }

        private async void SSButton_OnClicked(object sender, EventArgs e) =>
            await Navigation.PushAsync(new SSRealizationPage(_photoStream));

        private async void XFButton_Clicked(object sender, EventArgs e) =>
            await Navigation.PushAsync(new XFRealizationPage(_photoStream));
    }
}