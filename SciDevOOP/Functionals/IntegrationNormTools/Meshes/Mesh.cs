using SciDevOOP.MathematicalObjects;
using System.Text;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Functionals.IntegrationNormTools.Meshes;

internal class DelaunayTriangulationND(IList<IVector> points)
{
    public IList<IVector> Points = points;

    public IList<IList<int>> Simplices = [];

    public void Triangulate()
    {
        int dim = Points[0].Count;

        // Check points amount for triangulation.
        if (Points.Count <= dim) throw new ArgumentException("Too less points for triangulation.");

        // Initialize first simplex (например, создается один полный симплекс из первых dim+1 точек)
        var initialSimplexVerts = Enumerable.Range(0, dim + 1).ToList();
        Simplices.Add(initialSimplexVerts);

        // Инкрементальное добавление точек в триангуляцию:
        for (int i = dim + 1; i < Points.Count; i++)
        {
            var badSimplices = new List<IList<int>>();
            var boundaryFaces = new HashSet<List<int>>(new FaceComparer());

            // Находим все простексы, для которых новая точка лежит внутри описанной сферы
            foreach (var simplex in Simplices)
            {
                if (IsPointInCircumsphere(Points[i], simplex))
                    badSimplices.Add(simplex);
            }

            // Находим границу "дыр" вне badSimplices (грани, принадлежащие ровно одному плохому симплексу)
            foreach (var simplex in badSimplices)
            {
                var faces = GetFaces(simplex);
                foreach (var face in faces)
                {
                    if (!boundaryFaces.Remove(face))
                        boundaryFaces.Add(face);
                }
            }

            // Удаляем badSimplices
            foreach (var s in badSimplices)
                Simplices.Remove(s);

            // Добавляем новые простексы, образованные новой точкой и границей
            foreach (var face in boundaryFaces)
            {
                var newVerts = new List<int>(face);
                newVerts.Add(i);
                Simplices.Add(newVerts);
            }
        }
    }

    private bool IsPointInCircumsphere(IVector p, IList<int> simplex)
    {
        int dim = p.Count;
        int N = simplex.Count;
        // Матрица A и вектор B из уравнений для центра сферы
        double[,] A = new double[dim, dim];
        double[] B = new double[dim];

        //IVector p0 = Points[simplex.Vertices[0]];
        IVector p0 = Points[simplex[0]];
        for (int i = 0; i < dim; i++)
        {
            for (int j = 0; j < dim; j++)
                A[i, j] = 2 * (Points[simplex[i + 1]][j] - p0[j]);                
            B[i] = 0;
            for (int j = 0; j < dim; j++)
                B[i] += Math.Pow(Points[simplex[i + 1]][j], 2) - Math.Pow(p0[j], 2);
        }

        double[] center = SolveLinearSystem(A, B);

        double radius = 0;
        for (int i = 0; i < dim; i++)
            radius += Math.Pow(center[i] - p0[i], 2);
        radius = Math.Sqrt(radius);

        double dist = 0;
        for (int i = 0; i < dim; i++)
            dist += Math.Pow(center[i] - p[i], 2);
        dist = Math.Sqrt(dist);

        return dist < radius;
    }

    private double[] SolveLinearSystem(double[,] A, double[] B)
    {
        int n = B.Length;
        double[,] a = new double[n, n];
        double[] b = new double[n];
        Array.Copy(A, a, A.Length);
        Array.Copy(B, b, B.Length);

        for (int i = 0; i < n; i++)
        {
            int maxRow = i;
            for (int k = i + 1; k < n; k++)
                if (Math.Abs(a[k, i]) > Math.Abs(a[maxRow, i]))
                    maxRow = k;

            for (int k = i; k < n; k++)
                (a[i, k], a[maxRow, k]) = (a[maxRow, k], a[i, k]);
            (b[i], b[maxRow]) = (b[maxRow], b[i]);

            for (int k = i + 1; k < n; k++)
            {
                double factor = a[k, i] / a[i, i];
                for (int j = i; j < n; j++)
                    a[k, j] -= factor * a[i, j];
                b[k] -= factor * b[i];
            }
        }

        double[] x = new double[n];
        for (int i = n - 1; i >= 0; i--)
        {
            double sum = 0.0;
            for (int j = i + 1; j < n; j++)
                sum += a[i, j] * x[j];
            x[i] = (b[i] - sum) / a[i, i];
        }
        return x;
    }


    private List<List<int>> GetFaces(IList<int> simplex)
    {
        var faces = new List<List<int>>();
        int k = simplex.Count;
        for (int i = 0; i < k; i++)
        {
            var face = new List<int>(simplex);
            face.RemoveAt(i);
            face.Sort();
            faces.Add(face);
        }
        return faces;
    }

    private class FaceComparer : IEqualityComparer<List<int>>
    {
        public bool Equals(List<int> a, List<int> b)
        {
            if (a.Count != b.Count) return false;
            for (int i = 0; i < a.Count; i++)
                if (a[i] != b[i]) return false;
            return true;
        }
        public int GetHashCode(List<int> obj)
        {
            int hash = 17;
            foreach (var v in obj)
                hash = hash * 31 + v.GetHashCode();
            return hash;
        }
    }
}


public class Mesh
{
    private IList<IVector> _points = [];
    private IList<IList<int>> _elements = [];

    private DelaunayTriangulationND delaunayTriangulationND;

    public Mesh(IList<IList<double>> parameters)
    {
        IList<IVector> updatedParameters = [];
        foreach (var point in parameters)
            updatedParameters.Add(new Vector(point.ToList()));
        delaunayTriangulationND = new(updatedParameters);
        delaunayTriangulationND.Triangulate();

        _points = delaunayTriangulationND.Points;
        _elements = delaunayTriangulationND.Simplices;

    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine("Points:");
        for (int i = 0; i < _points.Count; ++i)
            sb.AppendLine($"{_points[i]}");
        
        sb.AppendLine("Elements:");
        for (int i = 0; i < _elements.Count; ++i)
        {
            foreach (var item in _elements[i])
                sb.Append($"{item} ");
            sb.AppendLine();
        }
        return sb.ToString();
    }

    internal IList<int> FindElem(IVector point)
    {
        throw new NotImplementedException();
    }
}