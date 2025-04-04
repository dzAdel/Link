using Link.Exceptions;
using System.Text.RegularExpressions;


namespace Link;

sealed class CreateDirsApp
{
    public bool EnableCasing { get; set; }
    public bool ReturnInput { get; set; }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Creates all directories and sub-directories specified in the input sequence unless they already " +
                    "exist." +
                    "The command returns the created directories." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\n--rx:\tUses a regular expression to extract the target directory from the input lines." +
                    "\n\t‘ptrn’ represents a regular expression pattern to match." +
                    "\n\tInput lines for which ‘ptrn’ is not matched are ignored." +
                    "\n\t‘dir’ is the directory to be created." +
                    "\n\tIt can consist of any combination of literal text and substitutions based on 'ptrn'," +
                    "\n\tsuch as capturing group that is identified by a number or a name." +
                    "\n\tBy default, the input line is considered the target directory." +
                    "\n--cs:\tSpecifies case-sensitive matching." +
                    "\n\tIt's an error to specify this option without the '--rx' switch." +
                    "\n--ri:\tFor each created directory, returns its input line instead of the destination directory." +
                    "\n\tIt's an error to specify this option without the '--rx' switch." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count > 0)
                throw new BadUsageException();

            string? rxPtrn = null;
            string? rxDir = null;
            CreateDirsApp app = new();

            foreach (string opt in argParser.Options.Keys)
            {
                IReadOnlyList<string> values = argParser.Options[opt];

                switch (opt)
                {
                    case "rx":
                        if (values.Count != 2)
                            throw new BadUsageException();

                        rxPtrn = argParser.Options[opt][0];
                        rxDir = argParser.Options[opt][1];
                        break;

                    case "ri":
                        if (values.Count > 0)
                            throw new BadUsageException();

                        app.ReturnInput = true;
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

            if (rxPtrn != null)
            {
                assert(rxDir != null);
                app.Run(rxPtrn, rxDir);
            }
            else
            {
                if (app.ReturnInput || app.EnableCasing)
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
    static string Usage => $"{AppName} [--rx ptrn dir [--cs][--ri]] [--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(CreateDirsApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    static void Run()
    {
        foreach (string dir in IOManager.ReadLines())
            try
            {
                DirectoryInfo dirInfo = new(dir);

                if (dirInfo.Exists)
                    continue;

                dirInfo.Create();

                IOManager.WriteLine(dir);
            }
            catch (Exception ex)
            {
                IOManager.LogError(ex.Message);
            }
    }

    void Run(string rxPtrn, string rxDir)
    {
        RegexOptions rxOpt = RegexOptions.Compiled | RegexOptions.CultureInvariant;

        if (!EnableCasing)
            rxOpt |= RegexOptions.IgnoreCase;

        Regex rx = new(rxPtrn, rxOpt);

        foreach (string str in IOManager.ReadLines())
            try
            {
                Match match = rx.Match(str);

                if (match.Success)
                {
                    string dir = match.Result(rxDir);
                    DirectoryInfo dirInfo = new(dir);

                    if (dirInfo.Exists)
                        continue;

                    dirInfo.Create();
                    IOManager.WriteLine(ReturnInput ? str : dir);
                }
            }
            catch (Exception ex)
            {
                IOManager.LogError(ex.Message);
            }
    }
}
