namespace Link;

public static class IOManager
{
    public static IEnumerable<string> ReadLines()
    {
        string? line;

        while ((line = Console.ReadLine()) != null)
            yield return line;
    }

    public static IEnumerable<string> ReadLines(string filePath)
    {
        require(File.Exists(filePath));

        return File.ReadLines(filePath);
    }

    public static void WriteLines(IEnumerable<string> lines)
    {
        require(lines != null);

        foreach (string line in lines)
            Console.WriteLine(line);
    }

    public static void WriteLines(string filePath, IEnumerable<string> lines, bool append = false)
    {
        require(lines != null);

        if (append)
            File.AppendAllLines(filePath, lines);
        else
            File.WriteAllLines(filePath, lines);
    }


    public static void WriteLine(string line) => Console.WriteLine(line);
    public static void WriteLine(string filePath, string line, bool append = false)
    {
        if (append)
            File.AppendAllText(filePath, line);
        else
            File.WriteAllText(filePath, line);
    }

    public static void Write(string str) => Console.Write(str);
    public static void WriteLine() => Console.WriteLine();
    public static void LogError(string message) => Console.Error.WriteLine(message);

    public static bool SamePath(string path1, string path2)
    {
        require(!string.IsNullOrWhiteSpace(path1));
        require(!string.IsNullOrWhiteSpace(path2));

        return string.Equals(Path.GetFullPath(path1), Path.GetFullPath(path2),
            StringComparison.OrdinalIgnoreCase);
    }
}
