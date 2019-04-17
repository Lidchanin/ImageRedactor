using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ImageRedactor.Pages
{
	public partial class SSRealizationPage : ContentPage
	{
        SKCanvasView _skiaView;

		public SSRealizationPage ()
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