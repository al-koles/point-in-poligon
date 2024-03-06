using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PointInPolygon;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly List<Point> _newPolygonPoints = [];

    private readonly List<Point> _homePolygonPoints =
        [new Point(200, 100), new Point(400, 100), new Point (350, 200), new Point(450, 400)];

    public MainWindow()
    {
        InitializeComponent();
        PrintHomePolygon();
    }

    private void HomePointsCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        var point = e.GetPosition(HomePointsCanvas);
        var ellipse = new Ellipse
        {
            Width = 10, Height = 10,
            Fill = HomePolygon.IsPointInside(point) ? Brushes.Green : Brushes.Red,
        };

        HomePointsCanvas.Children.Add(ellipse);
        Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
        Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);
    }

    private void NewPolygonCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        var newPoint = e.GetPosition(NewPolygonCanvas);
        var ellipse = new Ellipse
        {
            Width = 10, Height = 10,
            Fill = Brushes.Blue,
        };

        NewPolygonCanvas.Children.Add(ellipse);
        Canvas.SetLeft(ellipse, newPoint.X - ellipse.Width / 2);
        Canvas.SetTop(ellipse, newPoint.Y - ellipse.Height / 2);

        if (_newPolygonPoints.Any())
        {
            var lastPoint = _newPolygonPoints.Last();
            var line = new Line
            {
                X1 = lastPoint.X,
                Y1 = lastPoint.Y,
                X2 = newPoint.X,
                Y2 = newPoint.Y,
                Stroke = Brushes.Black,
                StrokeThickness = 4,
            };
            NewPolygonCanvas.Children.Add(line);
        }

        _newPolygonPoints.Add(newPoint);
    }

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        _homePolygonPoints.Clear();
        _homePolygonPoints.AddRange(_newPolygonPoints);
        PrintHomePolygon();
        MyTabControl.SelectedIndex = 0;
        
        _newPolygonPoints.Clear();
        NewPolygonCanvas.Children.Clear();
    }

    private void ClearButton_OnClick(object sender, RoutedEventArgs e)
    {
        _newPolygonPoints.Clear();
        NewPolygonCanvas.Children.Clear();
    }

    private void PrintHomePolygon()
    {
        HomePolygon.Points.Clear();
        HomePointsCanvas.Children.Clear();
        _homePolygonPoints.ForEach(p => HomePolygon.Points.Add(p));
    }
}
