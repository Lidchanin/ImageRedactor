using System.IO;
using Xamarin.Forms;

namespace ImageRedactor.ViewModels
{
    public class XFRealizationViewModel
    {
        public ImageSource PhotoSource => ImageSource.FromStream(() => _photoStream);

        private readonly Stream _photoStream;

        public XFRealizationViewModel(Stream photoStream)
        {
            _photoStream = photoStream;
        }
    }
}