using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;
using GraphicsLibrary;
using System.Windows.Media;
using System.Windows.Controls;

namespace Ellipse2DShape
{
    public class Ellipse2D : IShape
    {
        public override string Name => "Ellipse2D";

        public override UIElement Draw()
        {
            Ellipse ellipse = new Ellipse();

            // Width and Height of Ellipse
            double mWidth = Points[0].X - Points[1].X;
            double mHeight = Points[0].Y - Points[1].Y;

            // Set the properties for the ellipse
            ellipse.Width = Math.Abs(mWidth);
            ellipse.Height = Math.Abs(mHeight);
            ellipse.Stroke = new SolidColorBrush(Color);
            ellipse.StrokeThickness = StrokeThickness;
            ellipse.StrokeDashArray = DoubleCollection.Parse(DashStyle);
            if(isFill)
            {
                ellipse.Fill = new SolidColorBrush(Color);
            }
            // Position for the ellipse, get the minimum value of X and Y
            // Because the ellipse is drawn from the top left corner
            Canvas.SetLeft(ellipse, Math.Min(Points[0].X, Points[1].X));
            Canvas.SetTop(ellipse, Math.Min(Points[0].Y, Points[1].Y));
            Canvas.SetZIndex(ellipse, ZIndex);

            double x = (Math.Abs(Points[0].X - Points[1].X)) / 2;
            double y = (Math.Abs(Points[0].Y - Points[1].Y)) / 2;
            RotateTransform transform = new RotateTransform(this.rotationAngle, x, y);


            ellipse.RenderTransform = transform;

            return ellipse;
        }

        public override void HandleShiftMode()
        {
            throw new NotImplementedException();
        }

        public override IShape Clone()
        {
            IShape shape = new Ellipse2D();
            shape.StrokeThickness = StrokeThickness;
            shape.Color = Color;
            shape.DashStyle = DashStyle;

            var _point0 = new Point(Points[0].X + 10, Points[0].Y + 10);
            var _point1 = new Point(Points[1].X + 10, Points[1].Y + 10);

            shape.Points.Add(_point0);
            shape.Points.Add(_point1);

            return shape;
        }

        public override IShape CloneShape()
        {
            return new Ellipse2D();
        }

        public override List<UIElement> adornedShape()
        {
            throw new NotImplementedException();
        }
    }
}
