using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using GraphicsLibrary;

namespace Text2DShape
{
    public class Text2D : IShape
    {
        public override string Name => "Text2D";

        public override UIElement Draw()
        {
            TextBox text = new TextBox();

            double mWidth = Points[0].X - Points[1].X;
            double mHeight = Points[0].Y - Points[1].Y;

            text.Height = Math.Abs(mHeight);
            text.Width = Math.Abs(mWidth);

            text.Text = Text;
            text.BorderBrush = new SolidColorBrush(Color);
            text.BorderThickness = new Thickness(StrokeThickness);
            text.Background = Brushes.Transparent;
            text.Foreground = new SolidColorBrush(Color);
            text.TextWrapping = TextWrapping.Wrap;
            text.TextChanged += textChange;

            Canvas.SetLeft(text, Math.Min(Points[0].X, Points[1].X));
            Canvas.SetTop(text, Math.Min(Points[0].Y, Points[1].Y));

            return text;
        }

        private void textChange(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var text = textBox.Text;
            this.Text = text;
        }
        public override void HandleShiftMode()
        {
            throw new NotImplementedException();
        }
        public override IShape Clone()
        {
            throw new NotImplementedException();
        }

    }
}
