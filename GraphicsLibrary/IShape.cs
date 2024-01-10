using System;
using System.Windows;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Media;
using System.Security.Policy;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Forms;

namespace GraphicsLibrary
{
    public abstract class IShape
    {
        public abstract string Name { get; }
        public List<Point> Points { get; set; } = new List<Point>();
        public double StrokeThickness { get; set; } = 1;
        public Color Color { get; set; } = Colors.Black;
        public string DashStyle { get; set; } = "1, 0";
        public int hashCode { get; set; }
        public string Text { get; set; } = "Your Text!";
        public int ZIndex { get; set; } = 0;
        public bool isFill { get; set; } = false;
        public int rotationAngle { get; set; } = 0;
        public abstract void HandleShiftMode();
        public abstract UIElement Draw();
        public abstract IShape Clone();
        public abstract IShape CloneShape();
        public abstract List<UIElement> adornedShape();
        virtual public UIElement controlOutline()
        {
            var left = Math.Min(Points[0].X, Points[1].X);
            var top = Math.Min(Points[0].Y, Points[1].Y);

            var right = Math.Max(Points[0].X, Points[1].X);
            var bottom = Math.Max(Points[0].Y, Points[1].Y);

            var width = right - left;
            var height = bottom - top;

            var rect = new Rectangle()
            {
                Width = width,
                Height = height,
                StrokeThickness = 2,
                Stroke = Brushes.Red,
                StrokeDashArray = { 4, 2, 4 }
            };

            Canvas.SetLeft(rect, left);
            Canvas.SetTop(rect, top);

            double x = (Math.Abs(Points[0].X - Points[1].X)) / 2;
            double y = (Math.Abs(Points[0].Y - Points[1].Y)) / 2;
            RotateTransform transform = new RotateTransform(this.rotationAngle, x, y);

            rect.RenderTransform = transform;
            return rect;
        }
        virtual public List<ResizePoint> GetControlPoints()
        {
            List<ResizePoint> resizePoints = new List<ResizePoint>();

            ResizePoint resizePointTopLeft = new ResizePoint();
            resizePointTopLeft.setPoint(Points[0].X, Points[0].Y);
            resizePointTopLeft.setTypeName("TopLeft");

            ResizePoint resizePointBottomLeft = new ResizePoint();
            resizePointBottomLeft.setPoint(Points[0].X, Points[1].Y);
            resizePointBottomLeft.setTypeName("BottomLeft");

            ResizePoint resizePointTopRight = new ResizePoint();
            resizePointTopRight.setPoint(Points[1].X, Points[0].Y);
            resizePointTopRight.setTypeName("TopRight");

            ResizePoint resizePointBottomRight = new ResizePoint();
            resizePointBottomRight.setPoint(Points[1].X, Points[1].Y);
            resizePointBottomRight.setTypeName("BottomRight");

            resizePoints.Add(resizePointTopLeft);
            resizePoints.Add(resizePointBottomLeft);
            resizePoints.Add(resizePointTopRight);
            resizePoints.Add(resizePointBottomRight);

            return resizePoints;
        }
    }
}
