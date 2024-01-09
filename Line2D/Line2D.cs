using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using GraphicsLibrary;
using System;
using System.Windows.Controls;

namespace Line2DShape
{
    public class Line2D : IShape
    {

        public override string Name => "Line2D";

        public override UIElement Draw()
        {
            Line l = new Line();
            l.X1 = Points[0].X;
            l.Y1 = Points[0].Y;
            l.X2 = Points[1].X;
            l.Y2 = Points[1].Y;
            l.Stroke = new SolidColorBrush(Color);
            l.StrokeThickness = StrokeThickness;
            l.StrokeDashArray = DoubleCollection.Parse(DashStyle);
            l.Fill = new SolidColorBrush(Color);

            Canvas.SetZIndex(l, ZIndex);
            // TODO: add DashStyle here
            return l;
        }

        public override void HandleShiftMode()
        {
            
        }

        public override IShape Clone()
        {
            IShape shape = new Line2D();
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
            return new Line2D();
        }
    }
}
