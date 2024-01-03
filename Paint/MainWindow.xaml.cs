using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GraphicsLibrary;
using Line2DShape;
using Rectangle2DShape;
using Ellipse2DShape;
using Text2DShape;
using System.Windows.Controls.Ribbon;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Fluent;
using TextBox = System.Windows.Controls.TextBox;
using Microsoft.Win32;
// TODO: Redo , Undo for Image
namespace Paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Fluent.RibbonWindow
    {
        /// <summary>
        /// 1 : Line
        /// 2 : Rectangle
        /// 3 : Ellipse
        /// 4 : Pen
        /// 5 : Text
        /// 0 : Select
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

        }

        Point _start;
        Point _end;
        bool isDrawing = false;
        int index = 1;
        List<IShape> _shape = new List<IShape>();
        List<IShape> _redoUndo = new List<IShape>();

        // List Image
        List<MyImage> _image = new List<MyImage>();

        // Mode when key down
        bool ctrlMode = false;
        bool shiftMode = false;
        // Attribute
        int strokeThickNess = 1;
        Color color = Colors.Black;
        string dashStyle = "1, 0";

        //Drag and Drop
        bool _isDragDropMode = false;
        UIElement _selectedShape = null;
        Image _selectedImage = null;
        Point offset, endOffSet;

        // Current UIElement
        UIElement _currentUIElement = null;
        IShape _clone = null;
        // Current Image
        Image _currentImage = null;

        MyImage _myImageClone = null;

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDrawing = true;
            
            // clear the _redoUndo list
            _redoUndo.Clear();

            // Place the point where the mouse was clicked for the start point
            _start = e.GetPosition(drawingCanvas);
        }
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                _end = e.GetPosition(drawingCanvas);

                // Clear the canvas before drawing a new shape

                reDraw();
                // Draw shapes that have already been drawn
                if (index == 1)
                {
                    IShape preview = new Line2D();
                    preview.StrokeThickness = strokeThickNess;
                    preview.Color = color;
                    preview.DashStyle = dashStyle;
                    preview.Points.Add(_start);
                    preview.Points.Add(_end);
                    drawingCanvas.Children.Add(preview.Draw());
                }
                if (index == 2)
                {
                    IShape preview = new Regtangle2D();
                    preview.StrokeThickness = strokeThickNess;
                    preview.Color = color;
                    preview.DashStyle = dashStyle;

                    preview.Points.Add(_start);
                    preview.Points.Add(_end);
                    drawingCanvas.Children.Add(preview.Draw());
                }
                if (index == 3)
                {
                    IShape preview = new Ellipse2D();
                    preview.StrokeThickness = strokeThickNess;
                    preview.Color = color;
                    preview.DashStyle = dashStyle;

                    preview.Points.Add(_start);
                    preview.Points.Add(_end);
                    drawingCanvas.Children.Add(preview.Draw());
                }
                if(index == 4)
                {
                    IShape preview = new Line2D();
                    preview.StrokeThickness = strokeThickNess;
                    preview.Color = color;
                    preview.DashStyle = dashStyle;

                    preview.Points.Add(_start);
                    preview.Points.Add(_end);
                    _start = _end;
                    drawingCanvas.Children.Add(preview.Draw());
                    _shape.Add(preview);
                    _redoUndo.Add(preview);
                }
                if(index == 5)
                {
                    IShape preview = new Text2D();
                    preview.StrokeThickness = strokeThickNess;
                    preview.Color = color;
                    preview.DashStyle = dashStyle;

                    preview.Points.Add(_start);
                    preview.Points.Add(_end);
                    drawingCanvas.Children.Add(preview.Draw());
                }
            }
        }
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;

            _end = e.GetPosition(drawingCanvas);

            if (index == 1)
            {
                IShape shape = new Line2D();
                shape.Points.Add(_start);
                shape.Points.Add(_end);
                shape.StrokeThickness = strokeThickNess;
                shape.Color = color;
                shape.DashStyle = dashStyle;
                _shape.Add(shape);
                _redoUndo.Add(shape);
            }
            if (index == 2)
            {
                IShape shape = new Regtangle2D();
                shape.Points.Add(_start);
                shape.Points.Add(_end);
                shape.StrokeThickness = strokeThickNess;
                shape.Color = color;
                shape.DashStyle = dashStyle;
                _shape.Add(shape);
                _redoUndo.Add(shape);
            }
            if (index == 3)
            {
                IShape shape = new Ellipse2D();
                shape.Points.Add(_start);
                shape.Points.Add(_end);
                shape.StrokeThickness = strokeThickNess;
                shape.Color = color;
                shape.DashStyle = dashStyle;
                _shape.Add(shape);
                _redoUndo.Add(shape);
            }
            if(index == 5)
            {
                IShape shape = new Text2D();
                shape.Points.Add(_start);
                shape.Points.Add(_end);
                shape.StrokeThickness = strokeThickNess;
                shape.Color = color;
                shape.DashStyle = dashStyle;
                _shape.Add(shape);
                _redoUndo.Add(shape);

            }
            reDraw();

        }
        private void Line_Click(object sender, RoutedEventArgs e)
        {
            index = 1;
        }
        private void Rectangle_Click(object sender, RoutedEventArgs e)
        {
            index = 2;
        }
        private void Ellipse_Click(object sender, RoutedEventArgs e)
        {
            index = 3;
        }
        private void Pen_Click(object sender, RoutedEventArgs e)
        {
            index = 4;
        }
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if(_shape.Count > 0)
            {
                _redoUndo.Add(_shape.Last());
                _shape.Remove(_shape.Last());
                reDraw();
            }
        }
        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if(_redoUndo.Count > 0)
            {
                _shape.Add(_redoUndo.Last());
                _redoUndo.Remove(_redoUndo.Last());
                reDraw();
            }
        }
        private void reDraw()
        {
            drawingCanvas.Children.Clear();
            foreach (MyImage image in _image)
            {
                Canvas.SetLeft(image.mImage, image.mPoint.X);
                Canvas.SetTop(image.mImage, image.mPoint.Y);
                image.mImage.PreviewMouseDown += HandlerPreviewMouseDownImage;
                drawingCanvas.Children.Add(image.mImage);
            }


            foreach (IShape shape in _shape)
            {
                UIElement uIElement = shape.Draw();
                shape.hashCode = uIElement.GetHashCode();
                if(uIElement is Line line)
                {
                    line.PreviewMouseDown += HandlerPreviewMouseDownLine;
                }
                else if(uIElement is TextBox TextBox)
                {
                    TextBox.PreviewMouseDown += HandlerPreviewMouseDownTextBox;
                }
                else
                {
                    uIElement.PreviewMouseDown += HandlerPreviewMouseDown;
                }
                drawingCanvas.Children.Add(uIElement);
            }

        }

        private void HandlerPreviewMouseDownImage(object sender, MouseButtonEventArgs e)
        {
            _selectedImage = sender as Image;
            _currentImage = sender as Image;

            double imageTop = Canvas.GetTop(_selectedImage);    // Get the distance from the top edge of the Canvas to the top edge of the image
            double imageLeft = Canvas.GetLeft(_selectedImage);  // Get the distance from the left edge of the Canvas to the left edge of the image


            offset = e.GetPosition(drawingCanvas);
            offset.Y -= Canvas.GetTop(_selectedImage);
            offset.X -= Canvas.GetLeft(_selectedImage);
            drawingCanvas.CaptureMouse();
        }

        private void HandlerPreviewMouseDownLine(object sender, MouseButtonEventArgs e)
        {
            _currentUIElement = sender as UIElement;
            _selectedShape = sender as UIElement;
            var _line = _selectedShape as Line;
            AdornerLayer.GetAdornerLayer(drawingCanvas).Add(new ResizeAdorner(_selectedShape));

            offset = e.GetPosition(drawingCanvas);
            endOffSet = e.GetPosition(drawingCanvas);

            offset.Y -= _line.Y1;
            offset.X -= _line.X1;
            endOffSet.Y -= _line.Y2;
            endOffSet.X -= _line.X2;
        
            drawingCanvas.CaptureMouse();
        }
        private void HandlerPreviewMouseDownTextBox(object sender, MouseButtonEventArgs e)
        {
            _selectedShape = sender as UIElement;
            _currentUIElement = sender as UIElement;
            if (ctrlMode)
            {

            }
            else
            {
                AdornerLayer.GetAdornerLayer(drawingCanvas).Add(new ResizeAdorner(_selectedShape));

                offset = e.GetPosition(drawingCanvas);
                offset.Y -= Canvas.GetTop(_selectedShape);
                offset.X -= Canvas.GetLeft(_selectedShape);
                drawingCanvas.CaptureMouse();
            }

        }
        private void HandlerPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _selectedShape = sender as UIElement;
            _currentUIElement = sender as UIElement;

            AdornerLayer.GetAdornerLayer(drawingCanvas).Add(new ResizeAdorner(_selectedShape));

            offset = e.GetPosition(drawingCanvas);
            offset.Y -= Canvas.GetTop(_selectedShape);
            offset.X -= Canvas.GetLeft(_selectedShape);
            drawingCanvas.CaptureMouse();
        }
        private void drawingCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(drawingCanvas);

            if (_selectedShape != null)
            {
                if (_selectedShape is Line)
                {
                    var _line = _selectedShape as Line;
                    _line.X1 = position.X - offset.X;
                    _line.Y1 = position.Y - offset.Y;
                    _line.X2 = position.X - endOffSet.X;
                    _line.Y2 = position.Y - endOffSet.Y;

                }
                else if (_selectedShape is TextBox)
                {
                    if (!ctrlMode)
                    {
                        Canvas.SetTop(_selectedShape, position.Y - offset.Y);
                        Canvas.SetLeft(_selectedShape, position.X - offset.X);
                    }
                }
                else
                {
                    Canvas.SetTop(_selectedShape, position.Y - offset.Y);
                    Canvas.SetLeft(_selectedShape, position.X - offset.X);
                }

            } else if (_selectedImage != null)
            {
                Canvas.SetTop(_selectedImage, position.Y - offset.Y);
                Canvas.SetLeft(_selectedImage, position.X - offset.X);

            }
               
        }
        private void drawingCanvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if(_selectedShape != null)
            {
                for (int i = 0; i < _shape.Count; i++)
                {
                    if (_shape[i].hashCode == _selectedShape.GetHashCode())
                    {
                        _shape[i].Points[0] = new Point(Canvas.GetLeft(_selectedShape), Canvas.GetTop(_selectedShape));
                        _shape[i].Points[1] = new Point(Canvas.GetLeft(_selectedShape) + _selectedShape.RenderSize.Width, Canvas.GetTop(_selectedShape) + _selectedShape.RenderSize.Height);
                        break;
                    }
                }
                _selectedShape = null;
            }
            if(_selectedImage != null)
            {
                for(int i = 0; i < _image.Count; i++)
                {
                    if (_image[i].mImage.GetHashCode() == _selectedImage.GetHashCode())
                    {
                        _image[i].mPoint = new Point(Canvas.GetLeft(_selectedImage), Canvas.GetTop(_selectedImage));
                        Canvas.SetTop(_image[i].mImage, Canvas.GetTop(_selectedImage));
                        Canvas.SetLeft(_image[i].mImage, Canvas.GetLeft(_selectedImage));
                        _selectedImage = null;
                        break;
                    }
                }
                
            }
            this.drawingCanvas.ReleaseMouseCapture();
        }
        private void RibbonWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.LeftCtrl)
            {
                ctrlMode = true;
            }   
            else if (e.Key == Key.LeftShift)
            {
                shiftMode = true;
            }
            else if (shiftMode && ctrlMode && e.Key == Key.Z)
            {
                if (_shape.Count < _redoUndo.Count)
                {
                    _shape.Add(_redoUndo[_shape.Count]);
                    reDraw();
                }
            }
            else if(ctrlMode && e.Key == Key.Z)
            {
                if (_shape.Count > 0)
                {
                    _shape.Remove(_shape.Last());
                    reDraw();
                }
            }
        }
        private void RibbonWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.LeftCtrl)
            {
                ctrlMode = false;
            }
            if(e.Key == Key.LeftShift)
            {
                shiftMode = false;
            }
        }
        private void onePx_Click(object sender, RoutedEventArgs e)
        {
            strokeThickNess = 1;
        }
        private void twoPx_Click(object sender, RoutedEventArgs e)
        {
            strokeThickNess = 2;
        }
        private void fivePx_Click(object sender, RoutedEventArgs e)
        {
            strokeThickNess = 5;
        }
        private void tenPx_Click(object sender, RoutedEventArgs e)
        {
            strokeThickNess = 10;
        }
        private void ColorGallery_SelectedColorChanged(object sender, RoutedEventArgs e)
        {
            color = (Color)colorGallery.SelectedColor;
        }
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if(sender is ToggleButton toggleButton){
                switch (toggleButton.Name)
                {
                    case "dotline0":
                        lineStyleIcon.Source = new BitmapImage(new Uri("/Images/dotline0.png", UriKind.Relative));
                        dotline1.IsChecked = false;
                        dotline2.IsChecked = false;
                        dashStyle = "1, 0";
                        break;
                    case "dotline1":
                        lineStyleIcon.Source = new BitmapImage(new Uri("/Images/dotline1.png", UriKind.Relative));
                        dotline0.IsChecked = false;
                        dotline2.IsChecked = false;
                        dashStyle = "1 6";
                        break;
                    case "dotline2":
                        lineStyleIcon.Source = new BitmapImage(new Uri("/Images/dotline2.png", UriKind.Relative));
                        dotline1.IsChecked = false;
                        dotline0.IsChecked = false;
                        dashStyle = "6 1";
                        break;
                }
            }
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if(_currentUIElement != null)
            {
                for(int i = 0; i < _shape.Count; i++)
                {
                    if (_shape[i].hashCode == _currentUIElement.GetHashCode())
                    {
                        _shape.Remove(_shape[i]);
                        reDraw();
                        break;
                    }
                }
            }
            if(_currentImage != null)
            {
                for(int i = 0; i < _image.Count; i++)
                {
                    if (_image[i].mImage.GetHashCode() == _currentImage.GetHashCode())
                    {
                        _image.Remove(_image[i]);
                        reDraw();
                        break;
                    }
                }
            }
        }
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUIElement != null)
            {
                for (int i = 0; i < _shape.Count; i++)
                {
                    if (_shape[i].hashCode == _currentUIElement.GetHashCode())
                    {
                        _clone = _shape[i].Clone();
                    }
                }
            }
            if(_currentImage != null)
            {
                for(int i = 0; i < _image.Count; i++)
                {
                    if (_image[i].mImage.GetHashCode() == _currentImage.GetHashCode())
                    {
                        _myImageClone = _image[i].Clone();
                        break;
                    }
                }
            }
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            if(_clone != null)
            {
                _shape.Add(_clone.Clone());
                reDraw();
            }
            if(_myImageClone != null)
            {
                _image.Add(_myImageClone.Clone());
                reDraw();
            }
        }

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUIElement != null)
            {
                for (int i = 0; i < _shape.Count; i++)
                {
                    if (_shape[i].hashCode == _currentUIElement.GetHashCode())
                    {
                        _clone = _shape[i].Clone();
                        _shape.Remove(_shape[i]);
                        reDraw();
                        break;
                    }
                }
            }
            if (_currentImage != null)
            {
                for (int i = 0; i < _image.Count; i++)
                {
                    if (_image[i].mImage.GetHashCode() == _currentImage.GetHashCode())
                    {
                        _myImageClone = _image[i].Clone();
                        _image.Remove(_image[i]);
                        reDraw();
                        break;
                    }
                }
            }
        }

        private void Text_Click(object sender, RoutedEventArgs e)
        {
            index = 5;
        }

        private void Insert_Image_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files|*.bmp;*.jpg;*.png";
            openFileDialog.RestoreDirectory = true;
            if(openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(fileName);
                bitmapImage.EndInit();

                MyImage myImage = new MyImage();
                Image image = new Image();
                image.Source = bitmapImage;
                image.Width = bitmapImage.Width / 2;
                image.Height = bitmapImage.Height / 2;

                if(bitmapImage.Width > drawingCanvas.Width || double.IsNaN(drawingCanvas.Width))
                {
                    drawingCanvas.Width = bitmapImage.Width > drawingCanvas.ActualWidth ? bitmapImage.Width : drawingCanvas.ActualWidth;
                }
                if(bitmapImage.Height > drawingCanvas.Height || double.IsNaN(drawingCanvas.Height))
                {
                    drawingCanvas.Height = bitmapImage.Height > drawingCanvas.ActualHeight ? bitmapImage.Height : drawingCanvas.ActualHeight;
                }
                myImage.mImage = image;
                drawingCanvas.Children.Add(myImage.mImage);
                _image.Add(myImage);
                // Change to select mode
                _isDragDropMode = true;
                btnSelect.Foreground = Brushes.Red;
                Canvas.SetZIndex(drawingCanvas, 1);
                Canvas.SetZIndex(drawingBorder, 0);
                reDraw();
            }
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {

            if (_isDragDropMode == false)
            {
                _isDragDropMode = true;
                btnSelect.Foreground = Brushes.Red;
                Canvas.SetZIndex(drawingCanvas, 1);
                Canvas.SetZIndex(drawingBorder, 0);

            }
            else if (_isDragDropMode == true)
            {
                _isDragDropMode = false;
                btnSelect.Foreground = Brushes.Black;
                Canvas.SetZIndex(drawingCanvas, 0);
                Canvas.SetZIndex(drawingBorder, 1);
            }

        }

    }
    class MyImage
    {
        public Image mImage { get; set; }
        public Point mPoint { get; set; } = new Point(0, 0);
        public MyImage Clone()
        {
            MyImage myImage = new MyImage();
            if (mImage != null)
            {
                myImage.mImage = new Image();

                // Check if Source is not null before copying it
                if (mImage.Source != null)
                {
                    myImage.mImage.Source = mImage.Source;
                    myImage.mImage.Width = mImage.Width;
                    myImage.mImage.Height = mImage.Height;

                }

                // Copy other properties as needed
            }
            var newPoint = new Point(mPoint.X + 10, mPoint.Y + 10);
            myImage.mPoint = newPoint;
            return myImage;
        }
    }
}
