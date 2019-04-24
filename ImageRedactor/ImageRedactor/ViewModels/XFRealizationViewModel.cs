using System.IO;
using ImageRedactor.Helpers;
using Xamarin.Forms;

namespace ImageRedactor.ViewModels
{
    public class XFRealizationViewModel
    {
        public ImageSource PhotoSource;

        private readonly Stream _photoStream;

        public XFRealizationViewModel(Stream photoStream)
        {
            _photoStream = photoStream;
            if (_photoStream == null)
            {
                PhotoSource = ImageSource.FromResource(ConstantHelper.Monkey1Image);
            }
            else
            {
                PhotoSource = ImageSource.FromStream(() => _photoStream);
            }
        }
    }
}