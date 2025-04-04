using Link.Exceptions;

namespace Link;

sealed class RemoveApp
{
    public RemoveApp(string str)
    {
        require(str != null);

        m_targetStr = str;
    }

    public bool EnableCasing { get; set; }
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
                string hlp = "Returns a new string for each line in the input sequence in which " +
                    "all occurrences of a specified substring are deleted." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\nstr:\tThe substring to remove all occurrences." +
                    "\n--cs:\tSpecifies case-sensitive matching." +
                    "\n--pi:\tFor each output line, prepends its input line, using 'sep' as separator." +
                    "\n\tIf 'sep' is missing, a colon is used." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count != 1)
                throw new BadUsageException();


            RemoveApp app = new(argParser.Parameters[0]);

            foreach (string opt in argParser.Options.Keys)
            {
                switch (opt)
                {
                    case "cs":
                        if (argParser.Options[opt].Count > 0)
                            throw new BadUsageException();

                        app.EnableCasing = true;
                        break;

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
    readonly string m_targetStr;
    string m_sep = ":";

    static string Usage => $"{AppName} str [--cs][--pi [sep]][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(RemoveApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        if (m_targetStr.Length == 0)
        {
            IOManager.WriteLines(IOManager.ReadLines());
            return;
        }

        bool ignoreCase = !EnableCasing;

        foreach (string str in IOManager.ReadLines())
        {
            string newStr = str.Replace(m_targetStr, null, ignoreCase, null);
            IOManager.WriteLine(PrependInput ? $"{str}{m_sep}{newStr}" : newStr);
        }
    }
}
