using Link.Exceptions;


namespace Link;

sealed class SplitApp
{
    public bool RemoveEmptyEntries { get; set; }
    public bool TrimEntries { get; set; }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Splits each input line into substrings that are based on the provided separator. " +
                    "Each substring is retuned as a line." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t\t{Usage}" +
                    "\nsep:\t\tA string that delimits the substrings in the input lines." +
                    "\n\t\tIf ‘sep’ is absent, white-space characters are assumed to be the delimiters." +
                    "\n--nompty:\tOmit empty substrings from the result." +
                    "\n--trim:\t\tTrim white-space characters from each substring in the result." +
                    "\n--h:\t\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count > 1)
                throw new BadUsageException();

            string? sep = argParser.Parameters.Count == 1 && argParser.Parameters[0].Length > 0 ? argParser.Parameters[0] : null;
            SplitApp app = new();

            foreach (string opt in argParser.Options.Keys)
            {
                if (argParser.Options[opt].Count != 0)
                    throw new BadUsageException();

                switch (opt)
                {
                    case "nompty":
                        app.RemoveEmptyEntries = true;
                        break;

                    case "trim":
                        app.TrimEntries = true;
                        break;

                    default:
                        throw new BadArgException($"--{opt}");
                }
            }

            if (sep != null)
                app.Run(sep);
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
    static string Usage => $"{AppName} [sep][--nompty][--trim][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(SplitApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        StringSplitOptions opts = RemoveEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;

        if (TrimEntries)
            opts |= StringSplitOptions.TrimEntries;

        foreach (string str in IOManager.ReadLines())
            IOManager.WriteLines(str.Split(default(char[]), opts));
    }

    void Run(string sep)
    {
        require(!string.IsNullOrEmpty(sep));

        StringSplitOptions opts = RemoveEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;

        if (TrimEntries)
            opts |= StringSplitOptions.TrimEntries;

        foreach (string str in IOManager.ReadLines())
            IOManager.WriteLines(str.Split(sep, opts));
    }
}
