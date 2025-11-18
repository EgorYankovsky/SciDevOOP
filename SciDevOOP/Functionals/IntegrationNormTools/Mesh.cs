//using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

//namespace SciDevOOP.Functionals.IntegrationNormTools;

//internal class Mesh
//{
//    private IList<IVector> _points;
//    private IList<IList<int>> _elements;

//    private double FindDistance(IVector point1, IVector point2)
//    {
//        throw new NotImplementedException();
//    }

//    private void DelaunayTriangulation(IVector parameters)
//    {
//        var dim = _points[0].Count;
//        if (_points.Count != dim)
//            throw new ArgumentException("Too little points for triangulation.");
        
//        var initialSimplexVerts = Enumerable.Range(0, dim + 1).ToList();
//        _elements.Add(initialSimplexVerts);     // ???

//        // »нкрементальное добавление точек в триангул€цию:
//        for (int i = dim + 1; i < Points.Count; i++)
//        {
//            var badSimplices = new List<IList<int>>();
//            var boundaryFaces = new HashSet<List<int>>(new FaceComparer());

//            // Ќаходим все простексы, дл€ которых нова€ точка лежит внутри описанной сферы
//            foreach (var simplex in _elements)
//            {
//                if (IsPointInCircumsphere(_points[i], simplex))
//                    badSimplices.Add(simplex);
//            }

//            // Ќаходим границу "дыр" вне badSimplices (грани, принадлежащие ровно одному плохому симплексу)
//            foreach (var simplex in badSimplices)
//            {
//                var faces = GetFaces(simplex);
//                foreach (var face in faces)
//                {
//                    if (!boundaryFaces.Remove(face))
//                        boundaryFaces.Add(face);
//                }
//            }

//            // ”дал€ем badSimplices
//            foreach (var s in badSimplices)
//                Simplices.Remove(s);

//            // ƒобавл€ем новые простексы, образованные новой точкой и границей
//            foreach (var face in boundaryFaces)
//            {
//                var newVerts = new List<int>(face);
//                newVerts.Add(i);
//                _elements.Add(newVerts); // ???
//            }
//        }

//    }

//    public Mesh(IVector parameters)
//    {
//        _points = new();
//        _elements = new();
//        DelaunayTriangulation(parameters);
//    }

//    public IList<int> FindElem(IVector point)
//    {
//        throw new NotImplementedException();
//    } 
//}