using SkiaSharp.Views.Forms;
using System.IO;
using Xamarin.Forms;

namespace ImageRedactor.Pages
{
    public partial class SSRealizationPage : ContentPage
	{
        SKCanvasView _skiaView;

		public SSRealizationPage(Stream photoStream)
		{
			InitializeComponent ();

            _skiaView = new SKCanvasView();
            _skiaView.PaintSurface += SkiaViewPaintSurfaceHangle;

            Content = _skiaView;

            //Effects.Add()
        }

        void SkiaViewPaintSurfaceHangle(object sender, SKPaintSurfaceEventArgs e)
        {
        }

        void Handle_TouchAction(object sender, TouchActionEventArgs args)
        {
            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    break;
                case TouchActionType.Moved:
                    break;
                case TouchActionType.Released:
                case TouchActionType.Cancelled:
                    break;

                default:
                    break;
            }
        }
    }
}