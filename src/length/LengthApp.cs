using Link.Exceptions;
using System.Globalization;
using System.Text.RegularExpressions;


namespace Link;

sealed class LengthApp
{
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
                string hlp = "Returns the number of characters for each line in the input sequence." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\n--rx:\tUse regular expression to extract, from the input line, the target string." +
                    "\n\t‘ptrn’ represents a regular expression pattern to match against each input line." +
                    "\n\tThe input lines for which ‘ptrn’ does not match are ignored." +
                    "\n\t‘str’ is a transform pattern to apply to each input line to extract the target string." +
                    "\n\tIt can consist of any combination of literal text and substitutions based on 'ptrn'," +
                    "\n\tsuch as capturing group that is identified by a number or a name." +
                    "\n--cs:\tSpecifies case-sensitive matching." +
                    "\n\tIt's an error to specify this option without the '--rx' switch." +
                    "\n--pi:\tFor each line in the output sequence, prepend its input line, using 'sep' as separator." +
                    "\n\tIf 'sep' is missing, a colon is used." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count > 0)
                throw new BadUsageException();

            string? rxPattern = null;
            string? rxStr = null;
            LengthApp app = new();

            foreach (string opt in argParser.Options.Keys)
            {
                IReadOnlyList<string> values = argParser.Options[opt];

                switch (opt)
                {
                    case "rx":
                        if (values.Count != 2)
                            throw new BadUsageException();

                        rxPattern = values[0];
                        rxStr = values[1];
                        break;

                    case "pi":
                        if (values.Count > 1)
                            throw new BadUsageException();

                        app.PrependInput = true;

                        if (values.Count == 1)
                            app.Separator = values[0];
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

            if (rxPattern == null)
            {
                if (app.EnableCasing)
                    throw new BadUsageException();

                app.Run();
            }
            else
            {
                assert(rxStr != null);
                app.Run(rxPattern, rxStr);
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

    static string Usage => $"{AppName} [--rx ptrn str [--cs]][--pi [sep]][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(LengthApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        StringInfo si = new();

        foreach (string str in IOManager.ReadLines())
        {
            si.String = str;
            int len = si.LengthInTextElements;
            IOManager.WriteLine(PrependInput ? $"{str}{m_sep}{len}" : len.ToString());
        }
    }

    void Run(string rxPattern, string rxStr)
    {
        require(rxPattern != null);
        require(rxStr != null);

        RegexOptions opt = RegexOptions.CultureInvariant | RegexOptions.Compiled;

        if (!EnableCasing)
            opt |= RegexOptions.IgnoreCase;


        Regex rx = new(rxPattern, opt);
        StringInfo si = new();

        foreach (string s in IOManager.ReadLines())
            try
            {
                Match m = rx.Match(s);

                if (m.Success)
                {
                    si.String = m.Result(rxStr);
                    int len = si.LengthInTextElements;
                    IOManager.WriteLine(PrependInput ? $"{s}{m_sep}{len}" : len.ToString());
                }
            }
            catch (Exception ex)
            {
                IOManager.LogError($"{AppName}: {ex.Message}");
            }
    }
}
