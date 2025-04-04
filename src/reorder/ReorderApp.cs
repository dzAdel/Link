using Link.Exceptions;
using System.Text.RegularExpressions;


namespace Link;

sealed class ReorderApp
{
    public bool EnableCasing { get; set; }
    public bool SortDescending { get; set; }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Sorts the input sequence in ascending order. This command performs a stable sort." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    $"\n--rx:\tUse regular expression to distinguish input lines." +
                    $"\n\t'ptrn' represents a regular expression pattern to match." +
                    $"\n\tThe input lines for which ‘ptrn’ does not match are ignored." +
                    $"\n\t'key' is a transform pattern to apply to each input line to extract" +
                    $"\n\tthe key used to distinguish input lines." +
                    $"\n\tIt can consist of any combination of literal text and substitutions, " +
                    $"\n\tsuch as capturing group that is identified by a number or a name." +
                    "\n--cs:\tSpecifies case-sensitive comparison and/or pattern matching." +
                    "\n--dsc:\tSorts the input in descending order." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count != 0)
                throw new BadUsageException();

            ReorderApp app = new();
            string? rxPtrn = null;
            string? rxKey = null;

            foreach (string opt in argParser.Options.Keys)
                switch (opt)
                {
                    case "cs":
                        if (argParser.Options[opt].Count != 0)
                            throw new BadUsageException();

                        app.EnableCasing = true;
                        break;

                    case "dsc":
                        if (argParser.Options[opt].Count != 0)
                            throw new BadUsageException();

                        app.SortDescending = true;
                        break;

                    case "rx":
                        if (argParser.Options[opt].Count != 2)
                            throw new BadUsageException();

                        rxPtrn = argParser.Options[opt][0];
                        rxKey = argParser.Options[opt][1];
                        break;

                    default:
                        throw new BadArgException($"--{opt}");
                }



            if (rxPtrn != null)
            {
                assert(rxKey != null);
                app.Run(rxPtrn, rxKey);
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
    static string Usage => $"{AppName} [--rx ptrn key][--cs][--dsc][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(ReorderApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        StringComparer comparer = EnableCasing ? StringComparer.CurrentCulture : StringComparer.CurrentCultureIgnoreCase;
        IEnumerable<string> src = IOManager.ReadLines();

        IOManager.WriteLines(SortDescending ? src.OrderByDescending(e => e, comparer) : src.OrderBy(e => e, comparer));
    }

    void Run(string rxPattern, string rxKey)
    {
        require(rxPattern != null);
        require(rxKey != null);

        RegexOptions opts = RegexOptions.Compiled;

        if (!EnableCasing)
            opts |= RegexOptions.IgnoreCase;

        Regex rx = new(rxPattern, opts);
        IEnumerable<string> src = IOManager.ReadLines();

        var qry = from line in src
                  let m = rx.Match(line)
                  where m.Success
                  select (Line: line, Key: m.Result(rxKey));

        StringComparer comparer = EnableCasing ? StringComparer.CurrentCulture : StringComparer.CurrentCultureIgnoreCase;
        IOManager.WriteLines(SortDescending ? qry.OrderByDescending(p => p.Key, comparer).Select(p => p.Line) :
            qry.OrderBy(p => p.Key, comparer).Select(p => p.Line));
    }
}
