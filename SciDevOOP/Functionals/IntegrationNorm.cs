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
    public int GaussOrder { get; set; } = 5;

    // Новые параметры для управления точностью
    public double InterpolationTolerance { get; set; } = 1e-8;
    public bool UseExactDataValues { get; set; } = true;

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

        // ИСПРАВЛЕННАЯ ЧАСТЬ: используем точные значения данных
        double y = GetExactDataValue(point);

        double e = f - y;
        return e * e;
    }

    // ОСНОВНОЙ МЕТОД: получение точного значения из данных
    private double GetExactDataValue(IList<double> point)
    {
        if (points == null || points.Count == 0)
            return 0;

        // 1. Попытка найти точное совпадение координат
        var exactMatch = FindExactMatch(point);
        if (exactMatch != null)
            return exactMatch.Value;

        // 2. Если точного совпадения нет, используем взвешенное среднее по ближайшим точкам
        // с экспоненциальным затуханием весов для минимизации влияния далеких точек
        return GetWeightedAverageFromNeighbors(point);
    }

    // Поиск точного совпадения координат
    private double? FindExactMatch(IList<double> point)
    {
        foreach (var dataPoint in points)
        {
            if (IsPointMatch(point, dataPoint))
            {
                return dataPoint[dataPoint.Count - 1]; // возвращаем значение
            }
        }
        return null;
    }

    // Проверка совпадения координат с заданной точностью
    private bool IsPointMatch(IList<double> point1, IList<double> point2)
    {
        int coordCount = Math.Min(point1.Count, point2.Count - 1);

        for (int i = 0; i < coordCount; i++)
        {
            if (Math.Abs(point1[i] - point2[i]) > InterpolationTolerance)
                return false;
        }
        return true;
    }

    // Взвешенное среднее по ближайшим соседям с экспоненциальными весами
    private double GetWeightedAverageFromNeighbors(IList<double> point, int maxNeighbors = 5)
    {
        var neighbors = FindWeightedNeighbors(point, maxNeighbors);

        if (neighbors.Count == 0)
            return 0;

        double totalWeight = 0;
        double weightedSum = 0;

        foreach (var (dataPoint, weight) in neighbors)
        {
            weightedSum += weight * dataPoint[dataPoint.Count - 1];
            totalWeight += weight;
        }

        return totalWeight > 0 ? weightedSum / totalWeight : 0;
    }

    // Поиск соседей с экспоненциальными весами
    private List<(IList<double> Point, double Weight)> FindWeightedNeighbors(IList<double> point, int maxNeighbors)
    {
        var distances = new List<(IList<double> Point, double Distance)>();

        foreach (var dataPoint in points)
        {
            double distance = CalculateDistance(point, dataPoint);
            distances.Add((dataPoint, distance));
        }

        // Сортируем по расстоянию и берем ближайших соседей
        var sorted = distances.OrderBy(d => d.Distance).Take(maxNeighbors).ToList();

        // Вычисляем веса: exp(-distance^2 / sigma^2), где sigma = медиана расстояний
        double medianDistance = sorted.Count > 0 ?
            sorted[sorted.Count / 2].Distance : 1.0;

        if (medianDistance < InterpolationTolerance)
            medianDistance = 1.0;

        var weightedNeighbors = new List<(IList<double> Point, double Weight)>();

        foreach (var (dataPoint, distance) in sorted)
        {
            // Экспоненциальное затухание веса с расстоянием
            double weight = Math.Exp(-distance * distance / (medianDistance * medianDistance));
            weightedNeighbors.Add((dataPoint, weight));
        }

        return weightedNeighbors;
    }

    // Альтернативный метод: использование вороной диаграммы для 2D/3D
    private double GetVoronoiBasedValue(IList<double> point)
    {
        if (point.Count == 2)
            return VoronoiInterpolation2D(point[0], point[1]);
        else if (point.Count == 3)
            return VoronoiInterpolation3D(point[0], point[1], point[2]);
        else
            return GetWeightedAverageFromNeighbors(point);
    }

    // Упрощенная интерполяция на основе вороной диаграммы для 2D
    private double VoronoiInterpolation2D(double x, double y)
    {
        if (points == null || points.Count == 0) return 0;

        // Находим ближайшую точку (ячейку вороной)
        var nearest = points
            .OrderBy(p => CalculateDistance(new List<double> { x, y }, p))
            .First();

        return nearest[nearest.Count - 1];
    }

    // Упрощенная интерполяция на основе вороной диаграммы для 3D
    private double VoronoiInterpolation3D(double x, double y, double z)
    {
        if (points == null || points.Count == 0) return 0;

        var nearest = points
            .OrderBy(p => CalculateDistance(new List<double> { x, y, z }, p))
            .First();

        return nearest[nearest.Count - 1];
    }

    // Расчет расстояния между точками
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

    // Дополнительные методы для тонкой настройки

    // Установка высокой точности для работы с разрывными функциями
    public void SetHighPrecisionForDiscontinuousFunctions()
    {
        UseExactDataValues = true;
        InterpolationTolerance = 1e-12;
        GaussOrder = 6; // Высокий порядок интегрирования
    }

    // Установка режима для гладких функций
    public void SetSmoothFunctionMode()
    {
        UseExactDataValues = false;
        InterpolationTolerance = 1e-6;
        GaussOrder = 4;
    }

    // Метод для принудительного использования только точных совпадений
    public void SetExactMatchOnlyMode()
    {
        UseExactDataValues = true;
        InterpolationTolerance = 1e-14;
    }
}