using System;
using System.Collections.Generic;
using System.IO;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace ImageRedactor.Pages
{
    public partial class SSRealizationPage : ContentPage
    {
        SKCanvasView _skiaView;
        GestureManipulationBitmap _skImage;
        List<long> touchIds = new List<long>();

        SKBitmap _photoBmp;

        string[] _images;

        List<ImageButton> _imageButtons;

        public SSRealizationPage(Stream photoStream)
        {
            InitializeComponent();

            _images = new string[]
            {
                "ImageRedactor.Images.banana1.png",
                "ImageRedactor.Images.banana3.png",
                "ImageRedactor.Images.mask1.png",
                "ImageRedactor.Images.mask2.png",
                "ImageRedactor.Images.mask3.png",
                "ImageRedactor.Images.monkey1.png",
                "ImageRedactor.Images.monkey2.png",
                "ImageRedactor.Images.monkey3.png",
            };

            _imageButtons = new List<ImageButton>();

            _skiaView = new SKCanvasView();
            _skiaView.PaintSurface += SkiaViewPaintSurfaceHangle;
            _skiaView.EnableTouchEvents = true;

            if (photoStream != null)
            {
                _photoBmp = SKBitmap.Decode(photoStream);
            }
            else
            {
                _photoBmp = BitmapExtensions.LoadBitmapResource("ImageRedactor.Images.banana1.png"); ;

            }

            //var box = new BoxView();
            //box.BackgroundColor = Color.Transparent;

            var grid = new Grid();
            grid.Children.Add(_skiaView);

            var scroll = new ScrollView
            {
                Orientation = ScrollOrientation.Horizontal,
                VerticalOptions = LayoutOptions.End,
            };

            var stack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal
            };

            foreach (var name in _images)
            {
                var button = new ImageButton();
                button.HeightRequest = 100;
                button.WidthRequest = 100;
                button.Source = ImageSource.FromStream(() => { return BitmapExtensions.LoadStreamResource(name); });
                button.Clicked += Button_Clicked;

                _imageButtons.Add(button);

                stack.Children.Add(button);
            }

            scroll.Content = stack;
            grid.Children.Add(scroll);

            Content = grid;

            //var panRecognizer = new PanGestureRecognizer();
            //panRecognizer.PanUpdated += PanRecognizer_PanUpdated;

            //var pinchRecognizer = new PinchGestureRecognizer();
            //pinchRecognizer.PinchUpdated += PinchRecognizer_PinchUpdated;

            //box.GestureRecognizers.Add(panRecognizer);
            //box.GestureRecognizers.Add(pinchRecognizer);
            _skiaView.Touch += _skiaView_Touch;
        }

        void Button_Clicked(object sender, EventArgs e)
        {
            var control = (ImageButton)sender;

            var index = _imageButtons.IndexOf(control);

            _skImage = new GestureManipulationBitmap(BitmapExtensions.LoadBitmapResource(_images[index]));
            _skImage.TouchManager.Mode = TouchManipulationMode.ScaleRotate;
            _skImage.Matrix = SKMatrix.MakeTranslation((_skiaView.CanvasSize.Width * 0.5f) - _skImage.Bitmap.Width * 0.5f, (_skiaView.CanvasSize.Height * 0.5f) - _skImage.Bitmap.Height * 0.5f);

            _skiaView.InvalidateSurface();
        }

        void SkiaViewPaintSurfaceHangle(object sender, SKPaintSurfaceEventArgs e)
        {
            var info = e.Info;
            var surface = e.Surface;
            var canvas = surface.Canvas;

            canvas.Clear();

            canvas.DrawBitmap(_photoBmp, info.Rect, BitmapStretch.Uniform);

            if (_skImage != null)
            {
                _skImage.Paint(canvas);
            }
        }

        void Handle_TouchAction(object sender, TouchActionEventArgs args)
        {
            //Point pt = args.Location;
            //SKPoint point =
            //    new SKPoint((float)(_skiaView.CanvasSize.Width * pt.X / _skiaView.Width),
            //                (float)(_skiaView.CanvasSize.Height * pt.Y / _skiaView.Height));

            //switch (args.Type)
            //{
            //    case TouchActionType.Pressed:
            //        if (bitmap.HitTest(point))
            //        {
            //            touchIds.Add(args.Id);
            //            bitmap.ProcessTouchEvent(args.Id, args.Type, point);
            //            break;
            //        }
            //        break;

            //    case TouchActionType.Moved:
            //        if (touchIds.Contains(args.Id))
            //        {
            //            bitmap.ProcessTouchEvent(args.Id, args.Type, point);
            //            _skiaView.InvalidateSurface();
            //        }
            //        break;

            //    case TouchActionType.Released:
            //    case TouchActionType.Cancelled:
            //        if (touchIds.Contains(args.Id))
            //        {
            //            bitmap.ProcessTouchEvent(args.Id, args.Type, point);
            //            touchIds.Remove(args.Id);
            //            _skiaView.InvalidateSurface();
            //        }
            //        break;
            //}
        }

        void HandleAction(View arg1, object arg2)
        {
        }

        void PanRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            Console.WriteLine($"Pan: {new SKPoint((float)e.TotalX, (float)e.TotalY)}");
            //Point pt = .Location;
            //SKPoint point =
            //new SKPoint((float)(_skiaView.CanvasSize.Width * pt.X / _skiaView.Width),
            //(float)(_skiaView.CanvasSize.Height * pt.Y / _skiaView.Height));

            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    //if (bitmap.HitTest(point))
                    //{
                    //    touchIds.Add(args.Id);
                    //    bitmap.ProcessTouchEvent(args.Id, args.Type, point);
                    //    break;
                    //}
                    break;
                case GestureStatus.Running:
                    break;
                case GestureStatus.Canceled:
                case GestureStatus.Completed:
                    break;
            }
        }

        float _currentScale = 1;
        float _startScale;

        void PinchRecognizer_PinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            switch (e.Status)
            {
                case GestureStatus.Started:
                    _startScale = _currentScale;
                    //if (bitmap.HitTest(point))
                    //{
                    //    touchIds.Add(args.Id);
                    //    bitmap.ProcessTouchEvent(args.Id, args.Type, point);
                    //    break;
                    //}
                    break;
                case GestureStatus.Running:
                    _currentScale += ((float)e.Scale - 1f) * _startScale;
                    //_currentScale = Math.Max(0f, _currentScale);

                    break;
                case GestureStatus.Canceled:
                case GestureStatus.Completed:
                    break;
            }

            Console.WriteLine($"Pinch: {e.Scale}, {e.ScaleOrigin}\nCurrent Scale {_currentScale}");
        }

        void _skiaView_Touch(object sender, SKTouchEventArgs e)
        {
            if (_skImage == null)
                return;

            //Console.WriteLine($"SkiaTouch:\nActionType: {e.ActionType}\nDeviceType: {e.DeviceType}\nId: {e.Id}\nLocation: {e.Location}");
            e.Handled = true;

            Point pt = e.Location.ToFormsPoint();
            SKPoint point = e.Location;
            //new SKPoint((float)(_skiaView.CanvasSize.Width * pt.X / _skiaView.Width),
            //(float)(_skiaView.CanvasSize.Height * pt.Y / _skiaView.Height));

            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    if (_skImage.HitTest(point))
                    {
                        touchIds.Add(e.Id);
                        _skImage.ProcessTouchEvent(e.Id, e.ActionType, point);
                        break;
                    }
                    break;

                case SKTouchAction.Moved:
                    if (touchIds.Contains(e.Id))
                    {
                        _skImage.ProcessTouchEvent(e.Id, e.ActionType, point);
                        _skiaView.InvalidateSurface();
                    }
                    break;

                case SKTouchAction.Released:
                case SKTouchAction.Cancelled:
                    if (touchIds.Contains(e.Id))
                    {
                        _skImage.ProcessTouchEvent(e.Id, e.ActionType, point);
                        touchIds.Remove(e.Id);
                        _skiaView.InvalidateSurface();
                    }
                    break;
            }
        }
    }
}