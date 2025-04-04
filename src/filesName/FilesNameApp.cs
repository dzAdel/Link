using Link.Exceptions;
using System.Text.RegularExpressions;



namespace Link;

sealed class FilesNameApp
{
    public bool PrependInput { get; set; }
    public bool IgnoreExtention { get; set; }
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
                string hlp = "For every file in the input sequence, returns the file name and extension. " +
                    "The path need not exist." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t\t{Usage}" +
                    "\n--rx:\t\tUse regular expression to extract, from the input line, the target file." +
                    "\n\t\t‘ptrn’ represents a regular expression pattern to match against each input line." +
                    "\n\t\tThe input lines for which ‘ptrn’ does not match are ignored." +
                    "\n\t\t‘path’ is a transform pattern to apply to each input line to extract the target file." +
                    "\n\t\tIt can consist of any combination of literal text and substitutions based on 'ptrn', " +
                    "\n\t\tsuch as capturing group that is identified by a number or a name." +
                    "\n--cs:\t\tSpecifies case-sensitive matching." +
                    "\n\t\tIt's an error to specify this option without the '--rx' switch." +
                    "\n--noext:\tReturns the file name without the extension." +
                    "\n--pi:\t\tFor each line in the output sequence, prepend its input line, using 'sep' as separator." +
                    "\n\t\tIf 'sep' is missing, a colon is used." +
                    "\n--h:\t\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            string? rxPattern = null;
            string? rxPath = null;
            FilesNameApp app = new();

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

                    case "noext":
                        if (values.Count > 0)
                            throw new BadUsageException();

                        app.IgnoreExtention = true;
                        break;

                    case "cs":
                        if (values.Count > 0)
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

    static string Usage => $"{AppName} [--rx ptrn path [--cs]][--noext][--pi [sep]][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(FilesNameApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        foreach (string path in IOManager.ReadLines())
            try
            {
                string? name = IgnoreExtention ? Path.GetFileNameWithoutExtension(path) : Path.GetFileName(path);

                if (!string.IsNullOrWhiteSpace(name))
                    IOManager.WriteLine(PrependInput ? $"{path}{m_sep}{name}" : name);
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
                    string path = m.Result(rxPath);
                    string? name = IgnoreExtention ? Path.GetFileNameWithoutExtension(path) : Path.GetFileName(path);


                    if (!string.IsNullOrWhiteSpace(name))
                        IOManager.WriteLine(PrependInput ? $"{str}{m_sep}{name}" : name);
                }
            }
            catch (Exception ex)
            {
                IOManager.LogError($"{AppName}: {ex.Message}");
            }
        }

    }
}
