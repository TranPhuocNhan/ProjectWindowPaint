using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;
using GraphicsLibrary;
using System.Windows.Media;
using System.Windows.Controls;

namespace GraphicsLibrary
{
    public class ResizePoint
    {
        Point _point;
        private int _size = 12;

        public string typeName = "";

        public ResizePoint()
        {
            _point = new Point();
        }
        public void setPoint(double x, double y)
        {
            _point.X = x;
            _point.Y = y;
        }
        public void setTypeName(string name)
        {
            typeName = name;
        }
        public Point getPoint()
        {
            return _point;
        }
        public UIElement drawPoint(double angle, Point centrePoint)
        {
            UIElement element = new Ellipse()
            {
                Width = _size,
                Height = _size,
                Fill = Brushes.Red,
                Stroke = Brushes.Black,
                StrokeThickness = _size / 5,
            };

            

            Point pos = new Point(_point.X, _point.Y);
            Point centre = new Point(centrePoint.X, centrePoint.Y);

            Point afterTransform = ConvertPointTransform.Rotate(pos, angle, centre);

            Canvas.SetLeft(element, afterTransform.X - _size / 2);
            Canvas.SetTop(element, afterTransform.Y - _size / 2);

            return element;
        }
    }
}
