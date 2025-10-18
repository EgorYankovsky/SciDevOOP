using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.MathematicalObjects;

namespace SciDevOOP.Optimizators.LevenbergMarquardtTools.Solvers;

class Gauss : ISolver
{
    public IVector Solve(IMatrix A, IVector b)
    {
        var n = A.Count;
        // Create copies.
        var M = new double[n, n];
        var B = new double[n];
        for (var i = 0; i < n; ++i)
        {
            B[i] = b[i];
            for (var j = 0; j < n; ++j)
                M[i, j] = A[i][j];
        }

        // Gauss forward elimination
        for (var k = 0; k < n; ++k)
        {
            // Search for the main element
            var maxRow = k;
            for (var i = k + 1; i < n; ++i)
                if (Math.Abs(M[i, k]) > Math.Abs(M[maxRow, k]))
                    maxRow = i;
            // Swap rows
            for (var j = 0; j < n; ++j)
                (M[k, j], M[maxRow, j]) = (M[maxRow, j], M[k, j]);
            (B[k], B[maxRow]) = (B[maxRow], B[k]);
            // Zeroing under diagonal values.
            for (var i = k + 1; i < n; ++i)
            {
                var coef = M[i, k] / M[k, k];
                for (var j = k; j < n; ++j)
                    M[i, j] -= coef * M[k, j];
                B[i] -= coef * B[k];
            }
        }
        // Backward elimination
        var x = new double[n];
        for (var i = n - 1; i >= 0; --i)
        {
            var sum = 0.0D;
            for (var j = i + 1; j < n; ++j)
                sum += M[i, j] * x[j];
            x[i] = (B[i] - sum) / M[i, i];
        }

        var res = new Vector(x);
        return res;
    }
}