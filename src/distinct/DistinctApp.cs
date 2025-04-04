using Link.Exceptions;
using System.Text.RegularExpressions;



namespace Link;

sealed class DistinctApp
{
    public bool EnableCasing { get; set; }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Remove duplicate lines from input sequence." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\n--rx:\tUse regular expression to distinguish input lines." +
                    "\n\t‘ptrn’ represents a regular expression pattern to match." +
                    "\n\t‘key’ is the key used to distinguish input lines." +
                    "\n\tIt can consist of any combination of literal text and substitutions based on 'ptrn', " +
                    "\n\tsuch as capturing group that is identified by a number or a name." +
                    "\n\tThe input lines for which ‘ptrn’ does not match are ignored." +
                    "\n--cs:\tSpecifies case-sensitive comparison and/or pattern matching." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count > 0)
                throw new BadUsageException();

            string? rxPattern = null;
            string? rxKey = null;
            DistinctApp app = new();

            foreach (string opt in argParser.Options.Keys)
                switch (opt)
                {
                    case "rx":
                        if (argParser.Options[opt].Count != 2)
                            throw new BadUsageException();

                        rxPattern = argParser.Options[opt][0];
                        rxKey = argParser.Options[opt][1];
                        break;

                    case "cs":
                        if (argParser.Options[opt].Count > 0)
                            throw new BadUsageException();

                        app.EnableCasing = true;
                        break;

                    default:
                        throw new BadArgException($"--{opt}");
                }


            if (rxPattern == null)
                app.Run();
            else
            {
                assert(rxKey != null);
                app.Run(rxPattern, rxKey);
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
    static string Usage => $"{AppName} [--rx ptrn key][--cs][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(DistinctApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        StringComparer comp = EnableCasing ? StringComparer.InvariantCulture : StringComparer.InvariantCultureIgnoreCase;
        IOManager.WriteLines(IOManager.ReadLines().Distinct(comp));
    }

    void Run(string rxPattern, string rxKey)
    {
        require(rxPattern != null);
        require(rxKey != null);

        StringComparer comp = EnableCasing ? StringComparer.InvariantCulture : StringComparer.InvariantCultureIgnoreCase;
        HashSet<string> set = new(comp);

        RegexOptions options = EnableCasing ? RegexOptions.Compiled | RegexOptions.CultureInvariant :
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled;

        Regex rx = new(rxPattern, options);

        foreach (string str in IOManager.ReadLines())
        {
            Match m = rx.Match(str);

            if (m.Success && set.Add(m.Result(rxKey)))
                IOManager.WriteLine(str);
        }
    }
}
