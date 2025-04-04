using Link.Exceptions;


namespace Link;

static class MergeApp
{
    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Concatenates all lines in the input sequence, using the specified separator between each line." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\nsep:\tThe string to use as a separator." +
                    "\n\tIf ‘sep’ is missing, a colon is used as separator." +
                    "\n--n:\tMerges every ‘N’ input lines." +
                    "\n\tIf this flag is missing, all the input lines are merged together." +
                    "\n\t'N', a positive integer, represents the number of successive lines to be merged together." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count > 1)
                throw new BadUsageException();

            string sep = argParser.Parameters.Count == 1 ? argParser.Parameters[0] : ":";
            IReadOnlyDictionary<string, IReadOnlyList<string>> opts = argParser.Options;
            int count = 0;

            foreach (string opt in argParser.Options.Keys)
            {
                switch (opt)
                {
                    case "n":
                        IReadOnlyList<string> values = argParser.Options[opt];

                        if (values.Count != 1)
                            throw new BadUsageException();

                        if (!int.TryParse(values[0], out count) || count <= 0)
                            throw new BadUsageException();

                        break;

                    default:
                        throw new BadArgException($"--{opt}");
                }
            }

            if (count == 0)
                Run(sep);
            else
                Run(sep, count);

            return 0;
        }
        catch (Exception ex)
        {
            IOManager.LogError($"{AppName}: {ex.Message}");
        }

        return -1;
    }

    //private:
    static string Usage => $"{AppName} [sep] [--n N][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(MergeApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    static void Run(string sep)
    {
        IEnumerator<string> it = IOManager.ReadLines().GetEnumerator();

        if (it.MoveNext())
        {
            IOManager.Write(it.Current);

            while (it.MoveNext())
            {
                IOManager.Write(sep);
                IOManager.Write(it.Current);
            }

            IOManager.WriteLine();
        }
    }

    static void Run(string sep, int lineCount)
    {
        IEnumerator<string> it = IOManager.ReadLines().GetEnumerator();

        while (it.MoveNext())
        {
            IOManager.Write(it.Current);

            for (int i = 2; i < lineCount && it.MoveNext(); ++i)
            {
                IOManager.Write(sep);
                IOManager.Write(it.Current);
            }

            IOManager.WriteLine();
        }
    }
}
