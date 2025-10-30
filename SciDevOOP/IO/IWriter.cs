using SciDevOOP.Functions;

namespace SciDevOOP.IO;

interface IWriter
{
    /// <summary>
    /// Method, that writes solution.
    /// </summary>
    /// <param name="values">Vector of optimized parameters.</param>
    void Write(IList<double> values);

    /// <summary>
    /// Method, that writes prettified solution.
    /// </summary>
    /// <param name="function">Function, binded with result parameters.</param>
    void Write(IWritableFunction function); 
}