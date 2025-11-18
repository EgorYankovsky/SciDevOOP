using System.Text;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.MathematicalObjects;
using SciDevOOP.Optimizators.LevenbergMarquardtTools.Solvers;

namespace SciDevOOP.Functions;

class ThinPlateSpline : IParametricFunction
{

    /// <summary>
    /// Represents a Thin Plate Spline of the form:
    /// S(x1...xn) = a_0 sum_{i=1}^n (a_i * x_i) + sum_{j=1}^k (w_j * f(||p - p_j||))
    /// where n - dimension, k - points amount, a_i - linear coefficients, w_j - weights,
    /// p, p_j - points, ||*|| - Carthesian norm, f(t) = t^2 * ln(t).
    /// </summary>
    class InternalThinPlateSpline : IFunction
    {
        public int PointsAmount;

        public IVector? Points;

        public IVector? w;

        public IVector? a;

        private static double Distance(IVector point1, IVector ponit2)
        {
            if (point1.Count != ponit2.Count) throw new ArgumentException($"Point1 dimension {point1.Count} isn't equal to point2 dimension {ponit2.Count}.");
            var sum = 0.0D;
            for (int i = 0; i < point1.Count - 1; ++i)
                sum  += Math.Pow(point1[i] - ponit2[i], 2);
            return Math.Sqrt(sum);

        }

        public static InternalThinPlateSpline Create(int PointsAmount, IVector points)
        {
            var sb = new StringBuilder();
            var A = new Matrix();
            var b = new Vector();
            var solver = new Gauss();   // ???

            var dimension = points.Count / PointsAmount;

            for (int i = 0; i < PointsAmount + dimension; ++i)
            {
                A.Add([]);
                var pnt1 = new Vector(points.Skip(i * dimension).Take(dimension));
                for (int j = 0; j < PointsAmount + dimension; ++j)
                {
                    if (i > j) A[i].Add(A[j][i]);
                    else if (i == j) A[i].Add(0.0D);
                    else 
                    {
                        if (i < PointsAmount && j < PointsAmount)
                        {
                            var pnt2 = new Vector(points.Skip(j * dimension).Take(dimension));
                            A[i].Add(Phi(Distance(pnt1, pnt2)));
                        }
                        else if (i < PointsAmount && !(j < PointsAmount))
                        {
                            if(j == PointsAmount) A[i].Add(1.0D);
                            else A[i].Add(pnt1[j - PointsAmount - 1]);
                        }
                        else A[i].Add(0.0D);
                    }
                }
            }

            for (int i = 0; i < PointsAmount + dimension; ++i)
            {
                if (i < PointsAmount) b.Add(new Vector(points.Skip(i * dimension).Take(dimension))[^1]);
                else b.Add(0.0D);
            }


            var x = solver.Solve(A, b).Select(xi => -1.0 * xi);

            return new InternalThinPlateSpline()
            {
                PointsAmount = PointsAmount,
                Points = points,
                w = new Vector(x.Take(PointsAmount)),
                a = new Vector(x.Skip(PointsAmount).Take(dimension + 1))
            };
        }

        private static double Phi(double t) => t > 0 ? t * t * Math.Log(t) : t == 0 ? 0 : throw new Exception($"Input value must be positive, but has {t}");

        public double Value(IVector point)
        {
            if (Points is null) throw new NullReferenceException("Vector of points wasn't initialized."); 
            if (Points.Count / PointsAmount != point.Count) throw new ArgumentException("Point dimension isn't equal to spline dimension.");
            if (w is null) throw new NullReferenceException("Vector of weights wasn't initialized.");
            var ans = a?[0] ?? throw new NullReferenceException("Vector of coefcicients wasn't initialized.");

            for (int i = 0; i < point.Count; ++i) ans += point[i] * a[i + 1];

            for (int j = 0; j < PointsAmount; ++j) 
                ans += w[j] + Phi(Distance(point, new Vector(Points.Skip(j * PointsAmount).Take(point.Count))));

            return ans;
        }
    }

    /// <summary>
    /// Method, that binds parameters to function.
    /// </summary>
    /// <param name="parameters">parameters = [k, x00,..., xn0, x01, ..., xn1, ..., x0k, ..., xnk] where n - dimension, k - points amount.</param>
    /// <returns>Generated InternalThinPlateSpline class.</returns>
    /// <exception cref="ArgumentException">Raises if vector of parameters was incorrect.</exception>
    public IFunction Bind(IVector parameters)
    {
        if ((parameters.Count - 1) % parameters[0] != 0) throw new ArgumentException("Wrong input parameters.");
        return InternalThinPlateSpline.Create(Convert.ToInt32(parameters[0]), new Vector(parameters.Skip(1)));
    }
}