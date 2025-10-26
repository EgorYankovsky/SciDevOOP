using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.MathematicalObjects;
using System.Drawing;

namespace SciDevOOP.Functionals;

class IntegrationNorm : IFunctional
{
    public IList<IList<double>> points; // Точки данных
    public IList<double> LowerBound { get; set; }
    public IList<double> UpperBound { get; set; }
    public IList<int> NumberOfPoints { get; set; }
    public int GaussOrder { get; set; } = 5; // порядок квадратуры Гаусса: 3, 4 или 5

    public IntegrationNorm(IList<double> lowerBound, IList<double> upperBound, int gaussOrder = 5, IList<int> numberOfPoints = null)
    {
        LowerBound = lowerBound;
        UpperBound = upperBound;
        GaussOrder = Math.Clamp(gaussOrder, 3, 6);

        NumberOfPoints = numberOfPoints ?? Enumerable.Repeat(gaussOrder, lowerBound.Count).ToList();
    }

    public void SetDataPoints(IList<IList<double>> dataPoints)
    {
        points = dataPoints;
    }

    public IntegrationNorm(IList<double> lowerBounds, IList<double> upperBounds, IList<IList<double>> dataPoints, int gaussOrder = 4)
        : this(lowerBounds, upperBounds, gaussOrder)
    {
        SetDataPoints(dataPoints);
    }

    double IFunctional.Value(IFunction function)
    {
        double integral = MultiDimensionalIntegration(function, LowerBound, UpperBound, NumberOfPoints);
        return Math.Sqrt(integral);
    }

    private double MultiDimensionalIntegration(IFunction function, IList<double> lower, IList<double> upper, IList<int> nPoints)
    {
        return RecursiveIntegration(function, lower, upper, nPoints, 0, new List<double>());
    }

    private double RecursiveIntegration(IFunction function, IList<double> lower, IList<double> upper, IList<int> nPoints, int dim, List<double> point)
    {
        if (dim == lower.Count)
            return SquareError(function, point);

        double a = lower[dim];
        double b = upper[dim];
        int n = nPoints[dim];
        var (weights, nodes) = GetGaussWeightsAndNodes(n);

        double sum = 0.0;
        for (int i = 0; i < nodes.Length; i++)
        {
            double x = ((b - a) * nodes[i] + (a + b)) / 2.0;
            var newPoint = new List<double>(point) { x };
            double partial = RecursiveIntegration(function, lower, upper, nPoints, dim + 1, newPoint);
            sum += weights[i] * partial;
        }
        return sum * (b - a) / 2.0;
    }

    private (double[] weights, double[] nodes) GetGaussWeightsAndNodes(int n)
    {
        switch (n)
        {
            case 3:
                return (new double[] { 5.0 / 9, 8.0 / 9, 5.0 / 9 },
                        new double[] { -Math.Sqrt(3.0 / 5.0), 0.0, Math.Sqrt(3.0 / 5.0) });
            case 4:
                return (new double[] { 0.3478548451374538, 0.6521451548625461,
                                       0.6521451548625461, 0.3478548451374538 },
                        new double[] { -0.8611363115940526, -0.3399810435848563,
                                        0.3399810435848563, 0.8611363115940526 });
            case 5:
                return (new double[] { 0.2369268850561891, 0.4786286704993665, 0.5688888888888889,
                                       0.4786286704993665, 0.2369268850561891 },
                        new double[] { -0.9061798459386640, -0.5384693101056831, 0.0,
                                        0.5384693101056831, 0.9061798459386640 });
            case 6:
                return (new double[] { 0.1713244923791704, 0.3607615730481386, 0.4679139345726910,
                                       0.4679139345726910, 0.3607615730481386, 0.1713244923791704 },
                        new double[] { -0.9324695142031521, -0.6612093864662645, -0.2386191860831969,
                                       0.2386191860831969, 0.6612093864662645, 0.9324695142031521 });
            default:
                return GetGaussWeightsAndNodes(5);
        }
    }

    private double SquareError(IFunction function, IList<double> point)
    {
        Vector x = new Vector(point);
        double f = function.Value(x);
        double y = InterpolateValue(point);
        double e = f - y;
        return e * e;
    }

