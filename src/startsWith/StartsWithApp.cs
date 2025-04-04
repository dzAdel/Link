using Link.Exceptions;

namespace Link;

static class StartsWithApp
{
    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = $"Returns all input lines that starts with a specified string." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    $"\nprefix:\tThe string to compare to the substring at the beginning of each input line." +
                    $"\n--cs:\tSpecifies a case-sensitive comparison." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count != 1)
                throw new BadUsageException();

            string prefix = argParser.Parameters[0];
            bool enableCasing = false;

            foreach (string opt in argParser.Options.Keys)
            {
                IReadOnlyList<string> values = argParser.Options[opt];

                switch (opt)
                {
                    case "cs":
                        if (values.Count > 0)
                            throw new BadUsageException();

                        enableCasing = true;
                        break;

                    default:
                        throw new BadArgException($"--{opt}");
                }
            }

            Run(prefix, enableCasing);
            return 0;
        }
        catch (Exception ex)
        {
            IOManager.LogError($"{AppName}: {ex.Message}");
        }

        return -1;
    }

    //private:
    static string Usage => $"{AppName} prefix [--cs][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(StartsWithApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    static void Run(string prefix, bool caseSensitive)
    {
        require(prefix != null);

        StringComparison opt = caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;

        foreach (string str in IOManager.ReadLines())
            if (str.StartsWith(prefix, opt))
                IOManager.WriteLine(str);
    }
}
