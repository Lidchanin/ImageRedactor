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
        }

        void SkiaViewPaintSurfaceHangle(object sender, SKPaintSurfaceEventArgs e)
        {
        }
    }
}