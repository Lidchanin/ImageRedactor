using System;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using UIKit;

[assembly: ResolutionGroupName("ImageRedactorEffects")]
[assembly: ExportEffect(typeof(ImageRedactor.iOS.TouchEffect), "TouchEffect")]

namespace ImageRedactor.iOS
{
    public class TouchEffect : PlatformEffect
    {
        UIView _view;
        TouchRecognizer _touchRecognizer;

        protected override void OnAttached()
        {
            // Get the iOS UIView corresponding to the Element that the effect is attached to
            _view = Control == null ? Container : Control;

            // Get access to the TouchEffect class in the .NET Standard library
            ImageRedactor.TouchEffect effect = (ImageRedactor.TouchEffect)Element.Effects.FirstOrDefault(e => e is ImageRedactor.TouchEffect);

            if (effect != null && _view != null)
            {
                // Create a TouchRecognizer for this UIView
                _touchRecognizer = new TouchRecognizer(Element, _view, effect); 
                _view.AddGestureRecognizer(_touchRecognizer);
            }
        }

        protected override void OnDetached()
        {
            if (_touchRecognizer != null)
            {
                // Clean up the TouchRecognizer object
                _touchRecognizer.Detach();

                // Remove the TouchRecognizer from the UIView
                _view.RemoveGestureRecognizer(_touchRecognizer);
            }
        }
    }
}