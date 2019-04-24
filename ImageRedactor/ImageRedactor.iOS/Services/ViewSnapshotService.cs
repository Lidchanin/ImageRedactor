using Foundation;
using ImageRedactor.iOS.Services;
using ImageRedactor.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using ImageRedactor.Helpers;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(ViewSnapshotService))]
namespace ImageRedactor.iOS.Services
{
    public class ViewSnapshotService : IViewSnapshotService
    {
        public void MakeViewSnapshotAndSave(View view, string snapshotName)
        {
            try
            {
                var nativeView = Xamarin.Forms.Platform.iOS.Platform.GetRenderer(view).NativeView;
                UIGraphics.BeginImageContextWithOptions(nativeView.Bounds.Size, true, 0);
                nativeView.DrawViewHierarchy(nativeView.Bounds, afterScreenUpdates: false);
                var image = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();

                var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var directoryName = Path.Combine(documentsDirectory, ConstantHelper.PicturesFolder);
                Directory.CreateDirectory(directoryName);
                var pngFilePath = Path.Combine(directoryName, snapshotName);
                var imageData = image.AsPNG();
                NSError error = null;
                if (imageData.Save(pngFilePath, false, out error))
                {
                    Console.WriteLine("saved as " + pngFilePath);
                }
                else
                {
                    Console.WriteLine("not saved as " + pngFilePath + ", because" + error.LocalizedDescription);
                }
                var alert = new UIKit.UIAlertView("Saved Location", pngFilePath, null, "OK");
                alert.Show();
            }
            catch (Exception)
            {
            }
        }
    }
}