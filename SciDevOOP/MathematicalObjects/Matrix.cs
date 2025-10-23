using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.MathematicalObjects;

public class Matrix : List<IList<double>>, IDenseMatrix, IMatrixMultiplicand
{
    public int N => Count;
    public int M => this[0].Count;

    IVector IMatrixMultiplicand.Multiplicate(IVector v)
    {
        if (M != v.Count) throw new Exception("Matrix Vector multiplication is impossible.");
        var ans = new Vector();
        for (var i = 0; i < N; ++i)
        {
            var sum = 0.0;
            for (var j = 0; j < v.Count; ++j)
                sum += this[i][j] * v[j];
            ans.Add(sum);
        }
        return ans;
    }

    IMatrix IMatrixMultiplicand.Multiplicate(IMatrix A)
    {
        if (M != A.Count || N != A[0].Count) throw new Exception("Matrix and matrix multiplication is impossible.");
        var ans = new Matrix(N, N);

        for (var i = 0; i < ans.N; i++)
            for (var j = 0; j < ans.N; j++)
                for (var k = 0; k < M; k++)
                    ans[i][j] += this[i][k] * A[k][j];
        return ans;
    }

    IMatrix IDenseMatrix.GetTransposed()
    {
        var transposedMatrix = new Matrix(M, N);
        for (var i = 0; i < M; ++i)
            for (var j = 0; j < N; ++j)
                transposedMatrix[i][j] = base[j][i];
        return transposedMatrix;
    }

    public Matrix(int n, int m)
    {
        for (var i = 0; i < n; ++i)
        {
            Add([]);
            for (var j = 0; j < m; ++j)
                base[i].Add(0);
        }
    }

    public Matrix() {}

    public Matrix(Matrix other)
    {
        // Copy matrix.
    }
}