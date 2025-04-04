using Link.Exceptions;
using System.Text.RegularExpressions;


namespace Link;

sealed class ProjectApp
{
    public ProjectApp(string rxPattern, string output)
    {
        require(rxPattern != null);
        require(output != null);

        m_rxPattern = rxPattern;
        m_rxOutput = output;
    }

    public bool EnableCasing { get; set; }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Projects every input line into a new form." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +

                    "\nptrn:\tRepresents a regular expression pattern to match." +
                    "\n\tThe input strings for which ‘ptrn’ does not match are ignored." +
                    "\noutput:\tA transform pattern to apply to each input string." +
                    "\n\tIt can consist of any combination of literal text and substitutions," +
                    "\n\tsuch as capturing group that is identified by a number or a name." +
                    "\n--cs:\tSpecifies case-sensitive matching." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count != 2)
                throw new BadUsageException();

            ProjectApp app = new(argParser.Parameters[0], argParser.Parameters[1]);

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
    readonly string m_rxOutput;

    static string Usage => $"{AppName} ptrn output [--cs][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(ProjectApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        RegexOptions opt = EnableCasing ? RegexOptions.CultureInvariant | RegexOptions.Compiled :
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant;

        Regex rx = new(m_rxPattern, opt);

        foreach (string s in IOManager.ReadLines())
        {
            Match m = rx.Match(s);

            if (m.Success)
                IOManager.WriteLine(m.Result(m_rxOutput));
        }
    }
}
