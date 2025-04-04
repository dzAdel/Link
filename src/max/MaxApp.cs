using Link.Exceptions;
using System.Text.RegularExpressions;



namespace Link;

sealed class MaxApp
{
    public bool CaseSensitive { get; set; }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Returns the maximum entry in the input sequence." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\n--rx:\tUse regular expression to extract, from the input line, the target string." +
                    "\n\t'ptrn' represents a regular expression pattern to be matched." +
                    "\n\tThe input lines for which ‘ptrn’ does not match are ignored." +
                    "\n\t'key' represents the key used to compare input lines by." +
                    "\n\tIt can consist of any combination of literal text and substitutions, " +
                    "\n\tsuch as capturing group that is identified by a number or a name." +
                    "\n\tthat is identified by a number or a name." +
                    "\n--cs:\tSpecifies case-sensitive comparison and/or pattern matching." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count != 0)
                throw new BadUsageException();

            string? rxPattern = null;
            string? rxKey = null;
            MaxApp app = new();

            foreach (string opt in argParser.Options.Keys)
            {
                switch (opt)
                {
                    case "rx":
                        if (argParser.Options["rx"].Count != 2)
                            throw new BadUsageException();

                        rxPattern = argParser.Options["rx"][0];
                        rxKey = argParser.Options["rx"][1];
                        break;

                    case "cs":
                        if (argParser.Options["cs"].Count != 0)
                            throw new BadUsageException();

                        app.CaseSensitive = true;
                        break;

                    default:
                        throw new BadArgException($"--{opt}");
                }
            }

            if (rxPattern != null)
            {
                assert(rxKey != null);
                app.Run(rxPattern, rxKey);
            }
            else
                app.Run();

            return 0;
        }
        catch (Exception ex)
        {
            IOManager.LogError($"{AppName}: {ex.Message}");
        }

        return -1;
    }

    //private:
    static string Usage => $"{AppName} [--rx ptrn key][--cs][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(MaxApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        StringComparer comparer = CaseSensitive ? StringComparer.CurrentCulture :
            StringComparer.CurrentCultureIgnoreCase;

        string? max = IOManager.ReadLines().Max(comparer);

        if (max != null)
            IOManager.WriteLine(max);
    }

    void Run(string rxPattern, string rxKey)
    {
        require(rxPattern != null);
        require(rxKey != null);

        using IEnumerator<string> enumerator = IOManager.ReadLines().GetEnumerator();

        if (!enumerator.MoveNext())
            return;

        RegexOptions rxOpts = CaseSensitive ? RegexOptions.Compiled : RegexOptions.IgnoreCase | RegexOptions.Compiled;
        Regex rx = new(rxPattern, rxOpts);
        Match match;

        while (!(match = rx.Match(enumerator.Current)).Success)
            if (!enumerator.MoveNext())
                return;

        string max = enumerator.Current;
        string maxKey = match.Result(rxKey);
        StringComparer comparer = CaseSensitive ? StringComparer.CurrentCulture : StringComparer.CurrentCultureIgnoreCase;

        while (enumerator.MoveNext())
        {
            match = rx.Match(enumerator.Current);

            if (!match.Success)
                continue;

            string key = match.Result(rxKey);

            if (comparer.Compare(maxKey, key) < 0)
            {
                maxKey = key;
                max = enumerator.Current;
            }
        }

        IOManager.WriteLine(max);
    }
}
