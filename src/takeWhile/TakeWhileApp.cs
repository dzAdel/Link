using Link.Exceptions;
using System.Text.RegularExpressions;



namespace Link;

sealed class TakeWhileApp
{
    public TakeWhileApp(string rxPattern)
    {
        require(rxPattern != null);

        m_rxPattern = rxPattern;
    }

    public bool EnableCasing { get; set; }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Returns lines from the input sequence as long as a specified pattern is matched, " +
                    "and then skips the remaining ones." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\nptrn:\tThe regular expression pattern to match." +
                    "\n--cs:\tSpecifies case-sensitive matching." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }


            if (argParser.Parameters.Count != 1)
                throw new BadUsageException();

            TakeWhileApp app = new(argParser.Parameters[0]);

            foreach (string opt in argParser.Options.Keys)
                switch (opt)
                {
                    case "cs":
                        if (argParser.Options[opt].Count != 0)
                            throw new BadUsageException();

                        app.EnableCasing = true;
                        break;

                    default:
                        throw new BadArgException($"--{opt}");
                }


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
    readonly string m_rxPattern;

    static string Usage => $"{AppName} ptrn [--cs][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(TakeWhileApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        RegexOptions opts = EnableCasing ? RegexOptions.Compiled | RegexOptions.CultureInvariant :
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

        Regex rx = new(m_rxPattern, opts);
        using IEnumerator<string> it = IOManager.ReadLines().GetEnumerator();

        while (it.MoveNext() && rx.IsMatch(it.Current))
            IOManager.WriteLine(it.Current);

        while (it.MoveNext())
            ; //nop
    }
}
