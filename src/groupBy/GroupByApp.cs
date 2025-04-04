using Link.Exceptions;
using System.Text.RegularExpressions;



namespace Link;

sealed class GroupByApp
{
    public bool EnableCasing { get; set; }

    public void Run(string rxPattern, string rxKey)
    {
        require(rxPattern != null);
        require(rxKey != null);

        RegexOptions opts = EnableCasing ? RegexOptions.Compiled | RegexOptions.CultureInvariant :
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant;

        Regex rx = new(rxPattern, opts);

        foreach (string str in IOManager.ReadLines())
        {
            try
            {
                Match m = rx.Match(str);

                if (m.Success)
                {
                    string key = m.Result(rxKey);

                    if (!m_map.TryGetValue(key, out List<string>? values))
                    {
                        values = new List<string>();
                        m_map[key] = values;
                    }

                    values.Add(str);
                }
            }
            catch (Exception ex)
            {
                IOManager.LogError(ex.Message);
            }
        }

        foreach (string key in m_map.Keys)
            IOManager.WriteLines(m_map[key]);
    }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Groups the elements of the input sequence according to a specified key." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\nptrn:\tRepresents a regular expression pattern to match." +
                    "\n\tThe input lines for which ‘ptrn’ does not match are ignored." +
                    "\nkey:\tThe key used to group input lines." +
                    "\n\tThis is a transform pattern to apply to each input line to extract the key." +
                    "\n\tIt can consist of any combination of literal text and substitutions based on 'ptrn'," +
                    "\n\tsuch as capturing group that is identified by a number or a name." +
                    "\n--cs:\tSpecifies case-sensitive pattern matching." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count != 2)
                throw new BadUsageException();

            string rxPattern = argParser.Parameters[0];
            string rxKey = argParser.Parameters[1];

            GroupByApp app = new();

            foreach (string opt in argParser.Options.Keys)
            {
                switch (opt)
                {
                    case "cs":
                        if (argParser.Options[opt].Count > 0)
                            throw new BadUsageException();

                        app.EnableCasing = true;
                        break;

                    default:
                        throw new BadArgException($"--{opt}");
                }
            }

            app.Run(rxPattern, rxKey);

            return 0;
        }
        catch (Exception ex)
        {
            IOManager.LogError($"{AppName}: {ex.Message}");
        }

        return -1;
    }

    //private:
    readonly Dictionary<string, List<string>> m_map = new();

    static string Usage => $"{AppName} ptrn key [--cs][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(GroupByApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }


}
