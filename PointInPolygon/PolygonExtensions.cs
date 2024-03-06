using System.Windows;
using System.Windows.Shapes;

namespace PointInPolygon;

public static class PolygonExtensions
{
    public static bool IsPointInside(this Polygon polygon, Point point)
    {
        return PointInPolygon(polygon.Points.ToArray(), point.X, point.Y);
    }

    private static bool PointInPolygon(Point[] points, double x, double y)
    {
        // Get the angle between the point and the
        // first and last vertices.
        var maxPoint = points.Length - 1;
        var totalAngle = GetAngle(
            points[maxPoint].X, points[maxPoint].Y,
            x, y,
            points[0].X, points[0].Y);

        // Add the angles from the point
        // to each other pair of vertices.
        for (var i = 0; i < maxPoint; i++)
        {
            totalAngle += GetAngle(
                points[i].X, points[i].Y,
                x, y,
                points[i + 1].X, points[i + 1].Y);
        }

        // The total angle should be 2 * PI or -2 * PI if
        // the point is in the polygon and close to zero
        // if the point is outside the polygon.
        return Math.Abs(totalAngle) > 0.000001;
    }

    // Return the angle ABC.
    // Return a value between PI and -PI.
    // Note that the value is the opposite of what you might
    // expect because Y coordinates increase downward.
    private static double GetAngle(double ax, double ay, double bx, double by, double cx, double cy)
    {
        // Get the dot product.
        var dotProduct = DotProduct(ax, ay, bx, by, cx, cy);

        // Get the cross product.
        var crossProduct = CrossProductLength(ax, ay, bx, by, cx, cy);

        // Calculate the angle.
        return Math.Atan2(crossProduct, dotProduct);
    }

    // Return the dot product AB . BC.
    // Note that AB x BC = |AB| * |BC| * Cos(theta).
    private static double DotProduct(double ax, double ay, double bx, double by, double cx, double cy)
    {
        // Get the vectors' coordinates.
        var bax = ax - bx;
        var bay = ay - by;
        var bcx = cx - bx;
        var bcy = cy - by;

        // Calculate the dot product.
        return bax * bcx + bay * bcy;
    }

    // Return the cross product AB x BC.
    // The cross product is a vector perpendicular to AB
    // and BC having length |AB| * |BC| * Sin(theta) and
    // with direction given by the right-hand rule.
    // For two vectors in the X-Y plane, the result is a
    // vector with X and Y components 0 so the Z component
    // gives the vector's length and direction.
    private static double CrossProductLength(double ax, double ay, double bx, double by, double cx, double cy)
    {
        // Get the vectors' coordinates.
        var bax = ax - bx;
        var bay = ay - by;
        var bcx = cx - bx;
        var bcy = cy - by;

        // Calculate the Z coordinate of the cross product.
        return bax * bcy - bay * bcx;
    }
}
