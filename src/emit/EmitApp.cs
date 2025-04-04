using Link.Exceptions;
using System.Text.RegularExpressions;

namespace Link;

sealed class EmitApp
{
    public bool EnableCasing { get; set; }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Returns the contents of each file in the input sequence." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\n--rx:\tUse regular expression to extract, from each input line, the target file." +
                    "\n\t‘ptrn’ represents a regular expression pattern to match against each input line." +
                    "\n\tThe input lines for which ‘ptrn’ does not match are ignored." +
                    "\n\t‘file’ is the file whose contents are to be output. " +
                    "\n\tIt can consist of any combination of literal text and substitutions based on 'ptrn', " +
                    "\n\tsuch as capturing group that is identified by a number or a name." +
                    "\n--cs:\tSpecifies case-sensitive matching." +
                    "\n\tIt's an error to specify this option without the '--rx' switch." +
                    "\n--h:\tDisplays this help and exits.";


                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count > 0)
                throw new BadUsageException();

            string? rxPattern = null;
            string? rxFile = null;
            EmitApp app = new();

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

                    case "cs":
                        if (argParser.Options[opt].Count != 0)
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

                Run();
            }
            else
            {
                assert(rxFile != null);
                app.Run(rxPattern, rxFile);
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
    static string Usage => $"{AppName} [--rx ptrn file [--cs]][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(EmitApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    static void Run()
    {
        foreach (string fileName in IOManager.ReadLines())
            if (File.Exists(fileName))
                try
                {
                    using StreamReader reader = File.OpenText(fileName);
                    string? line;

                    while ((line = reader.ReadLine()) != null)
                        IOManager.WriteLine(line);
                }
                catch (Exception ex)
                {
                    IOManager.LogError($"{AppName}: {ex.Message}");
                }
    }

    void Run(string rxPattern, string rxFile)
    {
        require(rxFile != null);
        require(rxPattern != null);

        RegexOptions opts = RegexOptions.Compiled | RegexOptions.CultureInvariant;

        if (!EnableCasing)
            opts |= RegexOptions.IgnoreCase;

        Regex rx = new(rxPattern, opts);

        foreach (string str in IOManager.ReadLines())
        {
            Match match = rx.Match(str);

            if (match.Success)
            {
                string file = match.Result(rxFile);

                if (File.Exists(file))
                    try
                    {
                        using StreamReader reader = File.OpenText(file);
                        string? line;

                        while ((line = reader.ReadLine()) != null)
                            IOManager.WriteLine(line);
                    }
                    catch (Exception ex)
                    {
                        IOManager.LogError($"{AppName}: {ex.Message}");
                    }
            }
        }
    }
}
