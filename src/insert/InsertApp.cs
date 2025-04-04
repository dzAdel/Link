using Link.Exceptions;


namespace Link;

sealed class InsertApp
{
    public InsertApp(string insStr, int ndxStart)
    {
        require(insStr != null);
        require(ndxStart >= 0);

        m_value = insStr;
        m_ndxStart = ndxStart;
    }

    public bool PrependInput { get; set; }

    public string Separator
    {
        get => m_sep;
        set
        {
            require(value != null);

            m_sep = value;
        }
    }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Returns, for each line in the input sequence, a new line in which a specified string is " +
                    "inserted at a specified index." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\nstr:\tThe string to insert." +
                    "\nndx:\tThe zero-based index position of the insertion." +
                    "\n\tAny input line for which 'ndx' is greater than its length is ignored." +
                    "\n--pi:\tFor each line in the output sequence, prepend its input line, using 'sep' as separator." +
                    "\n\tIf 'sep' is missing, a colon is used." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count != 2)
                throw new BadUsageException();

            if (!int.TryParse(argParser.Parameters[1], out int ndx) || ndx < 0)
                throw new BadUsageException();


            InsertApp app = new(argParser.Parameters[0], ndx);

            foreach (string opt in argParser.Options.Keys)
                switch (opt)
                {
                    case "pi":
                        if (argParser.Options[opt].Count > 1)
                            throw new BadUsageException();

                        app.PrependInput = true;

                        if (argParser.Options[opt].Count == 1)
                            app.Separator = argParser.Options[opt][0];

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
    readonly string m_value;
    string m_sep = ":";
    readonly int m_ndxStart;

    static string Usage => $"{AppName} str ndx [--pi [sep]][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(InsertApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        foreach (string line in IOManager.ReadLines())
            if (line.Length >= m_ndxStart)
            {
                string result = line.Insert(m_ndxStart, m_value);

                IOManager.WriteLine(PrependInput ? $"{line}{m_sep}{result}" : result);
            }
    }
}
