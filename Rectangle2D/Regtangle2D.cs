using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using GraphicsLibrary;
using System.Windows.Controls;

namespace Rectangle2DShape
{
    public class Regtangle2D : IShape
    {
        public override string Name => "Regtangle2D";

        public override UIElement Draw()
        {
            Rectangle rectangle = new Rectangle();
            
            // Width and Height of Rectangle
            double mWidth = Points[0].X - Points[1].X;
            double mHeight =Points[0].Y - Points[1].Y;

            // Set the properties for the rectangle
            rectangle.Width = Math.Abs(mWidth);
            rectangle.Height = Math.Abs(mHeight);
            rectangle.Stroke = new SolidColorBrush(Color);
            rectangle.StrokeThickness = StrokeThickness;
            rectangle.StrokeDashArray = DoubleCollection.Parse(DashStyle);
            
            if(isFill)
            {
                rectangle.Fill = new SolidColorBrush(Color);
            }
            // Position for the rectangle, get the minimum value of X and Y
            // Because the rectangle is drawn from the top left corner
            Canvas.SetLeft(rectangle, Math.Min(Points[0].X, Points[1].X));
            Canvas.SetTop(rectangle, Math.Min(Points[0].Y, Points[1].Y));
            Canvas.SetZIndex(rectangle, ZIndex);


            return rectangle;
        }

        public override void HandleShiftMode()
        {
            
        }

        public override IShape Clone()
        {
            IShape shape = new Regtangle2D();
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
             return new Regtangle2D();
        }
    }
}
