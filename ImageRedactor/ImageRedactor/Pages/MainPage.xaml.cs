using System;

namespace ImageRedactor.Pages
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void XFButton_OnClicked(object sender, EventArgs e) =>
            await Navigation.PushAsync(new XFRealizationPage());

        private async void SSButton_OnClicked(object sender, EventArgs e) =>
            await Navigation.PushAsync(new SSRealizationPage());
    }
}
