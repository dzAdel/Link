using Link.Exceptions;

namespace Link;

sealed class SerializeApp
{
    public SerializeApp(string sep)
    {
        require(sep != null);

        m_sep = sep;
        m_step = 1;
    }

    public int Start { get; set; }

    public int Step
    {
        get => m_step;
        set
        {
            require(value != 0);

            m_step = value;
        }
    }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Inserts incremental numeric series of digits at the beginning of each line in the input sequence." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\nsep:\tThe string to use as a separator." +
                    "\n\tIf ‘sep’ is missing, a colon is used as separator." +
                    "\n--init:\tSpecifies the starting number." +
                    "\n\tThe default is 0." +
                    "\n--stp:\tSpecifies an optional increment." +
                    "\n\tThe default is 1." +
                    "\n\t'step', the increment, can be any non-zero integer." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count > 1)
                throw new BadUsageException();

            SerializeApp app = argParser.Parameters.Count == 1 ? new(argParser.Parameters[0]) : new(":");

            foreach (string opt in argParser.Options.Keys)
            {
                switch (opt)
                {
                    case "init":
                        if (argParser.Options[opt].Count != 1 ||
                                !int.TryParse(argParser.Options[opt][0], out int start))
                            throw new BadUsageException();

                        app.Start = start;
                        break;

                    case "stp":
                        if (argParser.Options[opt].Count != 1 ||
                                !int.TryParse(argParser.Options[opt][0], out int step) ||
                                step == 0)
                            throw new BadUsageException();

                        app.Step = step;
                        break;

                    default:
                        throw new BadArgException($"--{opt}");
                }
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
    readonly string m_sep;
    int m_step;

    static string Usage => $"{AppName} [sep] [--init start][--stp step][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(SerializeApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        int nber = Start;

        foreach (string str in IOManager.ReadLines())
        {
            IOManager.WriteLine($"{nber}{m_sep}{str}");
            nber += Step;
        }
    }
}
