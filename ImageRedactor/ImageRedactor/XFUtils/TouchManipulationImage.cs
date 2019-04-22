using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Point = Xamarin.Forms.Point;
using Rectangle = Xamarin.Forms.Rectangle;

namespace ImageRedactor.XFUtils
{
    public class TouchManipulationImage : Image
    {
        private readonly Dictionary<long, TouchManipulationInfoXF> _touchDictionary =
            new Dictionary<long, TouchManipulationInfoXF>();

        private Point _offset;
        private float _angleOfRotationDeg;
        private float _angleOfRotationRad;

        public Rectangle ImageRectangle { get; set; }
        public BoxView ImageBoxView { get; set; }

        public bool HitTest(Point location)
        {
            //var centerX = TranslationX + WidthRequest * 0.5;
            //var centerY = TranslationY + HeightRequest * 0.5;

            //var x = centerX + (location.X - centerX) * Math.Cos(_angleOfRotationRad) -
            //        (location.Y - centerY) * Math.Sin(_angleOfRotationRad);
            //var y = centerY + (location.Y - centerY) * Math.Cos(_angleOfRotationRad) +
            //        (location.X - centerX) * Math.Sin(_angleOfRotationRad);

            //Debug.WriteLine($"----------- normal:\t{location.X} , {location.Y}");
            //Debug.WriteLine($"----------- rotated:\t{x} , {y}");
            //Debug.WriteLine(
            //    $"----------- rectangle parameters:\t{ImageRectangle.X} , {ImageRectangle.Y}\n{ImageRectangle.Width}; {ImageRectangle.Height}");

            //return ImageRectangle.Contains(new Point(x, y));
            return true;
        }

        public void ProcessTouchEvent(long id, TouchActionType type, Point location)
        {
            switch (type)
            {
                case TouchActionType.Pressed:
                    _touchDictionary.Add(id, new TouchManipulationInfoXF
                    {
                        PreviousPoint = location,
                        NewPoint = location
                    });
                    _offset = new Point(location.X - TranslationX, location.Y - TranslationY);
                    break;

                case TouchActionType.Moved:
                    TouchManipulationInfoXF info = _touchDictionary[id];
                    info.NewPoint = location;
                    Manipulate();
                    info.PreviousPoint = info.NewPoint;
                    break;

                case TouchActionType.Released:
                    _touchDictionary.Remove(id);
                    break;

                case TouchActionType.Cancelled:
                    _touchDictionary.Remove(id);
                    break;
            }
        }

        private void Manipulate()
        {
            var newTransX = 0f;
            var newTransY = 0f;

            var infos = new TouchManipulationInfoXF[_touchDictionary.Count];
            _touchDictionary.Values.CopyTo(infos, 0);

            if (infos.Length == 1)
            {
                TranslationX = infos[0].NewPoint.X - _offset.X;
                TranslationY = infos[0].NewPoint.Y - _offset.Y;

                newTransX = (float) TranslationX;
                newTransY = (float) TranslationY;
            }
            else if (infos.Length >= 2)
            {
                var pivotIndex = infos[0].NewPoint == infos[0].PreviousPoint ? 0 : 1;

                var pivotPoint = infos[pivotIndex].NewPoint;
                var newPoint = infos[1 - pivotIndex].NewPoint;
                var prevPoint = infos[1 - pivotIndex].PreviousPoint;

                var oldVector = new Point(prevPoint.X - pivotPoint.X, prevPoint.Y - pivotPoint.Y);
                var newVector = new Point(newPoint.X - pivotPoint.X, newPoint.Y - pivotPoint.Y);

                // Find angles from pivot point to touch points
                var oldAngle = (float) Math.Atan2(oldVector.Y, oldVector.X);
                var newAngle = (float) Math.Atan2(newVector.Y, newVector.X);

                // Calculate rotation matrix
                var angleInRad = newAngle - oldAngle;

                // Effectively rotate the old vector
                var magnitudeRatio = Magnitude(oldVector) / Magnitude(newVector);
                oldVector.X = magnitudeRatio * newVector.X;
                oldVector.Y = magnitudeRatio * newVector.Y;

                var angleInDeg = angleInRad * 180 / Math.PI;

                _angleOfRotationRad = (float) ((_angleOfRotationRad + angleInRad) % (2 * Math.PI));
                _angleOfRotationDeg = (float) ((_angleOfRotationDeg + angleInDeg) % 360);

                Rotation = (Rotation + angleInDeg) % 360;
                ImageBoxView.Rotation = Rotation;

                float scaleX, scaleY;

                scaleX = scaleY = Magnitude(newVector) / Magnitude(oldVector);

                ScaleX *= scaleX;
                ScaleY *= scaleY;

                var growPointX = (float) (TranslationX + WidthRequest * AnchorX);
                var growPointY = (float) (TranslationY + HeightRequest * AnchorY);

                var kx = AnchorX == 0 ? 1 : -1;
                var ky = AnchorY == 0 ? 1 : -1;

                var growWidthX = WidthRequest * ScaleX * AnchorX * kx;
                var growHeightY = HeightRequest * ScaleY * AnchorY * ky;

                newTransX = (float) (growPointX + growWidthX);
                newTransY = (float) (growPointY + growHeightY);
            }

            ImageRectangle = new Rectangle(
                newTransX,
                newTransY,
                WidthRequest * ScaleX,
                HeightRequest * ScaleY);

            ImageBoxView.TranslationX = ImageRectangle.X;
            ImageBoxView.TranslationY = ImageRectangle.Y;
            ImageBoxView.WidthRequest = ImageRectangle.Width;
            ImageBoxView.HeightRequest = ImageRectangle.Height;
        }

        private static float Magnitude(Point point) => (float) Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(point.Y, 2));
    }
}