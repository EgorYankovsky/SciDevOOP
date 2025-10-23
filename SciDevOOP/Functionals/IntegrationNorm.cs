using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.MathematicalObjects;

namespace SciDevOOP.Functionals;

class IntegrationNorm : IFunctional
{
    public IList<IList<double>> points; // Точки для численного интегрирования
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

        //if (n % 2 == 1) n++;

        //double h = (b - a) / n;
        //double sum = 0;

        //for (int i = 0; i < n; i++)
        //{
        //    double x = a + i * h;
        //    List<double> newPoint = new List<double>(currentPoint) { x };

        //    double weight = GetSimpsonWeight(i, n);
        //    double partialIntegral = RecursiveIntegration(function, lower, upper, nPoints, currentDim + 1, newPoint);

        //    sum += weight * partialIntegral;
        //}
        //return sum * h / 3;

        var (weights, nodes) = GetGaussWeightsAndNodes(n);

        double sum = 0;

        for (int i = 0; i < n; i++)
        {
            // Преобразование координат из [-1,1] в [a,b]
            double x = ((b - a) * nodes[i] + (a + b)) / 2.0;
            List<double> newPoint = new List<double>(currentPoint) { x };

            double partialIntegral = RecursiveIntegration(function, lower, upper, nPoints, currentDim + 1, newPoint);
            sum += weights[i] * partialIntegral;
        }

        // Масштабирующий коэффициент для преобразования координат
        return sum * (b - a) / 2.0;    
    }

    // Метод для получения весов и узлов квадратуры Гаусса
    private (double[] weights, double[] nodes) GetGaussWeightsAndNodes(int n)
    {
        switch (n)
        {
            case 1:
                return (new double[] { 2.0 }, new double[] { 0.0 });
            case 2:
                return (new double[] { 1.0, 1.0 },
                        new double[] { -0.5773502691896257, 0.5773502691896257 });
            case 3:
                return (new double[] { 0.5555555555555556, 0.8888888888888888, 0.5555555555555556 },
                        new double[] { -0.7745966692414834, 0.0, 0.7745966692414834 });
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
                // Для большего количества точек используем 6 по умолчанию
                return GetGaussWeightsAndNodes(6);
        }
    }

    //private double GetSimpsonWeight(int i, int n)
    //{
    //    if (i == 0 || i == n) return 1;
    //    return (i % 2 == 0) ? 2 : 4;
    //}


    private double SquareError(IFunction function, IList<double> point)
    {
        // Создаем вектор для функции - все координаты кроме последней (если есть значение)
        var inputCoords = point;

        var input = new Vector(inputCoords);
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