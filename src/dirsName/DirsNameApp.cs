using Link.Exceptions;
using System.Text.RegularExpressions;


namespace Link;

static class DirsNameApp
{
    public static bool PrependInput { get; set; }
    public static bool EnableCasing { get; set; }

    public static string Separator
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
                string hlp = "Returns the directory information for each path in the input sequence. " +
                    "In most cases, the string returned consists of all characters in the path up to, " +
                    "but not including, the last directory separator. " +
                    "The file or directory specified by the input path is not required to exist." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\n--rx:\tUse regular expression to extract, from the input line, the target path." +
                    "\n\t‘ptrn’ represents a regular expression pattern to match against each input line." +
                    "\n\tThe input lines for which ‘ptrn’ does not match are ignored." +
                    "\n\t‘path’ is a transform pattern to apply to each input line to extract the target path." +
                    "\n\tIt can consist of any combination of literal text and substitutions based on 'ptrn'," +
                    "\n\tsuch as capturing group that is identified by a number or a name." +
                    "\n--cs:\tSpecifies case-sensitive matching." +
                    "\n\tIt's an error to specify this option without the '--rx' switch" +
                    "\n--pi:\tFor each line in the output sequence, prepend its input line, using 'sep' as separator." +
                    "\n\tIf 'sep' is missing, a colon is used." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count != 0)
                throw new BadUsageException();

            string? rxPattern = null;
            string? rxPath = null;

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

                        PrependInput = true;

                        if (values.Count == 1)
                            Separator = values[0];

                        break;

                    case "cs":
                        if (values.Count != 0)
                            throw new BadUsageException();

                        EnableCasing = true;
                        break;

                    default:
                        throw new BadArgException($"--{opt}");
                }
            }

            if (rxPattern != null)
            {
                assert(rxPath != null);
                Run(rxPattern, rxPath);
            }
            else
            {
                if (EnableCasing)
                    throw new BadUsageException();

                Run();
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
    static string m_sep = ":";

    static string Usage => $"{AppName} [--rx ptrn path][--pi [sep]][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(DirsNameApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    static void Run()
    {
        foreach (string path in IOManager.ReadLines())
            try
            {
                string? dir = Path.GetDirectoryName(path);

                if (!string.IsNullOrWhiteSpace(dir))
                    IOManager.WriteLine(PrependInput ? $"{path}{m_sep}{dir}" : dir);
            }
            catch (Exception ex)
            {
                IOManager.LogError(ex.Message);
            }
    }

    static void Run(string rxPattern, string rxPath)
    {
        RegexOptions opts = RegexOptions.Compiled | RegexOptions.CultureInvariant;

        if (!EnableCasing)
            opts |= RegexOptions.IgnoreCase;

        Regex rx = new(rxPattern, opts);

        foreach (string str in IOManager.ReadLines())
        {
            try
            {
                Match m = rx.Match(str);

                if (m.Success)
                {
                    string? dir = Path.GetDirectoryName(m.Result(rxPath));

                    if (!string.IsNullOrWhiteSpace(dir))
                        IOManager.WriteLine(PrependInput ? $"{str}{m_sep}{dir}" : dir);
                }
            }
            catch (Exception ex)
            {
                IOManager.LogError(ex.Message);
            }
        }
    }
}
