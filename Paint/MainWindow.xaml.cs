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
using System.Windows.Controls.Ribbon;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Fluent;
using TextBox = System.Windows.Controls.TextBox;
using Microsoft.Win32;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media.Media3D;
using System.Reflection;
// TODO: Redo , Undo for Image
namespace Paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class MyImage
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
    public class DrawingData
    {
        public List<IShape> _shape { get; set; }
        public List<MyImage> _image { get; set; }
    }
    public class Layer : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        private int _zIndex;
        public int ZIndex
        {
            get { return _zIndex; }
            set
            {
                _zIndex = value;
                OnPropertyChanged("ZIndex");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public partial class MainWindow : Fluent.RibbonWindow
    {
        /// <summary>
        /// 1 : Line
        /// 2 : Rectangle
        /// 3 : Ellipse
        /// 4 : Pen
        /// 5 : Text
        /// 6 : Fill
        /// 0 : Select
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

        }

        Point _start;
        Point _end;
        bool isDrawing = false;
        bool _isResize = false;

        string _choice = "";
        int index = 1;
        List<IShape> _shape = new List<IShape>();
        List<IShape> _redoUndo = new List<IShape>();
        ShapeFactory _factory;

        UIElement _controlOutline = null;
        List<ResizePoint> _resizePoints = new List<ResizePoint>();

        // List Image
        List<MyImage> _image = new List<MyImage>();

        // Mode when key down
        bool ctrlMode = false;
        bool shiftMode = false;
        bool _fillMode = false;
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
        IShape _currentIShape = null;

        MyImage _myImageClone = null;

        // Zoom variable
        int _startLoad = 2;

        // ZIndex
        int _totalIndex = 0;
        int _currentZIndex = 0;

        // List layer
        ObservableCollection<Layer> _layers = new ObservableCollection<Layer>();

        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;

            var abilities = new List<IShape>();

            // Do tim cac kha nang
            string folder = AppDomain.CurrentDomain.BaseDirectory;
            var fis = (new DirectoryInfo(folder)).GetFiles("*.dll");

            foreach (var fi in fis)
            {
                var assembly = Assembly.LoadFrom(fi.FullName);
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if (type.IsClass & (!type.IsAbstract))
                    {
                        if (typeof(IShape).IsAssignableFrom(type))
                        {
                            var shape = Activator.CreateInstance(type) as IShape;
                            abilities.Add(shape!);
                        }
                    }
                }
            }
            _factory = new ShapeFactory();
            foreach (var ability in abilities)
            {
                _factory.Prototypes.Add(
                    ability.Name, ability
                );
                var button = new Fluent.Button()
                {
                    Width = 80,
                    Height = 35,
                    Content = ability.Name,
                    Tag = ability.Name,
                    LargeIcon = new Image()
                    {
                        Source = new BitmapImage(new Uri("/Images/" + ability.Name + ".png", UriKind.Relative)),
                    }
                };
                RenderOptions.SetBitmapScalingMode((DependencyObject)button.LargeIcon, BitmapScalingMode.HighQuality);
                button.Click += (sender, args) =>
                {
                    var control = (Fluent.Button)sender;
                    _choice = (string)control.Tag;
                    _fillMode = false;
                };
                insertShape.Items.Add(button);
            };

            if (abilities.Count > 0)
            {
                _choice = abilities[0].Name;
            }

            _layers.Add(new Layer() { Name = "Layer", ZIndex = _totalIndex++ });
            myListLayer.SelectedItem = _layers.First();
            myListLayer.ItemsSource = _layers;

        }
        private Color getColorAtPoint(Canvas canvas, Point point)
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);

            renderTargetBitmap.Render(canvas);

            // Get the color of a pixel within the RenderTargetBitmap
            byte[] pixels = new byte[4];
            int stride = 4 * (int)canvas.ActualWidth;
            
            renderTargetBitmap.CopyPixels(new Int32Rect((int)point.X, (int)point.Y, 1, 1), pixels, stride, 0);
            Color color = Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
            return color;
        }
        private double getLeftPoint(Canvas canvas, Point point, Color old_color, Color new_color)
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);

            renderTargetBitmap.Render(canvas);

            // Get the color of a pixel within the RenderTargetBitmap
            byte[] pixels = new byte[4];
            int stride = 4 * (int)canvas.ActualWidth;
            double xLeft = point.X;
            // Get left point
            while (xLeft > 0)
            {
                xLeft -= 1;
                renderTargetBitmap.CopyPixels(new Int32Rect((int)xLeft, (int)point.Y, 1, 1), pixels, stride, 0);
                Color color = Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
                if (color != old_color) break;
            }
            return xLeft;
        }
        private double getRightPoint(Canvas canvas, Point point, Color old_color, Color new_color)
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);

            renderTargetBitmap.Render(canvas);

            // Get the color of a pixel within the RenderTargetBitmap
            byte[] pixels = new byte[4];
            int stride = 4 * (int)canvas.ActualWidth;
            double xRight = point.X;
            // Get right point
            while (xRight < canvas.ActualWidth - 1)
            {
                xRight += 1;
                renderTargetBitmap.CopyPixels(new Int32Rect((int)xRight, (int)point.Y, 1, 1), pixels, stride, 0);
                Color color = Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
                if (color != old_color) break;
            }
            return xRight;
        }
        private void fillColorTopToBottomPoint(Canvas canvas, Point point, Color old_color, Color new_color)
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);

            renderTargetBitmap.Render(canvas);

            // Get the color of a pixel within the RenderTargetBitmap
            byte[] pixels = new byte[4];
            int stride = 4 * (int)canvas.ActualWidth;
            double yTop = point.Y;
            double yBottom = point.Y;
            Point top = new Point(0, 0);
            Point bottom = new Point(0, 0);
            // Get top point
            while (yTop > 0)
            {
                yTop -= 1;
                renderTargetBitmap.CopyPixels(new Int32Rect((int)point.X, (int)yTop, 1, 1), pixels, stride, 0);
                Color color = Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
                if (color != old_color) break;
            }
            top.X = point.X;
            top.Y = yTop;
            // Get bottom point
            while (yBottom <= canvas.ActualHeight)
            {
                yBottom += 1;
                renderTargetBitmap.CopyPixels(new Int32Rect((int)point.X, (int)yBottom, 1, 1), pixels, stride, 0);
                Color color = Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
                if (color != old_color) break;
            }
            bottom.X = point.X + 1;
            bottom.Y = yBottom;

            IShape fillColor = _factory.Create("Regtangle2D");
            fillColor.StrokeThickness = strokeThickNess;
            fillColor.Color = new_color;
            fillColor.isFill = true;
            fillColor.Points.Add(top);
            fillColor.Points.Add(bottom);
            canvas.Children.Add(fillColor.Draw());
            _shape.Add(fillColor);
        }
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDrawing = true;
            
            // clear the _redoUndo list
            _redoUndo.Clear();

            // Place the point where the mouse was clicked for the start point
            _start = e.GetPosition(drawingCanvas);

            if(_fillMode)
            {
                Color old_color = getColorAtPoint(drawingCanvas, _start);
                Color new_color = color;
                double left = 0;
                double right = 0;
                left = getLeftPoint(drawingCanvas, _start, old_color, new_color);
                right = getRightPoint(drawingCanvas, _start, old_color, new_color);
             
                for(double i = left; i <= right; i+=1)
                {
                    Point point = new Point((int)i, _start.Y);
                    fillColorTopToBottomPoint(drawingCanvas, point, old_color, new_color);
                }
            }
        }
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            positionXY.Text = e.GetPosition(drawingCanvas).ToString();
            if (isDrawing && !_fillMode)
            {
                _end = e.GetPosition(drawingCanvas);

                // Clear the canvas before drawing a new shape

                reDraw();
                // Draw shapes that have already been drawn

                IShape preview = _factory.Create(_choice);
                preview.StrokeThickness = strokeThickNess;
                preview.Color = color;
                preview.DashStyle = dashStyle;
                preview.Points.Add(_start);
                preview.Points.Add(_end);
                preview.ZIndex = _currentZIndex;

                if(_choice == "Pen2D")
                {
                    _start = _end;
                    _shape.Add(preview);
                    _redoUndo.Add(preview);
                }

                drawingCanvas.Children.Add(preview.Draw());
            }
        }
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;

            _end = e.GetPosition(drawingCanvas);

            IShape shape = _factory.Create(_choice);
            shape.Points.Add(_start);
            shape.Points.Add(_end);
            shape.StrokeThickness = strokeThickNess;
            shape.Color = color;
            shape.DashStyle = dashStyle;
            shape.ZIndex = _currentZIndex;
            _shape.Add(shape);
            _redoUndo.Add(shape);

            reDraw();

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

            // Get Ishape
            for (int i = 0; i < _shape.Count; i++)
            {
                if (_shape[i].hashCode == _selectedShape.GetHashCode())
                {
                    rotationSlider.Value = _shape[i].rotationAngle;
                    break;
                }
            }

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
                // Get Ishape
                for (int i = 0; i < _shape.Count; i++)
                {
                    if (_shape[i].hashCode == _selectedShape.GetHashCode())
                    {
                        rotationSlider.Value = _shape[i].rotationAngle;
                        break;
                    }
                }

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

            // Get Ishape
            for(int i = 0; i < _shape.Count; i++)
            {
                if (_shape[i].hashCode == _selectedShape.GetHashCode())
                {
                    _currentIShape = _shape[i];

                    _resizePoints.Clear();
                    _resizePoints = _currentIShape.GetControlPoints();
                    double x = (Math.Abs(_currentIShape.Points[0].X + _currentIShape.Points[1].X)) / 2;
                    double y = (Math.Abs(_currentIShape.Points[0].Y + _currentIShape.Points[1].Y)) / 2;
                    for(int j = 0; j < _resizePoints.Count; j++)
                    {
                        UIElement uIElement = _resizePoints[j].drawPoint(0, new Point(x, y));
                        uIElement.MouseDown += UIElement_MouseDown;
                        if (_resizePoints[j].typeName.Equals("TopLeft"))
                        {
                            uIElement.MouseMove += UIElement_MouseMoveTopleft;

                        }
                        else if (_resizePoints[j].typeName.Equals("BottomLeft"))
                        {
                            uIElement.MouseMove += UIElement_MouseMoveBottomLeft;

                        }
                        else if (_resizePoints[j].typeName.Equals("TopRight"))
                        {
                            uIElement.MouseMove += UIElement_MouseMoveTopRight;

                        }
                        else if (_resizePoints[j].typeName.Equals("BottomRight"))
                        {
                            uIElement.MouseMove += UIElement_MouseMoveBottomRight;

                        }
                        uIElement.MouseUp += UIElement_MouseUp;
                        drawingCanvas.Children.Add(uIElement);
                    }

                    _controlOutline = _shape[i].controlOutline();
                    drawingCanvas.Children.Add(_controlOutline);
                    rotationSlider.Value = _shape[i].rotationAngle;
                    break;
                }
            }

            offset = e.GetPosition(drawingCanvas);
            offset.Y -= Canvas.GetTop(_selectedShape);
            offset.X -= Canvas.GetLeft(_selectedShape);
            drawingCanvas.CaptureMouse();
        }
        private void RotationValueChanged_Handler(object sender, RoutedPropertyChangedEventArgs<double> e)
        {       
            if (_currentIShape == null) { return; }
           
            _currentIShape.rotationAngle = (int)rotationSlider.Value;
            reDraw();
            _controlOutline = _currentIShape.controlOutline();
            drawingCanvas.Children.Add(_controlOutline);
            _resizePoints.Clear();
            _resizePoints = _currentIShape.GetControlPoints();
            double x = (Math.Abs(_currentIShape.Points[0].X + _currentIShape.Points[1].X)) / 2;
            double y = (Math.Abs(_currentIShape.Points[0].Y + _currentIShape.Points[1].Y)) / 2;

            if (_currentIShape.Name.Equals("Line2D"))
            {
                x = _currentIShape.Points[0].X;
                y = _currentIShape.Points[0].Y;
            }

            for (int j = 0; j < _resizePoints.Count; j++)
            {
                UIElement uIElement = _resizePoints[j].drawPoint((int)rotationSlider.Value, new Point(x, y));
                uIElement.MouseDown += UIElement_MouseDown;

                if (_resizePoints[j].typeName.Equals("TopLeft"))
                {
                    uIElement.MouseMove += UIElement_MouseMoveTopleft;

                }
                else if (_resizePoints[j].typeName.Equals("BottomLeft"))
                {
                    uIElement.MouseMove += UIElement_MouseMoveBottomLeft;

                }
                else if (_resizePoints[j].typeName.Equals("TopRight"))
                {
                    uIElement.MouseMove += UIElement_MouseMoveTopRight;

                }
                else if (_resizePoints[j].typeName.Equals("BottomRight"))
                {
                    uIElement.MouseMove += UIElement_MouseMoveBottomRight;

                }

                uIElement.MouseUp += UIElement_MouseUp;
                drawingCanvas.Children.Add(uIElement);
            }
        }
        private void UIElement_MouseMoveTopleft(object sender, MouseEventArgs e)
        {
            if (_isResize)
            {
                Point position = e.GetPosition(drawingCanvas);
                _currentIShape.Points[0] = position;
                reDraw();
                _controlOutline = _currentIShape.controlOutline();
                drawingCanvas.Children.Add(_controlOutline);

                _resizePoints.Clear();
                _resizePoints = _currentIShape.GetControlPoints();
                double x = (Math.Abs(_currentIShape.Points[0].X + _currentIShape.Points[1].X)) / 2;
                double y = (Math.Abs(_currentIShape.Points[0].Y + _currentIShape.Points[1].Y)) / 2;
                for (int j = 0; j < _resizePoints.Count; j++)
                {
                    UIElement uIElement = _resizePoints[j].drawPoint(_currentIShape.rotationAngle, new Point(x, y));
                    uIElement.MouseDown += UIElement_MouseDown;
                    if (_resizePoints[j].typeName.Equals("TopLeft"))
                    {
                        uIElement.MouseMove += UIElement_MouseMoveTopleft;

                    }
                    else if (_resizePoints[j].typeName.Equals("BottomLeft"))
                    {
                        uIElement.MouseMove += UIElement_MouseMoveBottomLeft;

                    }
                    else if (_resizePoints[j].typeName.Equals("TopRight"))
                    {
                        uIElement.MouseMove += UIElement_MouseMoveTopRight;

                    }
                    else if (_resizePoints[j].typeName.Equals("BottomRight"))
                    {
                        uIElement.MouseMove += UIElement_MouseMoveBottomRight;

                    }
                    uIElement.MouseUp += UIElement_MouseUp;
                    drawingCanvas.Children.Add(uIElement);
                }
            }
        }
        private void UIElement_MouseMoveBottomLeft(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(drawingCanvas);

            if (_isResize)
            {
                _currentIShape.Points[0] = new Point(position.X, _currentIShape.Points[0].Y);
                _currentIShape.Points[1] = new Point(_currentIShape.Points[1].X, position.Y);
                reDraw();
                _controlOutline = _currentIShape.controlOutline();
                drawingCanvas.Children.Add(_controlOutline);

                _resizePoints.Clear();
                _resizePoints = _currentIShape.GetControlPoints();
                double x = (Math.Abs(_currentIShape.Points[0].X + _currentIShape.Points[1].X)) / 2;
                double y = (Math.Abs(_currentIShape.Points[0].Y + _currentIShape.Points[1].Y)) / 2;
                for (int j = 0; j < _resizePoints.Count; j++)
                {
                    UIElement uIElement = _resizePoints[j].drawPoint(_currentIShape.rotationAngle, new Point(x, y));
                    uIElement.MouseDown += UIElement_MouseDown;
                    if (_resizePoints[j].typeName.Equals("TopLeft"))
                    {
                        uIElement.MouseMove += UIElement_MouseMoveTopleft;

                    }
                    else if (_resizePoints[j].typeName.Equals("BottomLeft"))
                    {
                        uIElement.MouseMove += UIElement_MouseMoveBottomLeft;

                    }
                    else if (_resizePoints[j].typeName.Equals("TopRight"))
                    {
                        uIElement.MouseMove += UIElement_MouseMoveTopRight;

                    }
                    else if (_resizePoints[j].typeName.Equals("BottomRight"))
                    {
                        uIElement.MouseMove += UIElement_MouseMoveBottomRight;

                    }
                    uIElement.MouseUp += UIElement_MouseUp;
                    drawingCanvas.Children.Add(uIElement);
                }
            }
        }
        private void UIElement_MouseMoveTopRight(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(drawingCanvas);

            if (_isResize)
            {
                _currentIShape.Points[0] = new Point(_currentIShape.Points[0].X, position.Y);
                _currentIShape.Points[1] = new Point(position.X, _currentIShape.Points[1].Y);
                reDraw();
                _controlOutline = _currentIShape.controlOutline();
                drawingCanvas.Children.Add(_controlOutline);

                _resizePoints.Clear();
                _resizePoints = _currentIShape.GetControlPoints();
                double x = (Math.Abs(_currentIShape.Points[0].X + _currentIShape.Points[1].X)) / 2;
                double y = (Math.Abs(_currentIShape.Points[0].Y + _currentIShape.Points[1].Y)) / 2;
                for (int j = 0; j < _resizePoints.Count; j++)
                {
                    UIElement uIElement = _resizePoints[j].drawPoint(_currentIShape.rotationAngle, new Point(x, y));
                    uIElement.MouseDown += UIElement_MouseDown;
                    if (_resizePoints[j].typeName.Equals("TopLeft"))
                    {
                        uIElement.MouseMove += UIElement_MouseMoveTopleft;

                    }
                    else if (_resizePoints[j].typeName.Equals("BottomLeft"))
                    {
                        uIElement.MouseMove += UIElement_MouseMoveBottomLeft;

                    }
                    else if (_resizePoints[j].typeName.Equals("TopRight"))
                    {
                        uIElement.MouseMove += UIElement_MouseMoveTopRight;

                    }
                    else if (_resizePoints[j].typeName.Equals("BottomRight"))
                    {
                        uIElement.MouseMove += UIElement_MouseMoveBottomRight;

                    }
                    uIElement.MouseUp += UIElement_MouseUp;
                    drawingCanvas.Children.Add(uIElement);
                }
            }
        } 
        private void UIElement_MouseMoveBottomRight(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(drawingCanvas);

            if (_isResize)
            {
                _currentIShape.Points[1] = position;
                reDraw();
                _controlOutline = _currentIShape.controlOutline();
                drawingCanvas.Children.Add(_controlOutline);

                _resizePoints.Clear();
                _resizePoints = _currentIShape.GetControlPoints();
                double x = (Math.Abs(_currentIShape.Points[0].X + _currentIShape.Points[1].X)) / 2;
                double y = (Math.Abs(_currentIShape.Points[0].Y + _currentIShape.Points[1].Y)) / 2;
                for (int j = 0; j < _resizePoints.Count; j++)
                {
                    UIElement uIElement = _resizePoints[j].drawPoint(_currentIShape.rotationAngle, new Point(x, y));
                    uIElement.MouseDown += UIElement_MouseDown;
                    if (_resizePoints[j].typeName.Equals("TopLeft"))
                    {
                        uIElement.MouseMove += UIElement_MouseMoveTopleft;

                    }
                    else if (_resizePoints[j].typeName.Equals("BottomLeft"))
                    {
                        uIElement.MouseMove += UIElement_MouseMoveBottomLeft;

                    }
                    else if (_resizePoints[j].typeName.Equals("TopRight"))
                    {
                        uIElement.MouseMove += UIElement_MouseMoveTopRight;

                    }
                    else if (_resizePoints[j].typeName.Equals("BottomRight"))
                    {
                        uIElement.MouseMove += UIElement_MouseMoveBottomRight;

                    }
                    uIElement.MouseUp += UIElement_MouseUp;
                    drawingCanvas.Children.Add(uIElement);
                }
            }
        }
        private void UIElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isResize = true;
        }
        private void UIElement_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isResize = false;
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

                    for(int i = 0; i < _shape.Count; i++)
                    {
                        if (_shape[i].hashCode == _selectedShape.GetHashCode())
                        {
                            _currentIShape = _shape[i];
                            break;
                        }
                    }

                    reDraw();
                    _controlOutline = _currentIShape.controlOutline();
                    drawingCanvas.Children.Add(_controlOutline);

                    _resizePoints.Clear();
                    _resizePoints = _currentIShape.GetControlPoints();
                    double x = (Math.Abs(_currentIShape.Points[0].X + _currentIShape.Points[1].X)) / 2;
                    double y = (Math.Abs(_currentIShape.Points[0].Y + _currentIShape.Points[1].Y)) / 2;
                    for (int j = 0; j < _resizePoints.Count; j++)
                    {
                        UIElement uIElement = _resizePoints[j].drawPoint(_currentIShape.rotationAngle, new Point(x, y));
                        uIElement.MouseDown += UIElement_MouseDown;
                        if (_resizePoints[j].typeName.Equals("TopLeft"))
                        {
                            uIElement.MouseMove += UIElement_MouseMoveTopleft;

                        }
                        else if (_resizePoints[j].typeName.Equals("BottomLeft"))
                        {
                            uIElement.MouseMove += UIElement_MouseMoveBottomLeft;

                        }
                        else if (_resizePoints[j].typeName.Equals("TopRight"))
                        {
                            uIElement.MouseMove += UIElement_MouseMoveTopRight;

                        }
                        else if (_resizePoints[j].typeName.Equals("BottomRight"))
                        {
                            uIElement.MouseMove += UIElement_MouseMoveBottomRight;

                        }
                        uIElement.MouseUp += UIElement_MouseUp;
                        drawingCanvas.Children.Add(uIElement);
                    }

                }
                else if (_selectedShape is TextBox)
                {
                    if (!ctrlMode)
                    {
                        for(int i = 0; i < _shape.Count; i++)
                        {
                            if (_shape[i].hashCode == _selectedShape.GetHashCode())
                            {
                                _currentIShape = _shape[i];
                                break;
                            }
                        }
                        _currentIShape.Points[0] = new Point(position.X - offset.X, position.Y - offset.Y);
                        _currentIShape.Points[1] = new Point(position.X - offset.X + _selectedShape.RenderSize.Width, position.Y - offset.Y + _selectedShape.RenderSize.Height);
                        reDraw();
                        _controlOutline = _currentIShape.controlOutline();
                        drawingCanvas.Children.Add(_controlOutline);

                        _resizePoints.Clear();
                        _resizePoints = _currentIShape.GetControlPoints();
                        double x = (Math.Abs(_currentIShape.Points[0].X + _currentIShape.Points[1].X)) / 2;
                        double y = (Math.Abs(_currentIShape.Points[0].Y + _currentIShape.Points[1].Y)) / 2;
                        for (int j = 0; j < _resizePoints.Count; j++)
                        {
                            UIElement uIElement = _resizePoints[j].drawPoint(_currentIShape.rotationAngle, new Point(x, y));
                            uIElement.MouseDown += UIElement_MouseDown;
                            if (_resizePoints[j].typeName.Equals("TopLeft"))
                            {
                                uIElement.MouseMove += UIElement_MouseMoveTopleft;

                            }
                            else if (_resizePoints[j].typeName.Equals("BottomLeft"))
                            {
                                uIElement.MouseMove += UIElement_MouseMoveBottomLeft;

                            }
                            else if (_resizePoints[j].typeName.Equals("TopRight"))
                            {
                                uIElement.MouseMove += UIElement_MouseMoveTopRight;

                            }
                            else if (_resizePoints[j].typeName.Equals("BottomRight"))
                            {
                                uIElement.MouseMove += UIElement_MouseMoveBottomRight;

                            }
                            uIElement.MouseUp += UIElement_MouseUp;
                            drawingCanvas.Children.Add(uIElement);
                        }
                    }
                }
                else
                {
                    _currentIShape.Points[0] = new Point(position.X - offset.X, position.Y - offset.Y);
                    _currentIShape.Points[1] = new Point(position.X - offset.X + _selectedShape.RenderSize.Width, position.Y - offset.Y + _selectedShape.RenderSize.Height);
                    reDraw();
                    _controlOutline = _currentIShape.controlOutline();
                    drawingCanvas.Children.Add(_controlOutline);

                    _resizePoints.Clear();
                    _resizePoints = _currentIShape.GetControlPoints();
                    double x = (Math.Abs(_currentIShape.Points[0].X + _currentIShape.Points[1].X)) / 2;
                    double y = (Math.Abs(_currentIShape.Points[0].Y + _currentIShape.Points[1].Y)) / 2;
                    for (int j = 0; j < _resizePoints.Count; j++)
                    {
                        UIElement uIElement = _resizePoints[j].drawPoint(_currentIShape.rotationAngle, new Point(x, y));
                        uIElement.MouseDown += UIElement_MouseDown;
                        if (_resizePoints[j].typeName.Equals("TopLeft"))
                        {
                            uIElement.MouseMove += UIElement_MouseMoveTopleft;

                        }
                        else if (_resizePoints[j].typeName.Equals("BottomLeft"))
                        {
                            uIElement.MouseMove += UIElement_MouseMoveBottomLeft;

                        }
                        else if (_resizePoints[j].typeName.Equals("TopRight"))
                        {
                            uIElement.MouseMove += UIElement_MouseMoveTopRight;

                        }
                        else if (_resizePoints[j].typeName.Equals("BottomRight"))
                        {
                            uIElement.MouseMove += UIElement_MouseMoveBottomRight;

                        }
                        uIElement.MouseUp += UIElement_MouseUp;
                        drawingCanvas.Children.Add(uIElement);
                    }
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
                        if (_shape[i].Name == "Line2D")
                        {
                            _selectedShape = null;
                            return;
                        }
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
        private void Fill_Click(object sender, RoutedEventArgs e)
        {
            _fillMode = true;
        }
        private void BackStageSave_MouseDown(object sender, MouseButtonEventArgs e)
        {
            backStage.IsOpen = false;

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "Binary File (*.bin)|*.bin";
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Title = "Save as Binary File";

            // Create the data to save
            DrawingData drawingData = new DrawingData();
            drawingData._shape = _shape;
            drawingData._image = _image;

            if(saveFileDialog.ShowDialog() == true)
            {
                SaveCanvasToBin(drawingData, saveFileDialog.FileName);
            }

        }
        private void SaveCanvasToBin(DrawingData drawingData, string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.CreateNew)) { };

            string json = JsonConvert.SerializeObject(drawingData, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            File.WriteAllText(fileName, json);
        }
        private void BackStageLoad_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Binary File (*.bin)|*.bin";
            openFileDialog.RestoreDirectory = true;
            if(openFileDialog.ShowDialog() == true)
            {
                LoadCanvasFromBin(openFileDialog.FileName);
            }
        }
        private void LoadCanvasFromBin(string fileName)
        {
            string json = File.ReadAllText(fileName);
            DrawingData drawingData = JsonConvert.DeserializeObject<DrawingData>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            _shape = drawingData._shape;
            _image = drawingData._image;
            reDraw();
            backStage.IsOpen = false;

        }
        private void BackStageSaveAs_MouseDown(object sender, MouseButtonEventArgs e)
        {
            backStage.IsOpen = false;
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "PNG Image (*.png)|*.png|JPG Image (*.jpg)|*.jpg|Bitmap Image (*.bmp)|*.bmp";
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Title = "Save as PNG";


            if (saveFileDialog.ShowDialog() == true)
            {
                switch(saveFileDialog.FilterIndex)
                {
                    case 1:
                        SaveCanvasToPng(drawingCanvas, saveFileDialog.FileName);
                        break;
                    case 2:
                        SaveCanvasToJpg(drawingCanvas, saveFileDialog.FileName);
                        break;
                    case 3:
                        SaveCanvasToBmp(drawingCanvas, saveFileDialog.FileName);
                        break;
                }
            }
        }
        public void SaveCanvasToPng(Canvas canvas, string fileName)
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(canvas);

            BitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
            pngBitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            using (var ms = new System.IO.MemoryStream())
            {
                pngBitmapEncoder.Save(ms);
                ms.Close();
                System.IO.File.WriteAllBytes(fileName, ms.ToArray());
            }
        }
        public void SaveCanvasToJpg(Canvas canvas, string fileName)
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(canvas);

            BitmapEncoder bitmapEncoder = new JpegBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            using (var ms = new System.IO.MemoryStream())
            {
                bitmapEncoder.Save(ms);
                ms.Close();
                System.IO.File.WriteAllBytes(fileName, ms.ToArray());
            }
        }
        public void SaveCanvasToBmp(Canvas canvas, string fileName)
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(canvas);

            BitmapEncoder bmpBitmapEncoder = new BmpBitmapEncoder();
            bmpBitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            using (var ms = new System.IO.MemoryStream())
            {
                bmpBitmapEncoder.Save(ms);
                ms.Close();
                System.IO.File.WriteAllBytes(fileName, ms.ToArray());
            }
        }
        private void BackStageQuit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var result = MessageBox.Show("Do you want to quit?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
        private void SliderValueChanged_Handler(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(_startLoad != 0)
            {
                _startLoad--;
                return;
            }
            int value = (int)(sender as Slider).Value;
            var st = new ScaleTransform();
            if(value == 50)
            {
                st.ScaleX /= 2;
                st.ScaleY /= 2;
                drawingCanvas.RenderTransform = st;
            }
            if(value == 100)
            {
                st.ScaleX = 1;
                st.ScaleY = 1;
                drawingCanvas.RenderTransform = st;
            }
            if(value == 150)
            {
                st.ScaleX *= 1.5;
                st.ScaleY *= 1.5;
                drawingCanvas.RenderTransform = st;
            }
            if(value == 200)
            {
                st.ScaleX *= 2;
                st.ScaleY *= 2;
                drawingCanvas.RenderTransform = st;
            }
        }
        private void AddLayer_Click(object sender, RoutedEventArgs e)
        {
            
            _layers.Add(new Layer() { Name = "Layer", ZIndex = _totalIndex++ });
            if (_layers.Count == 1)
            {
                myListLayer.SelectedItem = _layers.First();
            }
        }
        private void DeleteLayer_Click(object sender, RoutedEventArgs e)
        {
            if(_layers.Count > 0)
            {
                if(myListLayer.SelectedIndex != -1)
                {
                    for(int i = _shape.Count -1; i >= 0; i--)
                    {
                        IShape shape= _shape[i];
                        if(shape.ZIndex == _layers[myListLayer.SelectedIndex].ZIndex)
                        {
                            _shape.Remove(shape);
                        }
                    }
                    reDraw();
                    _layers.RemoveAt(myListLayer.SelectedIndex);
                }
                if(_layers.Count == 0)
                {
                    _totalIndex = 0;
                }
            }
        }
        private void myListLayer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(myListLayer.SelectedIndex != -1)
            {
                _currentZIndex = _layers[myListLayer.SelectedIndex].ZIndex;
            }
        }
        private void BackStageNew_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var result = MessageBox.Show("Do you want to save before creating a new one?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                backStage.IsOpen = false;
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "Binary File (*.bin)|*.bin";
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.Title = "Save as Binary File";

                // Create the data to save
                DrawingData drawingData = new DrawingData();
                drawingData._shape = _shape;
                drawingData._image = _image;

                if (saveFileDialog.ShowDialog() == true)
                {
                    SaveCanvasToBin(drawingData, saveFileDialog.FileName);

                    var Paint = new MainWindow();
                    Paint.Show();
                    this.Close();
                }
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
                _currentUIElement = null;
                _selectedShape = null;
                _selectedImage = null;
                Canvas.SetZIndex(drawingCanvas, 0);
                Canvas.SetZIndex(drawingBorder, 1);
            }
            
        }
    }

}
