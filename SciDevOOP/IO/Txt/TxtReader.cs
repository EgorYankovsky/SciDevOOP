using System.IO;
using System.Reflection;

namespace SciDevOOP.IO.Txt;

public class TxtReader : IReader
{
#if DEBUG
    private readonly string _basePath = AppDomain.CurrentDomain.BaseDirectory.Replace("Debug\\net9.0\\", "Resources\\");
#endif

#if RELEASE
    private readonly string _basePath = AppDomain.CurrentDomain.BaseDirectory.Replace("Release\\net9.0\\", "Resources\\");
#endif

    private readonly FileInfo? _file;

    public TxtReader(string path)
    {
        _file = path.Split('\\').Length == 1 ? new FileInfo(_basePath + path) : new FileInfo(path);
    }

    public IList<IList<double>>? Read()
    {
        IList<IList<double>>? result = null;
        if (!_file!.Exists) throw new FileNotFoundException(_file.FullName);
        result = [];
        var lines = File.ReadAllLines(_file.FullName);
        var pointsAmount = int.Parse(lines[0]);
        for (var i = 0; i < pointsAmount; ++i)
        {
            result.Add([.. lines[1 + i].Split(" ").Select(double.Parse)]);
            if (result[i].Count != result.First().Count)
                throw new InvalidDataException(lines[1 + i]);
        }
        return result;
    }
}