using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
namespace Paint
{
    public class ResizeAdorner : Adorner
    {
        VisualCollection AdornerVisuals;
        Thumb thumb1, thumb2;

        public ResizeAdorner(UIElement adornedElement) : base(adornedElement)
        {
            AdornerVisuals = new VisualCollection(this);

            thumb1 = new Thumb()
            {
                Background = Brushes.Black,
                Width = 10,
                Height = 10,
            };
            thumb2 = new Thumb()
            {
                Background = Brushes.Black,
                Width = 10,
                Height = 10,
            };


            thumb1.DragDelta += Thumb1_DragDelta;
            thumb2.DragDelta += Thumb2_DragDelta;

            AdornerVisuals.Add(thumb1);
            AdornerVisuals.Add(thumb2);
        }

        private void Thumb2_DragDelta(object sender, DragDeltaEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Thumb1_DragDelta(object sender, DragDeltaEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override Visual GetVisualChild(int index)
        {
            return AdornerVisuals[index];
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            thumb1.Arrange(new Rect(0,0, 10, 10));
            thumb2.Arrange(new Rect(AdornedElement.DesiredSize.Width, AdornedElement.DesiredSize.Height, 10, 10));

            return base.ArrangeOverride(finalSize);
        }
    }
}
