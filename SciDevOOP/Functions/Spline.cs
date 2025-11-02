using System.Text;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.MathematicalObjects;

namespace SciDevOOP.Functions;

class SplineFunction : IParametricFunction
{
    /// <summary>
    /// Represents a Hermite smoothing spline of the form:
    /// P(x) = sum_{i = 1}^{2n} (q_i * psi_i(x))
    /// where q_i - coefficient, psi_i - basis function.
    /// </summary>
    class InternalSplineFunction : IFunction, IMeshable, IWritableFunction
    {
        // Basis functions.
        private static double Phi1(double t) => 1.0D - 3.0D*t*t + 2.0D*t*t*t; 
        private static double Phi2(double t) => t - 2.0D*t*t + t*t*t; 
        private static double Phi3(double t) => 3.0D*t*t - 2.0D*t*t*t; 
        private static double Phi4(double t) => -1.0D*t*t + t*t*t;

        string IWritableFunction.ToString()
        {
            StringBuilder sb = new();
            for (int i = 0; i < (q?.Count ?? 0); ++i)
                sb.AppendLine($"q_{i} = {q![i]}");
            return sb.ToString();        
        }

        public IVector? q;   // Coefficients.
        public IVector? x;   // Spline mesh.

        /// <summary>
        /// Method, that finds index of spline according to point.
        /// </summary>
        /// <param name="point">Point to search.</param>
        /// <returns>Index of spline function.</returns>
        private int FindSplineIndex(IVector point)
        {
            var indexLeft = 0;
            var indexRight = x!.Count - 1;
            while (indexRight - indexLeft > 1)
            {
                var middleIndex = (indexLeft + indexRight) / 2;
                if (point[0] == x![middleIndex])
                {
                    indexLeft = middleIndex;
                    break;
                }
                else if (point[0] < x[middleIndex]) indexRight = middleIndex;
                else if (point[0] > x[middleIndex]) indexLeft = middleIndex;
            }
            return indexLeft;
        }

        double IFunction.Value(IVector point)
        {
            if (point[0] < x![0] || point[0] > x![^1])
                throw new ArgumentException("Point is out of spline range.");
            if (x![^1] - point[0] <= 1e-15)
            {
                var hLast = x![^1] - x![^2];
                return q![^4] * Phi1(1.0D) +
                       hLast * q![^3] * Phi2(1.0D) +
                       q![^2] * Phi3(1.0D) +
                       hLast * q![^1] * Phi4(1.0D);
            }
            var index = FindSplineIndex(point);
            var h = x![index + 1] - x![index];
            var eps = (point[0] - x![index]) / h;
            return q![2 * index] * Phi1(eps) +
                   h * q[2 * index + 1] * Phi2(eps) +
                   q![2 * (index + 1)] * Phi3(eps) +
                   h * q![2 * (index + 1) + 1] * Phi4(eps);
        }

        IVector IMeshable.GetMesh() => x is not null ? x : new Vector();
    }

    /// <summary>
    /// Method, that binds parameters to function.
    /// </summary>
    /// <param name="parameters">parameters = [q0,..., q_2n, x0,...,xn]</param>
    /// <returns>Generated InternalSplineFunction class.</returns>
    /// <exception cref="NotImplementedException">Raises if vector of parameters was incorrect.</exception>
    public IFunction Bind(IVector parameters)
    {
        if (parameters.Count % 3 != 0) throw new ArgumentException("Incorrect parameters.");
        var n = parameters.Count / 3;
        return new InternalSplineFunction()
        {
            q = new Vector(parameters.Take(2 * n)),
            x = new Vector(parameters.Skip(2 * n).Take(n))
        };
    }
}