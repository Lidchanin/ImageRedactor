using ImageRedactor.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using ImageRedactor.XFUtils;
using Xamarin.Forms;

namespace ImageRedactor.Pages
{
    public partial class XFRealizationPage
    {
        private readonly XFRealizationViewModel _viewModel;

        private TouchManipulationImage _image;
        List<long> touchIds = new List<long>();
        //private Rectangle _rectangle;

        public XFRealizationPage(Stream photoStream)
        {
            _viewModel = new XFRealizationViewModel(photoStream);

            BindingContext = _viewModel;

            InitializeComponent();
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
                HeightRequest= 200,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start
            };

            MainView.Children.Add(_image);

            //_rectangle = new Rectangle(0, 0, _image.Width, _image.Height);
        }

        //Point _offset;
        //private bool _isContains;

        void Handle_TouchAction(object sender, TouchActionEventArgs args)
        {
            var location = args.Location;

            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    if (_image.HitTest(location))
                    {
                        touchIds.Add(args.Id);
                        _image.ProcessTouchEvent(args.Id, args.Type, location);
                        break;
                    }

                    //touchIds.Add(args.Id);

                    //if (!_rectangle.Contains(location.X, location.Y))
                    //    return;

                    //_isContains = true;
                    //_offset = new Point(location.X - _image.TranslationX, location.Y - _image.TranslationY);

                    break;

                case TouchActionType.Moved:
                    if (touchIds.Contains(args.Id))
                    {
                        _image.ProcessTouchEvent(args.Id, args.Type, location);
                    }

                    break;

                case TouchActionType.Released:
                case TouchActionType.Cancelled:
                    //_rectangle = new Rectangle(_image.TranslationX, _image.TranslationY, _image.Width, _image.Height);
                    //_isContains = false;
                    //touchIds.Remove(args.Id);
                    if (touchIds.Contains(args.Id))
                    {
                        _image.ProcessTouchEvent(args.Id, args.Type, location);
                        touchIds.Remove(args.Id);
                    }

                    break;
            }
        }

        private static double GetDistance(Point newPoint, Point olderPoint) =>
            Math.Sqrt(Math.Pow((newPoint.X - olderPoint.X), 2) + Math.Pow((newPoint.Y - olderPoint.Y), 2));
    }
}