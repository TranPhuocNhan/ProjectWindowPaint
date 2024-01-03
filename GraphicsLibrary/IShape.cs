using System;
using System.Windows;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Media;
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
        public string Text { get; set; } = "Hello World!";
        public abstract void HandleShiftMode();
        public abstract UIElement Draw();
        public abstract IShape Clone();
    }
}
