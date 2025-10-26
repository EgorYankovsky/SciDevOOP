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
    class InternalSplineFunction : IFunction
    {
        // Basis functions.
        private double phi1(double t) => 1.0 - 3.0*t*t + 2.0*t*t*t; 
        private double phi2(double t) => t - 2.0*t*t + t*t*t; 
        private double phi3(double t) => 3.0*t*t - 2.0*t*t*t; 
        private double phi4(double t) => -1.0*t*t + t*t*t; 

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
            while (indexLeft <= indexRight)
            {
                var middleIndex = (indexLeft + indexRight) / 2;
                if (point[0] == x![middleIndex])
                {
                    indexLeft = middleIndex;
                    break;
                }
                else if (point[0] < x[middleIndex])
                    indexRight = middleIndex;
                else if (point[0] > x[middleIndex])
                    indexLeft = middleIndex;
            }
            return indexLeft;
        }

        double IFunction.Value(IVector point)
        {
            var index = FindSplineIndex(point);
            var h = x![index + 1] - x![index];
            var eps = (point[0] - x![index]) / h;
            return phi1(eps) + h * phi2(eps) + phi3(eps) + h * phi4(eps);
        }
    }

    /// <summary>
    /// Method, that binds parameters to function.
    /// </summary>
    /// <param name="parameters">parameters = [q0,..., q_2n, x0,...,xn]</param>
    /// <returns>Generated InternalSplineFunction class.</returns>
    /// <exception cref="NotImplementedException"></exception>
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