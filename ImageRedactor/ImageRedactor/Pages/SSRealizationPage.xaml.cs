using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace ImageRedactor.Pages
{
    public partial class SSRealizationPage : ContentPage
    {
        SKCanvasView _skiaView;
        TouchManipulationBitmap bitmap;
        List<long> touchIds = new List<long>();

        SKBitmap _photoBmp;

        public SSRealizationPage(Stream photoStream)
        {
            InitializeComponent();

            _skiaView = new SKCanvasView();
            _skiaView.PaintSurface += SkiaViewPaintSurfaceHangle;

            _photoBmp = SKBitmap.Decode(photoStream);

            bitmap = new TouchManipulationBitmap(Utils.LoadBitmapResource("ImageRedactor.Images.banana1.png"));
            bitmap.TouchManager.Mode = TouchManipulationMode.ScaleRotate;

            Content = _skiaView;
        }

        void SkiaViewPaintSurfaceHangle(object sender, SKPaintSurfaceEventArgs e)
        {
            var info = e.Info;
            var surface = e.Surface;
            var canvas = surface.Canvas;

            canvas.Clear();

            canvas.DrawBitmap(_photoBmp, info.Rect);

            bitmap.Paint(canvas);

            //canvas.DrawBitmap(_image, info.Rect);
        }

        void Handle_TouchAction(object sender, TouchActionEventArgs args)
        {
            Point pt = args.Location;
            SKPoint point =
                new SKPoint((float)(_skiaView.CanvasSize.Width * pt.X / _skiaView.Width),
                            (float)(_skiaView.CanvasSize.Height * pt.Y / _skiaView.Height));

            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    if (bitmap.HitTest(point))
                    {
                        touchIds.Add(args.Id);
                        bitmap.ProcessTouchEvent(args.Id, args.Type, point);
                        break;
                    }
                    break;

                case TouchActionType.Moved:
                    if (touchIds.Contains(args.Id))
                    {
                        bitmap.ProcessTouchEvent(args.Id, args.Type, point);
                        _skiaView.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Released:
                case TouchActionType.Cancelled:
                    if (touchIds.Contains(args.Id))
                    {
                        bitmap.ProcessTouchEvent(args.Id, args.Type, point);
                        touchIds.Remove(args.Id);
                        _skiaView.InvalidateSurface();
                    }
                    break;
            }
        }
    }
}