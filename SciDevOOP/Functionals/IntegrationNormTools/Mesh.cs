using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Functionals.IntegrationNormTools;

internal class Mesh
{
    private IList<IVector> _points;
    private IList<IList<int>> _elements;

    private double FindDistance(IVector point1, IVector point2)
    {
        throw new NotImplementedException();
    }

    private void DelaunayTriangulation(IVector parameters)
    {
        var dim = _points[0].Count;
        if (_points.Count != dim)
            throw new ArgumentException("Too little points for triangulation.");
        
        var initialSimplexVerts = Enumerable.Range(0, dim + 1).ToList();
        _elements.Add(initialSimplexVerts);     // ???

        // ��������������� ���������� ����� � ������������:
        for (int i = dim + 1; i < Points.Count; i++)
        {
            var badSimplices = new List<IList<int>>();
            var boundaryFaces = new HashSet<List<int>>(new FaceComparer());

            // ������� ��� ���������, ��� ������� ����� ����� ����� ������ ��������� �����
            foreach (var simplex in _elements)
            {
                if (IsPointInCircumsphere(_points[i], simplex))
                    badSimplices.Add(simplex);
            }

            // ������� ������� "���" ��� badSimplices (�����, ������������� ����� ������ ������� ���������)
            foreach (var simplex in badSimplices)
            {
                var faces = GetFaces(simplex);
                foreach (var face in faces)
                {
                    if (!boundaryFaces.Remove(face))
                        boundaryFaces.Add(face);
                }
            }

            // ������� badSimplices
            foreach (var s in badSimplices)
                Simplices.Remove(s);

            // ��������� ����� ���������, ������������ ����� ������ � ��������
            foreach (var face in boundaryFaces)
            {
                var newVerts = new List<int>(face);
                newVerts.Add(i);
                _elements.Add(newVerts); // ???
            }
        }

    }

    public Mesh(IVector parameters)
    {
        _points = new();
        _elements = new();
        DelaunayTriangulation(parameters);
    }

    public IList<int> FindElem(IVector point)
    {
        throw new NotImplementedException();
    } 
}