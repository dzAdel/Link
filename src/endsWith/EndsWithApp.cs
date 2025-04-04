using Link.Exceptions;


namespace Link;

static class EndsWithApp
{
    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = $"Returns lines of the input sequence that ends with a specified string." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    $"\nsuffix:\tThe string to compare to the sub-string at the end of each input line." +
                    $"\n--cs:\tSpecifies case-sensitive comparison." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count != 1)
                throw new BadUsageException();

            string suffix = argParser.Parameters[0];
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

            Run(suffix, enableCasing);
            return 0;
        }
        catch (Exception ex)
        {
            IOManager.LogError($"{AppName}: {ex.Message}");
        }

        return -1;
    }

    //private:
    static string Usage => $"{AppName} suffix [--cs][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(EndsWithApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    static void Run(string suffix, bool enableCasing)
    {
        require(suffix != null);

        StringComparison opts = enableCasing ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;

        foreach (string s in IOManager.ReadLines())
            if (s.EndsWith(suffix, opts))
                IOManager.WriteLine(s);
    }
}
