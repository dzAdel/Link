using Link.Exceptions;
using System.Text;


namespace Link;

sealed class CountFrqApp
{
    public string Separator
    {
        get => m_sep;
        set
        {
            require(value != null);
            m_sep = value;
        }
    }

    public int Precision
    {
        get => m_precision;
        set
        {
            require(value >= 0);
            m_precision = value;
        }
    }

    public bool IncludeFrequency { get; set; }

    public void CountWords()
    {
        foreach (string s in IOManager.ReadLines())
            foreach (string word in s.Split(null))
                ProcessWord(word);

        ProcessMap();
    }

    public void CountChars()
    {
        foreach (string s in IOManager.ReadLines())
            foreach (Rune r in s.EnumerateRunes())
                ProcessRune(r);

        ProcessMap();
    }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Returns the number of occurrences of each character, or word, " +
                    "that appears in each line of the input sequnece." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\n--wrd:\tCalculates the frequency of words instead of characters." +
                    "\n--frq:\tDisplays the frequency of each item." +
                    "\n\t'precision' is the precision specifier, the number of fractional digits to display for the frequency value." +
                    "\n\tThe default is to display 4 digits after the decimal point." +
                    "\n--sep:\tUses 'separator' as a string separator between each item and its count." +
                    "\n\tIf 'separator' is missing, a colon is used." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count > 0)
                throw new BadUsageException();

            bool countWord = false;
            CountFrqApp app = new();

            foreach (string opt in argParser.Options.Keys)
            {
                IReadOnlyList<string> values = argParser.Options[opt];

                switch (opt)
                {
                    case "frq":
                        {
                            int precision = 4;

                            if (values.Count > 1 || (values.Count == 1 && (!int.TryParse(values[0], out precision) ||
                                    precision < 0)))
                                throw new BadUsageException();

                            app.Precision = precision;
                            app.IncludeFrequency = true;
                        }
                        break;

                    case "sep":
                        if (values.Count != 1)
                            throw new BadUsageException();

                        app.Separator = values[0];
                        break;

                    case "wrd":
                        if (values.Count > 0)
                            throw new BadUsageException();

                        countWord = true;
                        break;

                    default:
                        throw new BadArgException($"--{opt}");
                }
            }

            if (countWord)
                app.CountWords();
            else
                app.CountChars();

            return 0;
        }
        catch (Exception ex)
        {
            IOManager.LogError($"{AppName}: {ex.Message}");
        }

        return -1;
    }

    //private:
    readonly Dictionary<string, int> m_map = new();
    string m_sep = ":";
    int m_precision;
    int m_itemCount;

    static string Usage => $"{AppName} [--wrd][--frq [precision]][--sep separator][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(CountFrqApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void ProcessMap()
    {
        if (IncludeFrequency)
            foreach (string key in m_map.Keys)
            {
                double freq = m_map[key] / (double)m_itemCount;
                IOManager.WriteLine($"{key}{m_sep}{m_map[key]}{m_sep}{freq.ToString($"F{m_precision}")}");
            }
        else
            foreach (string key in m_map.Keys)
                IOManager.WriteLine($"{key}{m_sep}{m_map[key]}");
    }

    void ProcessRune(Rune rune)
    {
        string key = Rune.IsControl(rune) || Rune.IsWhiteSpace(rune) ? $"U+{rune.Value:X4}" : rune.ToString();

        if (!m_map.TryAdd(key, 1))
            ++m_map[key];

        ++m_itemCount;
    }

    void ProcessWord(string key)
    {
        if (!m_map.TryAdd(key, 1))
            ++m_map[key];

        ++m_itemCount;
    }
}
