using Link.Exceptions;
using System.Text.RegularExpressions;

namespace Link;

sealed class PutWhenApp
{
    public PutWhenApp(string rxPattern, string str)
    {
        require(rxPattern != null);
        require(str != null);

        m_rxPattern = rxPattern;
        m_str = str;
    }

    public bool CaseSensitive { get; set; }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Returns the lines of the input sequence until a specified pattern is matched, " +
                    "then inserts a specified string and returns the remaining lines." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\nptrn:\tA regular expression pattern to match." +
                    "\nstr:\tThe string to insert." +
                    "\n--cs:\tSpecifies case-sensitive matching." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count != 2)
                throw new BadUsageException();

            PutWhenApp app = new(argParser.Parameters[0], argParser.Parameters[1]);

            if (argParser.Options.Count > 1)
                throw new BadUsageException();

            if (argParser.Options.Count == 1)
            {
                string opt = argParser.Options.Keys.Single();

                if (opt != "cs")
                    throw new BadArgException($"--{opt}");

                if (argParser.Options["cs"].Count != 0)
                    throw new BadUsageException();

                app.CaseSensitive = true;
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
    readonly string m_rxPattern;
    readonly string m_str;

    static string Usage => $"{AppName} ptrn str [--cs][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(PutWhenApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        RegexOptions options = CaseSensitive ? RegexOptions.CultureInvariant | RegexOptions.Compiled :
           RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant;

        Regex rx = new(m_rxPattern, options);
        using IEnumerator<string> enumerator = IOManager.ReadLines().GetEnumerator();

        while (enumerator.MoveNext())
        {
            IOManager.WriteLine(enumerator.Current);

            if (rx.IsMatch(enumerator.Current))
            {
                IOManager.WriteLine(m_str);
                break;
            }
        }

        while (enumerator.MoveNext())
            IOManager.WriteLine(enumerator.Current);
    }
}
