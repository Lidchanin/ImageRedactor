using System;
using System.Collections.Generic;
using System.IO;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Essentials;
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

        Button _saveButton;

        SKBitmap _saveBitmap;

        int CurrentTime => (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

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
            _saveButton = new Button();
            _saveButton.Text = "SAVE";
            _saveButton.Clicked += SaveButtonClickedHandler;
            _saveButton.VerticalOptions = LayoutOptions.Start;

            var grid = new Grid();
            grid.Children.Add(_skiaView);
            grid.Children.Add(_saveButton);

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

        protected override void OnAppearing()
        {
            base.OnAppearing();

            UpdateBitmap();
        }

        void Button_Clicked(object sender, EventArgs e)
        {
            var control = (ImageButton)sender;

            var index = _imageButtons.IndexOf(control);

            _skImage = new GestureManipulationBitmap(BitmapExtensions.LoadBitmapResource(_images[index]));
            _skImage.TouchManager.Mode = TouchManipulationMode.ScaleRotate;
            _skImage.Matrix = SKMatrix.MakeTranslation((_skiaView.CanvasSize.Width * 0.5f) - _skImage.Bitmap.Width * 0.5f, (_skiaView.CanvasSize.Height * 0.5f) - _skImage.Bitmap.Height * 0.5f);

            UpdateBitmap();
        }

        void SkiaViewPaintSurfaceHangle(object sender, SKPaintSurfaceEventArgs e)
        {
            var info = e.Info;
            var surface = e.Surface;
            var canvas = surface.Canvas;

            // Create bitmap the size of the display surface
            if (_saveBitmap == null)
            {
                _saveBitmap = new SKBitmap(info.Width, info.Height);
            }

            // Render the bitmap
            canvas.Clear();
            canvas.DrawBitmap(_saveBitmap, 0, 0);
        }

        void UpdateBitmap()
        {
            using (SKCanvas saveBitmapCanvas = new SKCanvas(_saveBitmap))
            {
                saveBitmapCanvas.Clear();

                saveBitmapCanvas.DrawBitmap(_photoBmp, SKRect.Create(0, 0, _saveBitmap.Width, _saveBitmap.Height), BitmapStretch.Uniform);

                if (_skImage != null)
                {
                    _skImage.Paint(saveBitmapCanvas);
                }
            }

            _skiaView.InvalidateSurface();
        }

        void _skiaView_Touch(object sender, SKTouchEventArgs e)
        {
            if (_skImage == null)
                return;

            e.Handled = true;

            SKPoint point = e.Location;

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
                        UpdateBitmap();
                    }
                    break;

                case SKTouchAction.Released:
                case SKTouchAction.Cancelled:
                    if (touchIds.Contains(e.Id))
                    {
                        _skImage.ProcessTouchEvent(e.Id, e.ActionType, point);
                        touchIds.Remove(e.Id);
                        UpdateBitmap();
                    }
                    break;
            }
        }

        void SaveButtonClickedHandler(object sender, EventArgs e)
        {
            var format = SKEncodedImageFormat.Png;
            var file = Path.Combine(FileSystem.AppDataDirectory, $"IMG_{CurrentTime}.{format.ToString()}");

            using (SKImage image = SKImage.FromBitmap(_saveBitmap))
            {
                SKData data = image.Encode(format, 100);

                File.WriteAllBytes(file, data.ToArray());
            }
        }
    }
}