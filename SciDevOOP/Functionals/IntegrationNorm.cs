using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.MathematicalObjects;

namespace SciDevOOP.Functionals;

class IntegrationNorm : IFunctional
{
    private IList<IList<double>> points; // Точки для численного интегрирования
    public IList<double> LowerBound { get; set; }
    public IList<double> UpperBound { get; set; }
    public IList<int> NumberOfPoints { get; set; }

    public IntegrationNorm(IList<double> lowerBound, IList<double> upperBound, IList<int> numberOfPoints = null)
    {
        LowerBound = lowerBound;
        UpperBound = upperBound;
        if (numberOfPoints == null)
        {
            NumberOfPoints = new List<int>();
            for (int i = 0; i<lowerBound.Count; i++)
            {
                NumberOfPoints.Add(1000);
            }
        }
        else
        {
            NumberOfPoints = numberOfPoints;
        }
    }

    double IFunctional.Value(IFunction function)
    {
        // L² норма ошибки: sqrt(∫|f(x) - y(x)|² dx)
        // где y(x) - интерполяция данных точек
        double integral = MultiDimensionalIntegration(function, LowerBound, UpperBound, NumberOfPoints);
        return Math.Sqrt(integral);
    }

    private double MultiDimensionalIntegration(IFunction function, IList<double> lower, IList<double> upper, IList<int> nPoints)
    {
        int dimesions = lower.Count;

        // Рекурсивное интегрирование по всем измерениям
        return RecursiveIntegration(function, lower, upper, nPoints, 0, new List<double>());
    }

    private double RecursiveIntegration(IFunction function, IList<double> lower, IList<double> upper, IList<int> nPoints, int currentDim, List<double> currentPoint)
    {
        if (currentDim == lower.Count)
        {
            // Для полной размерности вычисляем подынтегральное выражение
            return SquareError(function, currentPoint);
        }

        double a = lower[currentDim];
        double b = upper[currentDim];
        int n = nPoints[currentDim];

        if (n % 2 == 1) n++;

        double h = (b - a) / n;
        double sum = 0;

        for (int i = 1; i <= n; i++)
        {
            double x = a + i * h;
            List<double> newPoint = new List<double>(currentPoint) { x };

            double weight = GetSimpsonWeight(i, n);
            double partialIntegral = RecursiveIntegration(function, lower, upper, nPoints, currentDim + 1, newPoint);

            sum += weight * partialIntegral;
        }
        return sum * h / 3;
    }

    private double GetSimpsonWeight(int i, int n)
    {
        if (i == 0 || i == n) return 1;
        return (i % 2 == 0) ? 2 : 4;
    }


    private double SquareError(IFunction function, IList<double> point)
    {
        Vector input = new Vector();
        foreach (double coord in point)
        {
            input.Add(coord);
        }
        double functionValue = function.Value(input);
        // Находим ближайшую точку для y(x)
        double targetValue = InterpolateValue(point);
        double error = functionValue - targetValue;
        return error * error;
    }

    private double InterpolateValue(IList<double> point)
    {
        if (!point.Any()) return 0; /////////////////////////

        double minDistance = double.MaxValue;
        double bestValue = 0;
        // Простая линейная интерполяция
        foreach (var dataPoint in points)
        {
            double distance = CalculateDistance(point, dataPoint);
            if (distance < minDistance)
            {
                minDistance = distance;
                // Предполагаем, что последняя координата - значение функции
                bestValue = dataPoint[dataPoint.Count - 1];
            }
            //    if (x >= points[i].x && x <= points[i + 1].x)
            //    {
            //        double t = (x - points[i].x) / (points[i + 1].x - points[i].x);
            //        return points[i].y + t * (points[i + 1].y - points[i].y);
            //    }
            //}
            //return points[0].y; // fallback
        }
        return bestValue;
    }

    private double CalculateDistance(IList<double> point1, IList<double> point2)
    {
        double sum = 0;
        int minDim = Math.Min(point1.Count, point2.Count);

        for (int i = 0; i < minDim; i++)
        {
            double diff = point1[i] - point2[i];
            sum += diff * diff;
        }

        return Math.Sqrt(sum);
    }

    // Метод для установки точек данных
    public void SetDataPoints(IList<IList<double>> dataPoints)
    {
        this.points = dataPoints;
    }

    // Альтернативный конструктор с точками данных
    public IntegrationNorm(IList<double> lowerBounds, IList<double> upperBounds,
                          IList<IList<double>> dataPoints, IList<int> numberOfPoints = null)
        : this(lowerBounds, upperBounds, numberOfPoints)
    {
        SetDataPoints(dataPoints);
    }
}