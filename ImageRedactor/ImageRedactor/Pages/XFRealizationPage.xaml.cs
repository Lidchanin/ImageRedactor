using ImageRedactor.Services;
using ImageRedactor.ViewModels;
using ImageRedactor.XFUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ImageRedactor.Pages
{
    public partial class XFRealizationPage
    {
        private readonly XFRealizationViewModel _viewModel;

        private TouchManipulationImage _image;
        private readonly BoxView _boxView;
        private readonly List<long> _touchIds = new List<long>();

        public XFRealizationPage(Stream photoStream)
        {
            _viewModel = new XFRealizationViewModel(photoStream);

            BindingContext = _viewModel;

            InitializeComponent();

            _boxView = new BoxView
            {
                WidthRequest = 200,
                HeightRequest = 200,
                //BackgroundColor = Color.Red,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                AnchorX =0.5f,
                AnchorY = 0.5f,
            };

            MainView.Children.Add(_boxView);
        }

        private void ThumbnailImage_OnTapped(object sender, EventArgs e)
        {
            var thumbnailImage = (Image) sender;
            var commandParameter = ((TappedEventArgs) e).Parameter.ToString();

            if (_image != null)
                MainView.Children.Remove(_image);

            var touchEffect = new TouchEffect {Capture = true};
            touchEffect.TouchAction += Handle_TouchAction;

            _image = new TouchManipulationImage
            {
                Source = ImageSource.FromFile(commandParameter),
                WidthRequest = 200,
                HeightRequest = 200,
                //BackgroundColor = Color.Yellow,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                AnchorX = 0.5f,
                AnchorY = 0.5f,
            };

            MainView.Children.Add(_image);

            _image.ImageRectangle = new Rectangle(TranslationX, TranslationY, _image.Width, _image.Height);
            _image.ImageBoxView = _boxView;
        }

        private void Handle_TouchAction(object sender, TouchActionEventArgs args)
        {
            var location = args.Location;

            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    //Debug.WriteLine("\n------------------------------------"+_image.HitTest(location));
                    if(_image == null)
                        return;

                    if (_image.HitTest(location))
                    {
                        _touchIds.Add(args.Id);
                        _image.ProcessTouchEvent(args.Id, args.Type, location);
                    }
                    break;
                case TouchActionType.Moved:
                    if (_touchIds.Contains(args.Id))
                    {
                        _image.ProcessTouchEvent(args.Id, args.Type, location);
                    }
                    break;
                case TouchActionType.Released:
                case TouchActionType.Cancelled:
                    if (_touchIds.Contains(args.Id))
                    {
                        _image.ProcessTouchEvent(args.Id, args.Type, location);
                        _touchIds.Remove(args.Id);
                    }
                    break;
            }
        }

        private async void SaveButton_OnClicked(object sender, EventArgs e)
        {
            ImageSelectorScrollView.IsVisible = false;
            SaveButton.IsVisible = false;

            try
            {
                DependencyService.Get<IViewSnapshotService>().MakeViewSnapshotAndSave(MainView, DateTime.Now.ToFileTime() + ".png");
            }
            catch (Exception ex)
            {
            }

            ImageSelectorScrollView.IsVisible = true;
            SaveButton.IsVisible = true;
        }
    }
}