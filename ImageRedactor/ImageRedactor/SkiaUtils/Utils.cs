using System;
using System.Reflection;
using SkiaSharp;

namespace ImageRedactor
{
    public class Utils
    {
        public static SKBitmap LoadBitmapResource(string resourceID)
        {
            var assembly = typeof(Utils).GetTypeInfo().Assembly;

            using (var stream = assembly.GetManifestResourceStream(resourceID))
            {
                return SKBitmap.Decode(stream);
            }
        }
    }
}