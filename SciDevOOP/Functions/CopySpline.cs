using System.Text;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.MathematicalObjects;
using SciDevOOP.Mesh;

namespace SciDevOOP.Functions;

class NewSplineFunction : IParametricFunction
{
    /// <summary>
    /// Represents a Hermite smoothing spline of the form:
    /// P(x) = sum_{i = 1}^{2n} (q_i * psi_i(x))
    /// where q_i - coefficient, psi_i - basis function.
    /// </summary>
    class InternalNewSplineFunction : IFunction, IMeshable, IWritableFunction
    {
        public IMesh? mesh;

        // Basis functions.
        private static double Phi1(double t) => 1.0D - 3.0D * t * t + 2.0D * t * t * t;
        private static double Phi2(double t) => t - 2.0D * t * t + t * t * t;
        private static double Phi3(double t) => 3.0D * t * t - 2.0D * t * t * t;
        private static double Phi4(double t) => -1.0D * t * t + t * t * t;

        string IWritableFunction.ToString()
        {
            StringBuilder sb = new();
            for (int i = 0; i < (q?.Count ?? 0); ++i)
                sb.AppendLine($"q_{i} = {q![i]}");
            return sb.ToString();
        }

        public IVector? q;   // Coefficients.
        public IList<IVector>? points;   // Spline mesh.
        public IList<IVector>? uniques;     // Unique coordinates above each axis.

        /// <summary>
        /// Method, that finds index of spline coefficient according to point.
        /// </summary>
        /// <param name="point">Point to search.</param>
        /// <param name="axisPoints">Axis where to search.</param>
        /// <returns>Index of spline function.</returns>
        private static int FindSplineIndex(IVector point, IVector axisPoints)
        {
            var indexLeft = 0;
            var indexRight = axisPoints.Count - 1;
            while (indexRight - indexLeft > 1)
            {
                var middleIndex = (indexLeft + indexRight) / 2;
                if (point[0] == axisPoints![middleIndex])
                {
                    indexLeft = middleIndex;
                    break;
                }
                else if (point[0] < axisPoints[middleIndex]) indexRight = middleIndex;
                else if (point[0] > axisPoints[middleIndex]) indexLeft = middleIndex;
            }
            return indexLeft;
        }


        private IList<(int, int)> FindIndexes(IVector point)
        {
            if (point.Count != points?.Count)
                throw new ArgumentException("Point dimension isn't equal to mesh dimension.");
            IList<(int, int)> ans = [];
            for (int i = 0; i < point.Count; ++i)
            {
                var indexI = FindSplineIndex(new Vector { point[i] }, uniques![i]);
                ans.Add((indexI, indexI + 1));
            }
            return ans;
        }

        double IFunction.Value(IVector point)
        {
            if (point.Count != points?.Count)
                throw new ArgumentException("Point dimension isn't equal to mesh dimension.");
            var ans = 0.0D;
            var dimension = uniques!.Count;
            var pointsAmount = uniques![0].Count;

            var indexes = FindIndexes(point);
            var indexes1 = indexes.Select(ind => ind.Item1);
            var localElement = mesh!.GetElementsIndexes([..indexes1]);

            var bias = Convert.ToInt32(Math.Pow(2, point.Count));

            var qShki = new List<IVector>();
            foreach (var item in localElement)
                qShki.Add(new Vector([.. q!.Skip(item * bias).Take(bias)]));


            for (int i = 0; i < point.Count; ++i)
            {
                if (point[i] < uniques![i][0] || point[i] > uniques![i][^1])
                    throw new ArgumentException("Point is out of spline range.");
                if (uniques![i][^1] - point[i] <= 1e-15)
                {
                    var hLast = uniques![i][^1] - uniques![i][^2];
                    ans += qShki![i][^4] * Phi1(1.0D) +
                           hLast * qShki![i][^3] * Phi2(1.0D) +
                           qShki![i][^2] * Phi3(1.0D) +
                           hLast * qShki![i][^1] * Phi4(1.0D);
                }
                else
                {
                    var h = uniques![i][indexes[i].Item2] - uniques![i][indexes[i].Item1];
                    var eps = (point[i] - uniques![i][indexes[i].Item1]) / h;
                    ans += q![2 * indexes[i].Item1] * Phi1(eps) +
                           h * q[2 * indexes[i].Item1 + 1] * Phi2(eps) +
                           q![2 * indexes[i].Item2] * Phi3(eps) +
                           h * q![2 * indexes[i].Item2 + 1] * Phi4(eps);
                }
            }
            return ans;
        }

        IVector IMeshable.GetMesh() => throw new NotImplementedException();
    }

    /// <summary>
    /// Method, that binds parameters to function.
    /// </summary>
    /// <param name="parameters">parameters = [n, q0..0,..., q_2m..2k, x..0,...,xm..k]</param>
    /// <returns>Generated InternalSplineFunction class.</returns>
    /// <exception cref="NotImplementedException">Raises if vector of parameters was incorrect.</exception>
    public IFunction Bind(IVector parameters)
    {
        var n = Convert.ToInt32(parameters[0]);
        if ((parameters.Count - 1) % (Math.Pow(2, n) + n) != 0) throw new ArgumentException("Incorrect parameters.");
        int pointsAmount = (parameters.Count - 1) / Convert.ToInt32(Math.Pow(2, n) + n);
        IList<IVector> _points = [];
        IList<IVector> _uniques = [];
        for (int i = 0; i < n; ++i) _points.Add(new Vector());
        for (int i = 0; i < n; ++i) _uniques.Add(new Vector());

        for (int i = 0; i < pointsAmount; ++i)
            for (int j = 0; j < n; ++j)
            {
                var indexSoso = 1 + pointsAmount * Convert.ToInt32(Math.Pow(2, n)) + i * n + j;
                _points[j].Add(parameters[indexSoso]);
                if (!_uniques[j].Contains(parameters[indexSoso]))
                    _uniques[j].Add(parameters[indexSoso]);
            }

        IMeshBuilder meshBuilder = new MeshBuilderND();

        return new InternalNewSplineFunction()
        {
            q = new Vector(parameters.Skip(1).Take(pointsAmount * Convert.ToInt32(Math.Pow(2, n)))),
            points = _points,
            uniques = _uniques,
            mesh = meshBuilder.Build(_uniques)
        };
    }
}