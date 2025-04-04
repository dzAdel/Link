using Link.Exceptions;
using System.Text.RegularExpressions;

namespace Link;

sealed class DelDirsApp
{
    public DelDirsApp(string rxPtrn, string rxDir)
    {
        require(rxPtrn != null);
        require(rxDir != null);

        m_rxDir = rxDir;
        m_rxPtrn = rxPtrn;
    }

    public DelDirsApp()
    { }

    public bool EnableCasing { get; set; }
    public bool ReturnInput { get; set; }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Deletes directories specified in the input sequence. Returns the sequences of deleted directories. " +
                    "Non-existent directories are ignored." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\n--rx:\tUses an optional regular expression to extract, from the input line, the target directory." +
                    "\n\t'ptrn' represents a regular expression pattern to match against each input line." +
                    "\n\tThe input line for which ‘ptrn’ is not matched are ignored. 'dir' the directory to delete," +
                    "\n\tcan consist of any combination of literal text and substitutions based on 'ptrn'," +
                    "\n\tsuch as capturing group that is identified by a number or a name." +
                    "\n\tAny file and sub-directory of the target directory will be also deleted." +
                    "\n--cs:\tSpecifies case-sensitive matching." +
                    "\n\tIt's an error to specify this option without the '--rx' switch." +
                    "\n--ri:\tFor each deleted directory, returns its input line instead of the target directory." +
                    "\n\tIt's an error to specify this option without the '--rx' switch." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count > 0)
                throw new BadUsageException();

            string? rxPtrn = null, rxFile = null;
            bool ec = false, ri = false;

            foreach (string opt in argParser.Options.Keys)
            {
                IReadOnlyList<string> values = argParser.Options[opt];

                switch (opt)
                {
                    case "rx":
                        if (values.Count != 2)
                            throw new BadUsageException();

                        rxPtrn = values[0];
                        rxFile = values[1];
                        break;

                    case "cs":
                        if (values.Count != 0)
                            throw new BadUsageException();

                        ec = true;
                        break;

                    case "ri":
                        if (values.Count != 0)
                            throw new BadUsageException();

                        ri = true;
                        break;

                    default:
                        throw new BadArgException($"--{opt}");
                }
            }


            if (rxPtrn != null)
            {
                assert(rxFile != null);
                var app = new DelDirsApp(rxPtrn, rxFile)
                {
                    EnableCasing = ec,
                    ReturnInput = ri,
                };

                app.RegexRun();
            }
            else
            {
                if (ec || ri)
                    throw new BadUsageException();

                Run();
                return 0;
            }
        }
        catch (Exception ex)
        {
            IOManager.LogError($"{AppName}: {ex.Message}");
        }

        return -1;
    }

    //private:
    readonly string? m_rxPtrn;
    readonly string? m_rxDir;

    static string Usage => $"{AppName} [--rx ptrn dir [--cs][--ri]] [--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(DelDirsApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    static void Run()
    {
        foreach (string dir in IOManager.ReadLines())
            try
            {
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                    IOManager.WriteLine(dir);
                }
            }
            catch (Exception ex)
            {
                IOManager.LogError(ex.Message);
            }
    }

    void RegexRun()
    {
        RegexOptions rxOpt = RegexOptions.Compiled;

        if (!EnableCasing)
            rxOpt |= RegexOptions.IgnoreCase;

        assert(m_rxPtrn != null);
        assert(m_rxDir != null);
        Regex rx = new(m_rxPtrn, rxOpt);

        foreach (string str in IOManager.ReadLines())
            try
            {
                Match m = rx.Match(str);

                if (m.Success)
                {
                    string dir = m.Result(m_rxDir);

                    if (Directory.Exists(dir))
                    {
                        Directory.Delete(dir, true);
                        IOManager.WriteLine(ReturnInput ? str : dir);
                    }
                }
            }
            catch (Exception ex)
            {
                IOManager.LogError(ex.Message);
            }
    }
}
