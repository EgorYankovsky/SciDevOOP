using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using System.Collections;

namespace SciDevOOP.MathematicalObjects;

public class Matrix : List<IList<double>>, IDenseMatrix
{
    public int N { get; init; } 
    public int M { get; init; }

    double IDenseMatrix.this[int i, int j] 
    { 
        get => throw new NotImplementedException(); 
        set => throw new NotImplementedException(); 
    }
    
    public double this[int i, int j]
    {
        get => base[i][j];
        set => base[i][j] = value;
    }

    public Matrix GetTransposed()
    {
        var transposedMatrix = new Matrix(M, N);
        for (var i = 0; i < M; ++i)
            for (var j = 0; j < N; ++j)
                transposedMatrix[i, j] = base[j][i];
        return transposedMatrix;
    }

    IVector IDenseMatrix.Multiplicate(IVector v)
    {
        throw new NotImplementedException();
    }

    IMatrix IDenseMatrix.Multiplicate(IMatrix M)
    {
        return this * (M as Matrix)!;
    }

    IMatrix IDenseMatrix.GetTransposed() => GetTransposed();

    public static Matrix operator *(Matrix m1, Matrix m2)
    {
        if (m1.M != m2.N || m1.N != m2.M) throw new ArgumentException("Matrix multiplication is impossible.");
        var ans = new Matrix(m1.N, m1.N);

        for (var i = 0; i < ans.N; i++)
            for (var j = 0; j < ans.N; j++)
                for (var k = 0; k < m1.M; k++)
                    ans[i, j] += m1[i, k] * m2[k, j];
        return ans;
    }

    public Matrix(int n, int m)
    {
        (N, M) = (n, m);
        for (var i = 0; i < N; ++i)
        {
            Add([]);
            for (var j = 0; j < M; ++j)
                base[i].Add(0);
        }
    }

    public Matrix()
    {

    }

    public Matrix(Matrix other)
    {
        // Copy matrix.
    }
}