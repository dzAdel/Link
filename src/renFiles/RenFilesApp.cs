using Link.Exceptions;
using System.Text.RegularExpressions;


namespace Link;

sealed class RenFilesApp
{
    public RenFilesApp(string rxPattern, string rxDest)
    {
        require(rxPattern != null);
        require(rxDest != null);

        m_rxPattern = rxPattern;
        m_rxDest = rxDest;
    }

    public bool ReturnInput { get; set; }
    public bool Overwrite { get; set; }
    public bool EnableCasing { get; set; }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Rename or move each file from the input sequence, and then returns the processed files." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +

                    "\nptrn:\tRepresents a regular expression pattern to match." +
                    "\n\tInput files for which ‘ptrn’ does not match are ignored." +
                    "\ndest:\tA transform pattern to apply to each input line" +
                    "\n\tto extract the new path or name for the input file." +
                    "\n\tIt can consist of any combination of literal text and substitutions," +
                    "\n\tsuch as capturing group that is identified by a number or a name." +
                    "\nsrc:\tA transform pattern to apply to each input line to extract the source file." +
                    "\n\tIt can consist of any combination of literal text and substitutions," +
                    "\n\tsuch as capturing group that is identified by a number or a name." +
                    "\n\tIf this parameter is missing, the input string is considered to be the source file." +
                    "\n--ri:\tReturns the processed input files." +
                    "\n\tIf this flag is omitted, the command returns the destination files." +
                    "\n--ovw:\tOverwrites the destination file if it already exists." +
                    "\n--cs:\tSpecifies case-sensitive matching." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count is < 2 or > 3)
                throw new BadUsageException();

            RenFilesApp app = new(argParser.Parameters[0], argParser.Parameters[1]);

            foreach (string opt in argParser.Options.Keys)
            {
                if (argParser.Options[opt].Count > 0)
                    throw new BadUsageException();


                switch (opt)
                {
                    case "ri":
                        app.ReturnInput = true;
                        break;

                    case "ovw":
                        app.Overwrite = true;
                        break;

                    case "cs":
                        app.EnableCasing = true;
                        break;

                    default:
                        throw new BadArgException($"--{opt}");
                }
            }

            if (argParser.Parameters.Count == 3)
                app.Run(argParser.Parameters[2]);
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
    readonly string m_rxPattern;
    readonly string m_rxDest;

    static string Usage => $"{AppName} ptrn dest [src] [--ri][--ovw][--cs][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(RenFilesApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        RegexOptions opt = RegexOptions.Compiled | RegexOptions.CultureInvariant;

        if (!EnableCasing)
            opt |= RegexOptions.IgnoreCase;


        Regex rx = new(m_rxPattern, opt);

        foreach (string srcFile in IOManager.ReadLines())
        {
            Match m = rx.Match(srcFile);

            if (m.Success)
            {
                FileInfo fi = new(srcFile);

                if (fi.Exists)
                {
                    string destFile = m.Result(m_rxDest);

                    try
                    {
                        fi.MoveTo(destFile, Overwrite);
                        IOManager.WriteLine(ReturnInput ? srcFile : destFile);
                    }
                    catch (Exception ex)
                    {
                        IOManager.LogError($"{AppName}: {ex.Message}");
                    }
                }
            }
        }
    }

    void Run(string rxSrc)
    {
        require(rxSrc != null);

        RegexOptions opt = RegexOptions.Compiled | RegexOptions.CultureInvariant;

        if (!EnableCasing)
            opt |= RegexOptions.IgnoreCase;


        Regex rx = new(m_rxPattern, opt);

        foreach (string str in IOManager.ReadLines())
        {
            Match m = rx.Match(str);

            if (m.Success)
            {
                string srcFile = m.Result(rxSrc);
                FileInfo fi = new(srcFile);

                if (fi.Exists)
                {
                    string destFile = m.Result(m_rxDest);

                    try
                    {
                        fi.MoveTo(destFile, Overwrite);
                        IOManager.WriteLine(ReturnInput ? srcFile : destFile);
                    }
                    catch (Exception ex)
                    {
                        IOManager.LogError($"{AppName}: {ex.Message}");
                    }
                }
            }
        }
    }
}