    private double InterpolateValue(IList<double> point)
    {
        // Для 2D данных используем билинейную интерполяцию
        if (point.Count == 2)
            return BilinearInterpolation(point[0], point[1]);

        // Для общего случая - интерполяция по ближайшим соседям с весами
        return InverseDistanceWeighting(point);
    }

    // Билинейная интерполяция для 2D данных
    private double BilinearInterpolation(double x, double y)
    {
        if (points == null || points.Count == 0) return 0;

        // Находим окружающие точки
        var sortedByX = points.OrderBy(p => p[0]).ToList();
        var sortedByY = points.OrderBy(p => p[1]).ToList();

        // Создаем точку для поиска соседей
        var currentPoint = new List<double> { x, y };

        // Находим ближайшие точки для интерполяции
        var neighbors = FindNearestNeighbors(currentPoint, 4); // Берем 4 ближайшие точки

        if (neighbors.Count < 3)
            return neighbors.FirstOrDefault().Value;

        // Для треугольной интерполяции
        return TriangularInterpolation(x, y, neighbors);
    }

    // Интерполяция с обратными весами расстояний
    private double InverseDistanceWeighting(IList<double> point, double power = 2.0)
    {
        if (points == null || points.Count == 0) return 0;

        double sumWeights = 0;
        double sumValues = 0;

        foreach (var dataPoint in points)
        {
            double distance = CalculateDistance(point, dataPoint);
            if (distance < 1e-10) // Если точка совпадает с одной из данных
                return dataPoint[dataPoint.Count - 1];

            double weight = 1.0 / Math.Pow(distance, power);
            sumWeights += weight;
            sumValues += weight * dataPoint[dataPoint.Count - 1];
        }

        return sumValues / sumWeights;
    }

    // Треугольная интерполяция для 2D
    private double TriangularInterpolation(double x, double y, List<(IList<double> Point, double Value)> neighbors)
    {
        if (neighbors.Count < 3) return neighbors.First().Value;

        // Находим три ближайшие точки, образующие треугольник
        var triangle = FindTriangle(x, y, neighbors);
        if (triangle.Count != 3) return InverseDistanceWeighting(new List<double> { x, y });

        // Барицентрическая интерполяция
        return BarycentricInterpolation(x, y, triangle);
    }

    private double BarycentricInterpolation(double x, double y, List<(IList<double> Point, double Value)> triangle)
    {
        var p1 = triangle[0].Point; double v1 = triangle[0].Value;
        var p2 = triangle[1].Point; double v2 = triangle[1].Value;
        var p3 = triangle[2].Point; double v3 = triangle[2].Value;

        double x1 = p1[0], y1 = p1[1];
        double x2 = p2[0], y2 = p2[1];
        double x3 = p3[0], y3 = p3[1];

        double det = (y2 - y3) * (x1 - x3) + (x3 - x2) * (y1 - y3);

        double w1 = ((y2 - y3) * (x - x3) + (x3 - x2) * (y - y3)) / det;
        double w2 = ((y3 - y1) * (x - x3) + (x1 - x3) * (y - y3)) / det;
        double w3 = 1 - w1 - w2;

        return w1 * v1 + w2 * v2 + w3 * v3;
    }

    private List<(IList<double> Point, double Value)> FindNearestNeighbors(IList<double> point, int k)
    {
        return points
            .Select(p => (Point: p, Distance: CalculateDistance(point, p), Value: p[p.Count - 1]))
            .OrderBy(t => t.Distance)
            .Take(k)
            .Select(t => (t.Point, t.Value))
            .ToList();
    }

    private List<(IList<double> Point, double Value)> FindTriangle(double x, double y, List<(IList<double> Point, double Value)> neighbors)
    {
        // Простая реализация - берем три ближайшие точки
        return neighbors.Take(3).ToList();
    }

    private double CalculateDistance(IList<double> a, IList<double> b)
    {
        int dim = Math.Min(a.Count, b.Count - 1); // Учитываем, что последний элемент b - значение
        double sum = 0;
        for (int i = 0; i < dim; i++)
        {
            double diff = a[i] - b[i];
            sum += diff * diff;
        }
        return Math.Sqrt(sum);
    }
}
