using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.MathematicalObjects;

public class Vector : List<double>, IVector
{
    public Vector() : base() { }

    public Vector(int  capacity) : base(capacity) { }

    public Vector(IEnumerable<double> collection) : base(collection) {}
    
    // Make as IVector.
    public static Vector operator *(Matrix A, Vector b)
    {
        var ans = new Vector();
        for (var i = 0; i < b.Count; ++i)
        {
            var sum = 0.0;
            for (var j = 0; j < b.Count; ++j)
                sum += A[i, j] * b[j];
            ans.Add(sum);
        }
        return ans;
    }
}