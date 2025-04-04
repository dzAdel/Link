using Link.Exceptions;
using System.Text.RegularExpressions;



namespace Link;

sealed class RelativePathsApp
{
    public RelativePathsApp(string path)
    {
        require(!string.IsNullOrWhiteSpace(path));

        m_path = path;
    }

    public bool PrependInput { get; set; }
    public bool EnableCasing { get; set; }

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
                string hlp = "Returns, for each path in the input sequence, a relative path to a specified directory." +
                    "Returns input line if the paths don't share the same root." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\ndir:\tThe path the result should be relative to." +
                    "\n\tThis parameter is always considered to be a directory." +
                    "\n--rx:\tUses regular expression to extract, from the input line, the target path." +
                    "\n\t‘ptrn’ represents a regular expression pattern to match against each input line." +
                    "\n\tThe input lines for which ‘ptrn’ does not match are ignored." +
                    "\n\t‘path' is a transform pattern to apply to each input line" +
                    "\n\tto extract the taget file or directory path." +
                    "\n\tIt can consist of any combination of literal text and substitutions, " +
                    "\n\tsuch as capturing group that is identified by a number or a name" +
                    "\n--cs:\tSpecifies case-sensitive matching." +
                    "\n\tIt's an error to specify this option without the '--rx' switch." +
                    "\n--pi:\tFor each line in the output sequence, prepends its input line, using 'sep' as separator." +
                    "\n\tIf 'sep' is missing, a colon is used." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count != 1 || string.IsNullOrWhiteSpace(argParser.Parameters[0]))
                throw new BadUsageException();

            RelativePathsApp app = new(argParser.Parameters[0]);
            string? rxPattern = null, rxPath = null;


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

                    case "pi":
                        if (values.Count > 1)
                            throw new BadUsageException();

                        app.PrependInput = true;

                        if (values.Count == 1)
                            app.Separator = values[0];

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
    readonly string m_path;
    string m_sep = ":";

    static string Usage => $"{AppName} dir [--rx ptrn path [--cs]] [--pi [sep]][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(RelativePathsApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        foreach (string path in IOManager.ReadLines())
            try
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    string relPath = Path.GetRelativePath(m_path, path);
                    IOManager.WriteLine(PrependInput ? $"{path}{m_sep}{relPath}" : relPath);
                }
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

        RegexOptions opts = RegexOptions.CultureInvariant | RegexOptions.Compiled;

        if (!EnableCasing)
            opts |= RegexOptions.IgnoreCase;

        Regex rx = new(rxPattern, opts);

        foreach (string str in IOManager.ReadLines())
        {
            Match match = rx.Match(str);

            if (match.Success)
            {
                string path = match.Result(rxPath);

                try
                {
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        string relPath = Path.GetRelativePath(m_path, path);
                        IOManager.WriteLine(PrependInput ? $"{str}{m_sep}{relPath}" : relPath);
                    }
                }
                catch (Exception ex)
                {
                    IOManager.LogError($"{AppName}: {ex.Message}");
                }
            }
        }
    }
}
