using Link.Exceptions;
using System.Text.RegularExpressions;


namespace Link;

sealed class FilesSizeApp
{
    public bool PrependInput { get; set; }
    public bool EnableCasing { get; set; }
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

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Returns the size of each file in the input sequence." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\n--rx:\tUse regular expression to extract, from the input line, the target file." +
                    "\n\t‘ptrn’ represents a regular expression pattern to match against each input line." +
                    "\n\tThe input lines for which ‘ptrn’ does not match are ignored." +
                    "\n\t‘file’ is a transform pattern to apply to each input line to extract the target  file." +
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
            string? rxFile = null;
            FilesSizeApp app = new();


            foreach (string opt in argParser.Options.Keys)
            {
                switch (opt)
                {
                    case "rx":
                        if (argParser.Options[opt].Count != 2)
                            throw new BadUsageException();

                        rxPattern = argParser.Options[opt][0];
                        rxFile = argParser.Options[opt][1];
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
                assert(rxFile != null);
                app.Run(rxPattern, rxFile);
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

    static string Usage => $"{AppName} [--rx ptrn file [--cs]] [--pi [sep]][--fmt][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(FilesSizeApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        foreach (string file in IOManager.ReadLines())
        {
            long sz;

            try
            {
                FileInfo fi = new(file);

                if (!fi.Exists)
                    continue;

                sz = fi.Length;

                string szStr = FormatResult ? Formater.FormatSize(sz) : sz.ToString();
                IOManager.WriteLine(PrependInput ? $"{file}{m_sep}{szStr}" : szStr);
            }
            catch (Exception ex)
            {
                IOManager.LogError($"{AppName} {ex.Message}");
            }
        }
    }

    void Run(string rxPattern, string rxFile)
    {
        require(rxFile != null);
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
                FileInfo fi = new(match.Result(rxFile));

                long sz;

                try
                {
                    if (!fi.Exists)
                        continue;

                    sz = fi.Length;

                    string szStr = FormatResult ? Formater.FormatSize(sz) : sz.ToString();
                    IOManager.WriteLine(PrependInput ? $"{fi}{m_sep}{szStr}" : szStr);
                }
                catch (Exception ex)
                {
                    IOManager.LogError($"{AppName}: {ex.Message}");
                }
            }
        }
    }
}
