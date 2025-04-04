using Link.Exceptions;
using System.Text.RegularExpressions;


namespace Link;

sealed class DirsSizeApp
{
    public bool EnableCasing { get; set; }
    public bool PrependInput { get; set; }
    public bool FormatResult { get; set; }

    public string Separator
    {
        get => m_sep;
        set
        {
            require(value != null);
            m_sep = value;
        }
    }

    public void Run()
    {
        foreach (string dir in IOManager.ReadLines())
        {
            long sz;

            try
            {
                DirectoryInfo di = new(dir);

                if (!di.Exists)
                    continue;

                sz = GetSize(di);

                string szStr = FormatResult ? Formater.FormatSize(sz) : sz.ToString();
                IOManager.WriteLine(PrependInput ? $"{dir}{m_sep}{szStr}" : szStr);
            }
            catch (Exception ex)
            {
                IOManager.LogError($"{AppName} {ex.Message}");
            }
        }
    }

    public void Run(string rxPattern, string rxDir)
    {
        require(rxDir != null);
        require(rxPattern != null);

        RegexOptions options = RegexOptions.Compiled | RegexOptions.CultureInvariant;

        if (!EnableCasing)
            options |= RegexOptions.IgnoreCase;


        Regex rx = new(rxPattern, options);

        foreach (string str in IOManager.ReadLines())
        {
            Match match = rx.Match(str);

            if (match.Success)
            {

                long sz;

                try
                {
                    DirectoryInfo dir = new(match.Result(rxDir));

                    if (!dir.Exists)
                        continue;

                    sz = GetSize(dir);

                    string szStr = FormatResult ? Formater.FormatSize(sz) : sz.ToString();
                    IOManager.WriteLine(PrependInput ? $"{dir}{m_sep}{szStr}" : szStr);
                }
                catch (Exception ex)
                {
                    IOManager.LogError($"{AppName}: {ex.Message}");
                }
            }
        }
    }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Returns the size of the contents of each directory in the input sequence." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\n--rx:\tUse regular expression to extract, from the input line, the target directory." +
                    "\n\t‘ptrn’ represents a regular expression pattern to match against each input line." +
                    "\n\tThe input lines for which ‘ptrn’ does not match are ignored." +
                    "\n\t‘dir’ is a transform pattern to apply to each input line to extract the target directory." +
                    "\n\tIt can consist of any combination of literal text and substitutions based on 'ptrn'," +
                    "\n\tsuch as capturing group that is identified by a number or a name." +
                    "\n--cs:\tSpecifies case-sensitive matching." +
                    "\n\tIt's an error to specify this option without the '--rx' switch." +
                    "\n--pi:\tFor each line in the output sequence, prepend its input line, using 'sep' as separator." +
                    "\n\tIf 'sep' is missing, a colon is used." +
                    "\n--fmt:\tReturns the size in a human-readable format." +
                    "\n\tIf this switch is missing, the size is returned in bytes." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count > 0)
                throw new BadUsageException();

            string? rxPattern = null;
            string? rxDir = null;
            DirsSizeApp app = new();

            foreach (string opt in argParser.Options.Keys)
            {
                switch (opt)
                {
                    case "rx":
                        if (argParser.Options[opt].Count != 2)
                            throw new BadUsageException();

                        rxPattern = argParser.Options[opt][0];
                        rxDir = argParser.Options[opt][1];
                        break;

                    case "pi":
                        if (argParser.Options[opt].Count > 1)
                            throw new BadUsageException();

                        app.PrependInput = true;

                        if (argParser.Options[opt].Count == 1)
                            app.Separator = argParser.Options[opt][0];
                        break;

                    case "fmt":
                        if (argParser.Options[opt].Count > 0)
                            throw new BadUsageException();

                        app.FormatResult = true;
                        break;

                    case "cs":
                        if (argParser.Options[opt].Count > 0)
                            throw new BadUsageException();

                        app.EnableCasing = true;
                        break;

                    default:
                        throw new BadArgException($"--{opt}");
                }
            }


            if (rxPattern != null)
            {
                assert(rxDir != null);
                app.Run(rxPattern, rxDir);
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
    readonly Dictionary<string, long> m_sizes = new(StringComparer.InvariantCultureIgnoreCase);
    string m_sep = ":";

    static string Usage => $"{AppName} [--rx ptrn dir [--cs]] [--pi [sep]][--fmt][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(DirsSizeApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    long GetSize(DirectoryInfo dir)
    {
        if (!m_sizes.TryGetValue(dir.FullName, out long sz))
        {
            foreach (FileInfo fi in dir.EnumerateFiles())
                sz += fi.Length;

            foreach (DirectoryInfo di in dir.EnumerateDirectories())
                sz += GetSize(di);

            m_sizes[dir.FullName] = sz;
        }

        return sz;
    }
}
