using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.MathematicalObjects;

namespace SciDevOOP.Optimizators.LevenbergMarquardtTools.Solvers;

class NewGauss : ISolver
{
    public IVector Solve(IMatrix A, IVector b)
    {
        if (A.Count != A[0].Count || A.Count != b.Count) throw new ArgumentException($"Impossible to solve SLAE with Matrix of {A.Count}x{A[0].Count} and Vector of {b.Count}");
        var n = A.Count;
        var scale = 1.0D;

        // Create augmented matrix
        var augmented = new double[n, n + 1];

        for (var i = 0; i < n; i++)
        {
            for (var j = 0; j < n; j++)
                augmented[i, j] = A[i][j];
                //augmented[i, j] = A[i * n + j];
            augmented[i, n] = scale * b[i];
        }

        // Gauss forward elimination
        for (var i = 0; i < n; i++)
        {
            // Search for the main element
            var maxRow = i;
            for (var k = i + 1; k < n; k++)
            {
                if (Math.Abs(augmented[k, i]) > Math.Abs(augmented[maxRow, i]))
                    maxRow = k;
            }

            // Swap rows
            if (maxRow != i)
                for (var k = 0; k <= n; k++)
                    (augmented[maxRow, k], augmented[i, k]) = (augmented[i, k], augmented[maxRow, k]);


            // Degeneracy check
            if (Math.Abs(augmented[i, i]) < 1e-15)
            {
                // Return zeros
                var zeroSolution = new Vector();
                for (var idx = 0; idx < n; idx++)
                    zeroSolution.Add(0);
                return zeroSolution;
            }

            // Exception
            for (var k = i + 1; k < n; k++)
            {
                var factor = augmented[k, i] / augmented[i, i];
                for (var j = i; j <= n; j++)
                    augmented[k, j] -= factor * augmented[i, j];
            }
        }

        // Backward elimination
        var solution = new Vector();
        for (var i = 0; i < n; i++)
            solution.Add(0);

        for (var i = n - 1; i >= 0; i--)
        {
            solution[i] = augmented[i, n];
            for (var j = i + 1; j < n; j++)
                solution[i] -= augmented[i, j] * solution[j];
            solution[i] /= augmented[i, i];
        }

        return solution;
    }
}