using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Microsoft.Win32;
using Point = System.Windows.Point;
using Polygon = System.Windows.Shapes.Polygon;

namespace PointInPolygon;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly List<Point> _newPolygonPoints = [];
    private readonly NetTopologySuitePolygons _topologySuitePolygons = new();

    private  List<Point[]> _homePolygons =
        [[new Point(200, 100), new Point(400, 100), new Point (350, 200), new Point(450, 400)]];

    public MainWindow()
    {
        InitializeComponent();
        PrintHomePolygons();
        _topologySuitePolygons.Load(_homePolygons);
    }

    private void HomePointsCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        var point = e.GetPosition(HomeCanvas);

        var isInside = _topologySuitePolygons.Contains(point);
        var ellipse = new Ellipse
        {
            Width = 10, Height = 10,
            Fill = isInside ? Brushes.Green : Brushes.Red,
        };

        HomeCanvas.Children.Add(ellipse);
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
        _homePolygons.Clear();
        _homePolygons.Add(_newPolygonPoints.ToArray());
        _topologySuitePolygons.Load(_homePolygons);
        PrintHomePolygons();
        MyTabControl.SelectedIndex = 0;

        _newPolygonPoints.Clear();
        NewPolygonCanvas.Children.Clear();
    }

    private void ClearButton_OnClick(object sender, RoutedEventArgs e)
    {
        _newPolygonPoints.Clear();
        NewPolygonCanvas.Children.Clear();
    }

    private void UploadFile_OnClick(object sender, RoutedEventArgs e)
    {
        var fileDialog = new OpenFileDialog
        {
            DefaultExt = ".csv",
        };
        if (fileDialog.ShowDialog() == false)
            return;

        using var reader = new StreamReader(fileDialog.FileName);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var polygons = csv.GetRecords<CsvPolygonData>();
        _topologySuitePolygons.LoadFromWkt(polygons.Select(p => p.Wkt));

        _homePolygons = _topologySuitePolygons.Polygons;
        ScaleHomePolygons(50);
        _topologySuitePolygons.Load(_homePolygons);

        PrintHomePolygons();
    }

    private void PrintHomePolygons()
    {
        HomeCanvas.Children.Clear();
        foreach (var polygonPoints in _homePolygons)
        {
            var polygon = new Polygon
            {
                Stroke = Brushes.Black,
                Fill = Brushes.LightBlue,
                StrokeThickness = 2,
                Points = new PointCollection(polygonPoints),
            };
            HomeCanvas.Children.Add(polygon);
        }
    }

    private void ScaleHomePolygons(double scale)
    {
        _homePolygons = _homePolygons.Select(polygonPoints => polygonPoints.Select(point =>
                {
                    point.X -= 25;
                    point.X *= scale;

                    point.Y -= 45;
                    point.Y *= scale;
                    return point;
                })
                .ToArray())
            .ToList();
    }

    private class CsvPolygonData
    {
        [Name("WKT")]
        public string Wkt { get; set; } = null!;

        [Name("name")]
        public string Name { get; set; } = null!;

        [Name("description")]
        public string Description { get; set; } = null!;
    }
}
