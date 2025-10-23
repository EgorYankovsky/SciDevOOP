namespace SciDevOOP.IO;

interface IWriter
{
    /// <summary>
    /// Method, that writes solution.
    /// </summary>
    /// <param name="values">Vector of optimized parameters.</param>
    void Write(IList<double> values);
}