using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using System.Collections;

namespace SciDevOOP.MathematicalObjects;

public class Matrix : IMatrix
{
    private double[,] _values;

    public int N { get; init; } 
    public int M { get; init; }

    public double this[int i, int j]
    {
        get => _values[i, j];
        set => _values[i, j] = value;
    }

    public Matrix GetTransposed()
    {
        var transposedMatrix = new Matrix(M, N);
        for (int i = 0; i < M; ++i)
            for (int j = 0; j < N; ++j)
                transposedMatrix[i, j] = _values[j, i];
        return transposedMatrix;
    }

    public static Matrix operator *(Matrix m1, Matrix m2)
    {
        if (m1.M != m2.N || m1.N != m2.M) throw new ArgumentException("Matrix multiplication is impossible.");
        var ans = new Matrix(m1.N, m1.N);

        for (int i = 0; i < ans.N; i++)
            for (int j = 0; j < ans.N; j++)
                for (int k = 0; k < m1.M; k++)
                    ans[i, j] += m1[i, k] * m2[k, j];
        return ans;
    }

    public Matrix(int n, int m)
    {
        N = n;
        M = m;
        _values = new double[N, M];
    }

    public Matrix(Matrix other)
    {
        // Copy matrix.
    }

    [Obsolete(message:"No need usage.", true)]
    public int Count => throw new NotImplementedException();
    [Obsolete(message: "No need usage.", true)]
    public bool IsReadOnly => throw new NotImplementedException();
    [Obsolete(message: "No need usage.", true)]
    public IList<double> this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    [Obsolete(message: "No need usage.", true)]
    public int IndexOf(IList<double> item)
    {
        throw new NotImplementedException();
    }
    [Obsolete(message: "No need usage.", true)]
    public void Insert(int index, IList<double> item)
    {
        throw new NotImplementedException();
    }
    [Obsolete(message: "No need usage.", true)]
    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }
    [Obsolete(message: "No need usage.", true)]
    public void Add(IList<double> item)
    {
        throw new NotImplementedException();
    }
    [Obsolete(message: "No need usage.", true)]
    public void Clear()
    {
        throw new NotImplementedException();
    }
    [Obsolete(message: "No need usage.", true)]
    public bool Contains(IList<double> item)
    {
        throw new NotImplementedException();
    }
    [Obsolete(message: "No need usage.", true)]
    public void CopyTo(IList<double>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }
    [Obsolete(message: "No need usage.", true)]
    public bool Remove(IList<double> item)
    {
        throw new NotImplementedException();
    }
    [Obsolete(message: "No need usage.", true)]
    public IEnumerator<IList<double>> GetEnumerator()
    {
        throw new NotImplementedException();
    }
    [Obsolete(message: "No need usage.", true)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}