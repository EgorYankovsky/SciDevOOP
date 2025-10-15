using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.MathematicalObjects;

namespace SciDevOOP.Functionals;

class IntegrationNorm : IFunctional
{
    public List<(double x, double y)> points; // Точки для численного интегрирования
    public double LowerBound { get; set; }
    public double UpperBound { get; set; }
    public int NumberOfPoints { get; set; } = 1000;

    public IntegrationNorm(double lowerBound, double upperBound, int numberOfPoints = 1000)
    {
        LowerBound = lowerBound;
        UpperBound = upperBound;
        NumberOfPoints = numberOfPoints;
    }

    double IFunctional.Value(IFunction function)
    {
        // L² норма ошибки: sqrt(∫|f(x) - y(x)|² dx)
        // где y(x) - интерполяция данных точек
        double integral = SimpsonIntegration(function, LowerBound, UpperBound, NumberOfPoints);
        return Math.Sqrt(integral);
    }

    private double SimpsonIntegration(IFunction function, double a, double b, int n)
    {
        if (n % 2 == 1) n++;

        double h = (b - a) / n;
        double sum = SquareError(function, a) + SquareError(function, b);

        for (int i = 1; i < n; i++)
        {
            double x = a + i * h;
            double value = SquareError(function, x);

            if (i % 2 == 0)
                sum += 2 * value;
            else
                sum += 4 * value;
        }

        return sum * h / 3;
    }

    private double SquareError(IFunction function, double x)
    {
        // Находим ближайшую точку для y(x)
        double targetY = InterpolateY(x);
        double functionValue = function.Value(new Vector() { x });
        double error = functionValue - targetY;
        return error * error;
    }

    private double InterpolateY(double x)
    {
        // Простая линейная интерполяция
        for (int i = 0; i < points.Count - 1; i++)
        {
            if (x >= points[i].x && x <= points[i + 1].x)
            {
                double t = (x - points[i].x) / (points[i + 1].x - points[i].x);
                return points[i].y + t * (points[i + 1].y - points[i].y);
            }
        }
        return points[0].y; // fallback
    }
}