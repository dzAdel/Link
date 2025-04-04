using Link.Exceptions;
using System.Text.RegularExpressions;

namespace Link;

sealed class DumpApp
{
    public DumpApp(string file)
    {
        require(!string.IsNullOrWhiteSpace(file));

        m_filePath = file;
    }

    public bool Overwrite { get; set; }
    public bool EnableCasing { get; set; }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Writes its input sequence to the standard output and to a specifiled file." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\nfile:\tFile to which input is written." +
                    "\n--rx:\tUses regular expression to filter written lines to the specified file." +
                    "\n\t‘ptrn’ represents a regular expression pattern to match." +
                    "\n\tThe input lines for which ‘ptrn’ does not match are not written to the file." +
                    "\n--cs:\tSpecifies case-sensitive matching." +
                    "\n\tIt's an error to specify this option without the '--rx' switch." +
                    "\n--ovw:\tOverwites the output to each existing file, rather than appending it." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }


            if (argParser.Parameters.Count != 1 || string.IsNullOrWhiteSpace(argParser.Parameters[0]))
                throw new BadUsageException();

            DumpApp app = new(argParser.Parameters[0]);
            string? rxPattern = null;

            foreach (string opt in argParser.Options.Keys)
            {
                switch (opt)
                {
                    case "rx":
                        if (argParser.Options[opt].Count != 1)
                            throw new BadUsageException();

                        rxPattern = argParser.Options[opt][0];
                        break;

                    case "ovw":
                        if (argParser.Options[opt].Count != 0)
                            throw new BadUsageException();

                        app.Overwrite = true;
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

            if (rxPattern != null)
                app.Run(rxPattern);
            else
            {
                if (app.EnableCasing)
                    throw new BadUsageException();

                app.Run();
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
    readonly string m_filePath;

    static string Usage => $"{AppName} file [--rx ptrn [--cs]][--ovw][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(DumpApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        FileStreamOptions opts = new()
        {
            Options = FileOptions.SequentialScan,
            Mode = Overwrite ? FileMode.Create : FileMode.Append,
            Access = FileAccess.Write
        };

        using StreamWriter writer = new(File.Open(m_filePath, opts));

        foreach (string line in IOManager.ReadLines())
        {
            IOManager.WriteLine(line);
            writer.WriteLine(line);
        }
    }

    public void Run(string rxPattern)
    {
        require(rxPattern != null);

        RegexOptions opt = RegexOptions.Compiled | RegexOptions.CultureInvariant;

        if (!EnableCasing)
            opt |= RegexOptions.IgnoreCase;

        Regex rx = new(rxPattern, opt);

        FileStreamOptions opts = new()
        {
            Options = FileOptions.SequentialScan,
            Mode = Overwrite ? FileMode.Create : FileMode.Append,
            Access = FileAccess.Write
        };

        using StreamWriter writer = new(File.Open(m_filePath, opts));

        foreach (string line in IOManager.ReadLines())
        {
            IOManager.WriteLine(line);

            if (rx.IsMatch(line))
                writer.WriteLine(line);
        }
    }
}
