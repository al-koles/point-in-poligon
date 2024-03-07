using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Implementation;
using NetTopologySuite.IO;
using Point = System.Windows.Point;

namespace PointInPolygon;

public class NetTopologySuitePolygons
{
    private readonly GeometryFactory _gf;
    private readonly List<Polygon> _polygons = [];

    public NetTopologySuitePolygons()
    {
        NtsGeometryServices.Instance = new NtsGeometryServices(
            // default CoordinateSequenceFactory
            CoordinateArraySequenceFactory.Instance,
            // default precision model
            new PrecisionModel(),
            // default SRID
            4326,
            // Geometry overlay operation function set to use (Legacy or NG)
            GeometryOverlay.NG,
            // Coordinate equality comparer to use (CoordinateEqualityComparer or PerOrdinateEqualityComparer)
            new CoordinateEqualityComparer());

        _gf = NtsGeometryServices.Instance.CreateGeometryFactory(4326);
    }

    public List<Point[]> Polygons =>
        _polygons.Select(p => p.Coordinates.Select(c => new Point(c.X, c.Y)).ToArray()).ToList();

    public void Load(List<Point[]> polygons)
    {
        var gfPolygons = polygons.Select(polygonPoints =>
            _gf.CreatePolygon(
                polygonPoints
                    .Append(polygonPoints[0])
                    .Select(p => new Coordinate(p.X, p.Y))
                    .ToArray()));

        _polygons.Clear();
        _polygons.AddRange(gfPolygons);
    }

    public void LoadFromWkt(IEnumerable<string> wktPolygons)
    {
        _polygons.Clear();
        var reader = new WKTReader();
        foreach (var wkt in wktPolygons)
        {
            var geometry = reader.Read(wkt);
            if (geometry is Polygon polygon)
                _polygons.Add(polygon);
            else if (geometry is GeometryCollection geometryCollection)
            {
                foreach (var g in geometryCollection.Geometries)
                {
                    if (g is Polygon p)
                        _polygons.Add(p);
                }
            }
        }
    }

    public bool Contains(Point point)
    {
        var gfPoint = _gf.CreatePoint(new Coordinate(point.X, point.Y));

        return _polygons.Any(p => p.Contains(gfPoint));
    }
}
