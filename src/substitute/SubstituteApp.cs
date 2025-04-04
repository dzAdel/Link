using Link.Exceptions;

namespace Link;

sealed class SubstituteApp
{
    public SubstituteApp(string oldStr, string newStr)
    {
        require(!string.IsNullOrEmpty(oldStr));
        require(newStr != null);

        OldString = oldStr;
        NewString = newStr;
    }

    public string OldString { get; }
    public string NewString { get; }
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
                string hlp = "Returns, for each input line, a new string in which all occurrences of a specified string " +
                    "are replaced with another string." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\noldStr:\tThe substring to be replaced." +
                    "\nnewStr:\tThe string to replace all occurrences of 'oldStr'." +
                    "\n--cs:\tSpecifies case-sensitive matching." +
                    "\n--pi:\tFor each result, prepends its input line, using 'sep' as separator." +
                    "\n\tIf 'sep' is missing, a colon is used." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count != 2 || argParser.Parameters[0].Length == 0)
                throw new BadUsageException();

            string newStr = argParser.Parameters[1];
            SubstituteApp app = new(argParser.Parameters[0], newStr);

            foreach (string opt in argParser.Options.Keys)
            {
                IReadOnlyList<string> values = argParser.Options[opt];

                switch (opt)
                {
                    case "cs":
                        if (values.Count != 0)
                            throw new BadUsageException();

                        app.EnableCasing = true;
                        break;

                    case "pi":
                        if (values.Count > 1)
                            throw new BadUsageException();

                        app.PrependInput = true;

                        if (values.Count == 1)
                            app.Separator = values[0];
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
    string m_sep = ":";

    static string Usage => $"{AppName} oldStr newStr [--cs][--pi [sep]][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(SubstituteApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }


    void Run()
    {
        StringComparison opt = EnableCasing ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;

        foreach (string str in IOManager.ReadLines())
        {
            string s = str.Replace(OldString, NewString, opt);
            IOManager.WriteLine(PrependInput ? $"{str}{m_sep}{s}" : s);
        }
    }
}
