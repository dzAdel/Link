using Link.Exceptions;
using System.Text.RegularExpressions;

namespace Link;

sealed class PathsDateApp
{
    public bool EnableCasing { get; set; }
    public bool SelectCreationDate { get; set; }
    public bool SelectUTC { get; set; }
    public bool PrependInput { get; set; }
    public bool IgnoreTime { get; set; }

    public string Separator
    {
        get => m_sep;
        set
        {
            require(value != null);
            m_sep = value;
        }
    }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Returns, for each line in the input sequence, the date and time the specified file or " +
                    "directory was last written to." +
                    "The result is in ISO 8601 extended format (YYYY-MM-DDThh:mm:ss)." +
                    "Non-existent input paths are ignored." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\n--rx:\tUse regular expression to extract, from the input line, the target path." +
                    "\n\t'ptrn' represents the regular expression pattern to match against each input line." +
                    "\n\tThe input lines for which ‘ptrn’ does not match are ignored." +
                    "\n\t'path' is a transform pattern to apply to each input line to extract the file or directory" +
                    "\n\tfor which the date is to seek for." +
                    "\n\tIt can consist of any combination of literal text and substitutions, " +
                    "\n\tsuch as capturing group that is identified by a number or a name." +
                    "\n--cs:\tSpecifies case-sensitive matching." +
                    "\n\tIt's an error to specify this option without the '--rx' switch." +
                    "\n--crd:\tReturns the creation date instead of the modification date." +
                    "\n--utc:\tReturns the date in Coordinated Universal Time." +
                    "\n--pi:\tFor each line in the output sequence, prepends its input line, using 'sep' as separator." +
                    "\n\tIf 'sep' is missing, a colon is used." +
                    "\n--notm:\tReturns the date component only." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count != 0)
                throw new BadUsageException();

            string? rxPattern = null, rxPath = null;
            PathsDateApp app = new();

            foreach (string opt in argParser.Options.Keys)
            {
                IReadOnlyList<string> values = argParser.Options[opt];

                switch (opt)
                {
                    case "rx":
                        if (values.Count != 2)
                            throw new BadUsageException();

                        rxPattern = values[0];
                        rxPath = values[1];
                        break;

                    case "crd":
                        if (values.Count != 0)
                            throw new BadUsageException();

                        app.SelectCreationDate = true;
                        break;

                    case "utc":
                        if (values.Count != 0)
                            throw new BadUsageException();

                        app.SelectUTC = true;
                        break;

                    case "pi":
                        if (values.Count > 1)
                            throw new BadUsageException();

                        app.PrependInput = true;

                        if (values.Count == 1)
                            app.Separator = values[0];

                        break;

                    case "notm":
                        if (values.Count != 0)
                            throw new BadUsageException();

                        app.IgnoreTime = true;
                        break;

                    case "cs":
                        if (values.Count != 0)
                            throw new BadUsageException();

                        app.EnableCasing = true;
                        break;

                    default:
                        throw new BadArgException($"--{opt}");
                }
            }

            if (rxPattern != null)
            {
                assert(rxPath != null);
                app.Run(rxPattern, rxPath);
            }
            else
            {
                if (app.EnableCasing)
                    throw new BadUsageException();

                app.Run();
            }

            return 0;
        }
        catch (Exception ex)
        {
            IOManager.LogError($"{AppName}: {ex.Message}");
        }

        return -1;
    }

    //private:
    string m_sep = ":";

    static string Usage => $"{AppName} [--rx ptrn path [--cs]] [--crd][--utc][--pi [sep]][--notm][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(PathsDateApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        string fmt = IgnoreTime ? "yyyy-MM-dd" : "yyyy-MM-ddTHH:mm:ss";

        foreach (string path in IOManager.ReadLines())
            try
            {
                if (File.GetAttributes(path) == (FileAttributes)(-1)) //does not exists
                    continue;

                DateTime dt = GetTime(path);
                string output = PrependInput ? $"{path}{m_sep}{dt.ToString(fmt)}" : dt.ToString(fmt);

                IOManager.WriteLine(output);
            }
            catch (Exception ex)
            {
                IOManager.LogError($"{AppName}: {ex.Message}");
            }
    }

    void Run(string rxPattern, string rxPath)
    {
        require(rxPattern != null);
        require(rxPath != null);

        string fmt = IgnoreTime ? "yyyy-MM-dd" : "yyyy-MM-ddTHH:mm:ss";

        RegexOptions opt = RegexOptions.Compiled;

        if (!EnableCasing)
            opt |= RegexOptions.IgnoreCase;

        Regex rx = new(rxPattern, opt);

        foreach (string str in IOManager.ReadLines())
            try
            {
                Match m = rx.Match(str);

                if (m.Success)
                {
                    string path = m.Result(rxPath);

                    if (File.GetAttributes(path) == (FileAttributes)(-1)) //does not exists
                        continue;

                    DateTime dt = GetTime(path);
                    string output = PrependInput ? $"{str}{m_sep}{dt.ToString(fmt)}" : dt.ToString(fmt);

                    IOManager.WriteLine(output);
                }
            }
            catch (Exception ex)
            {
                IOManager.LogError($"{AppName}: {ex.Message}");
            }
    }

    DateTime GetTime(string path)
    {
        if (SelectCreationDate)
            return SelectUTC ? File.GetCreationTimeUtc(path) : File.GetCreationTime(path);

        return SelectUTC ? File.GetLastWriteTimeUtc(path) : File.GetLastWriteTime(path);
    }
}
