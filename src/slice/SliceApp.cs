using Link.Exceptions;
using System.Text;


namespace Link;

sealed class SliceApp
{
    public SliceApp(int ndxStart)
    {
        require(ndxStart >= 0);

        m_ndxStart = ndxStart;
        m_count = -1;
    }

    public SliceApp(int ndxStart, int count)
    {
        require(ndxStart >= 0);
        require(count >= 0);

        m_ndxStart = ndxStart;
        m_count = count;
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
                string hlp = "Retrieves a substring from each input line. " +
                    "The substring starts at a specified character position and continues to the end of " +
                    "the line or has a specified length." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t\t{Usage}" +
                    "\nndxStart:\tThe zero-based starting character position of a substring in the input line." +
                    "\n\t\tThe input line is skiped if 'ndxStart' is greater than or equal to the length of the line." +
                    "\nN:\t\tThe number of characters in the substring." +
                    "\n\t\tIf this parameter is missing, the substring starts at 'ndxStart' character position and" +
                    "\n\t\tcontinues to the end of the line." +
                    "\n\t\tThe input line is skiped if 'ndxStart' + 'N' is greater than the length of the input string." +
                    "\n--pi:\t\tFor each ouput line, prepends its input line, using 'sep' as separator." +
                    "\n\t\tIf 'sep' is missing, a colon is used." +
                    "\n--h:\t\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count is 0 or > 2)
                throw new BadUsageException();

            SliceApp app;

            if (!int.TryParse(argParser.Parameters[0], out int ndxStart) || ndxStart < 0)
                throw new BadUsageException();

            if (argParser.Parameters.Count == 2)
            {
                if (!int.TryParse(argParser.Parameters[1], out int count) || count < 0)
                    throw new BadUsageException();

                app = new(ndxStart, count);
            }
            else
                app = new(ndxStart);

            foreach (string opt in argParser.Options.Keys)
            {
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
    readonly int m_ndxStart;
    readonly int m_count;

    static string Usage => $"{AppName} ndxStart [N] [--pi [sep]] [--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(SliceApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        StringBuilder sb = new();

        if (m_count == -1)
            foreach (string str in IOManager.ReadLines())
            {
                foreach (Rune rune in str.EnumerateRunes().Skip(m_ndxStart))
                    sb.Append(rune.ToString());

                IOManager.WriteLine(PrependInput ? $"{str}{m_sep}{sb}" : sb.ToString());
                sb.Clear();
            }
        else
            foreach (string str in IOManager.ReadLines())
            {
                foreach (Rune rune in str.EnumerateRunes().Skip(m_ndxStart).Take(m_count))
                    sb.Append(rune.ToString());

                IOManager.WriteLine(PrependInput ? $"{str}{m_sep}{sb}" : sb.ToString());
                sb.Clear();
            }
    }
}
