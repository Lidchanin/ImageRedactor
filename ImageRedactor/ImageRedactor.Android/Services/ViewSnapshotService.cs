using Android.Graphics;
using ImageRedactor.Droid.Services;
using ImageRedactor.Services;
using Java.IO;
using System;
using System.IO;
using ImageRedactor.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Environment = Android.OS.Environment;
using Path = System.IO.Path;

[assembly: Dependency(typeof(ViewSnapshotService))]
namespace ImageRedactor.Droid.Services
{
    public class ViewSnapshotService : IViewSnapshotService
    {
        public async void MakeViewSnapshotAndSave(View view, string snapshotName)
        {
            try
            {
                var nativeView = view.GetRenderer().View;
                nativeView.DrawingCacheEnabled = false;
                nativeView.BuildDrawingCache(false);
                var bitmap = nativeView.GetDrawingCache(false);

                byte[] bytes;
                using (var memoryStream = new MemoryStream())
                {
                    await bitmap.CompressAsync(Bitmap.CompressFormat.Png, 100, memoryStream);
                    bytes = memoryStream.ToArray();
                }

                var filename =
                    Path.Combine(
                        Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures).ToString(),
                        ConstantHelper.PicturesFolder);
                Directory.CreateDirectory(filename);
                var fullFilename = Path.Combine(filename, snapshotName);

                for (;;)
                {
                    if (System.IO.File.Exists(fullFilename))
                    {
                        snapshotName = snapshotName.Insert(0, "(1)");
                        fullFilename = Path.Combine(filename, snapshotName);
                    }
                    else
                    {
                        break;
                    }
                }

                using (var fileOutputStream = new FileOutputStream(fullFilename))
                {
                    await fileOutputStream.WriteAsync(bytes);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}