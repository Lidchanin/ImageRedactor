using System;
using System.Collections.Generic;
using System.Diagnostics;
using SkiaSharp;
using Xamarin.Forms;

namespace ImageRedactor.XFUtils
{
    public class TouchManipulationImage : Image
    {
        //SKBitmap bitmap;
        public TouchManipulationMode Mode { set; get; }

        Dictionary<long, TouchManipulationInfoXF> touchDictionary =
            new Dictionary<long, TouchManipulationInfoXF>();

        

        private Point _offset;

        public TouchManipulationImage()
        {
            //this.bitmap = bitmap;
            //Matrix = SKMatrix.MakeIdentity();

            //TouchManager = new TouchManipulationManager
            //{
            //    Mode = TouchManipulationMode.ScaleRotate
            //};
        }

        //public TouchManipulationManager TouchManager { set; get; }

        //public void Paint(SKCanvas canvas)
        //{
        //    //canvas.Save();
        //    //SKMatrix matrix = Matrix;
        //    //canvas.Concat(ref matrix);
        //    //canvas.DrawBitmap(bitmap, 0, 0);
        //    //canvas.Restore();
        //}

        public bool HitTest(Point location)
        {
            return new Rectangle(TranslationX, TranslationY, Width, Height).Contains(location.X, location.Y);

            // Invert the matrix
            //SKMatrix inverseMatrix;

            //if (Matrix.TryInvert(out inverseMatrix))
            //{
            //    // Transform the point using the inverted matrix
            //    SKPoint transformedPoint = inverseMatrix.MapPoint(location);

            //    // Check if it's in the untransformed bitmap rectangle
            //    SKRect rect = new SKRect(0, 0, bitmap.Width, bitmap.Height);
            //    return rect.Contains(transformedPoint);
            //}
            //return false;
        }

        public void ProcessTouchEvent(long id, TouchActionType type, Point location)
        {
            switch (type)
            {
                case TouchActionType.Pressed:
                    touchDictionary.Add(id, new TouchManipulationInfoXF
                    {
                        PreviousPoint = location,
                        NewPoint = location
                    });
                    _offset = new Point(location.X - TranslationX, location.Y - TranslationY);

                    break;

                case TouchActionType.Moved:
                    TouchManipulationInfoXF info = touchDictionary[id];
                    info.NewPoint = location;
                    //Debug.WriteLine($"===================== id: {id}");
                    //Debug.WriteLine($"===================== pX: {info.PreviousPoint.X} \tpY: {info.PreviousPoint.Y}");
                    //Debug.WriteLine($"===================== nX: {info.NewPoint.X} \tnY: {info.NewPoint.Y}");
                    Manipulate();
                    info.PreviousPoint = info.NewPoint;
                    break;

                case TouchActionType.Released:
                    touchDictionary[id].NewPoint = location;
                    Manipulate();

                    Debug.WriteLine($"=================Width&Height {Width} + {Height}");
                    Debug.WriteLine($"=================Translations {TranslationX} + {TranslationY}");
                    Debug.WriteLine("\n");

                    touchDictionary.Remove(id);
                    break;

                case TouchActionType.Cancelled:
                    touchDictionary.Remove(id);
                    break;
            }
        }

        void Manipulate()
        {
            var infos = new TouchManipulationInfoXF[touchDictionary.Count];
            touchDictionary.Values.CopyTo(infos, 0);

            if (infos.Length == 1)
            {
                TranslationX = infos[0].NewPoint.X - _offset.X;
                TranslationY = infos[0].NewPoint.Y - _offset.Y;

                //SKPoint prevPoint = infos[0].PreviousPoint;
                //SKPoint newPoint = infos[0].NewPoint;
                //SKPoint pivotPoint = Matrix.MapPoint(bitmap.Width / 2, bitmap.Height / 2);

                //touchMatrix = TouchManager.OneFingerManipulate(prevPoint, newPoint, pivotPoint);
            }
            else if (infos.Length >= 2)
            {
                int pivotIndex = infos[0].NewPoint == infos[0].PreviousPoint ? 0 : 1;
                Point pivotPoint = infos[pivotIndex].NewPoint;
                Point newPoint = infos[1 - pivotIndex].NewPoint;
                Point prevPoint = infos[1 - pivotIndex].PreviousPoint;

                //Debug.WriteLine($"p_index: {pivotIndex}");
                //Debug.WriteLine($"pivot: {pivotPoint}");
                //Debug.WriteLine($"prev: {newPoint}");
                //Debug.WriteLine($"new: {prevPoint}");
                //Debug.WriteLine("\n");

                var oldVector2 = new SKPoint((float) prevPoint.X, (float) prevPoint.Y) -
                                 new SKPoint((float) pivotPoint.X, (float) pivotPoint.Y);
                var newVector2 = new SKPoint((float) newPoint.X, (float) newPoint.Y) -
                                 new SKPoint((float) pivotPoint.X, (float) pivotPoint.Y);

                Point oldVector = new Point(prevPoint.X - pivotPoint.X, prevPoint.Y - pivotPoint.Y);
                Point newVector = new Point(newPoint.X - pivotPoint.X, newPoint.Y - pivotPoint.Y);

                //Debug.WriteLine($"old: {oldVector}");
                //Debug.WriteLine($"new: {newVector}");
                //Debug.WriteLine($"sk old: {oldVector2}");
                //Debug.WriteLine($"sk new: {newVector2}");
                //Debug.WriteLine("\n");

                // Find angles from pivot point to touch points
                float oldAngle = (float) Math.Atan2(oldVector.Y, oldVector.X);
                float newAngle = (float) Math.Atan2(newVector.Y, newVector.X);

                // Calculate rotation matrix
                float angle = newAngle - oldAngle;

                // Effectively rotate the old vector
                float magnitudeRatio = Magnitude(oldVector) / Magnitude(newVector);
                oldVector.X = magnitudeRatio * newVector.X;
                oldVector.Y = magnitudeRatio * newVector.Y;

                float scaleX = 1;
                float scaleY = 1;

                scaleX = (float)(newVector.X / oldVector.X);
                scaleY = (float)(newVector.Y / oldVector.Y);

                //scaleX = scaleY = Magnitude(newVector) / Magnitude(oldVector);

                if (!float.IsNaN(scaleX) && !float.IsInfinity(scaleX) &&
                    !float.IsNaN(scaleY) && !float.IsInfinity(scaleY))
                {
                    //ScaleX = scaleX;
                    //ScaleY = scaleY;
                    ////SKMatrix.PostConcat(ref touchMatrix, SKMatrix.MakeScale(scaleX, scaleY, pivotPoint.X, pivotPoint.Y));
                }

                //scaleX = newVector.X / oldVector.X;
                //scaleY = newVector.Y / oldVector.Y;

                //Debug.WriteLine($"======= X: {scaleX} Y: {scaleY}");

                var scaleOffsetX = scaleX - 1;
                var scaleOffsetY = scaleY - 1;

                ScaleX += scaleOffsetX;
                ScaleY += scaleOffsetY;

                //WidthRequest += scaleX;
                //HeightRequest += scaleY;
            }
        }

        float Magnitude(Point point) => (float) Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(point.Y, 2));

    }
}