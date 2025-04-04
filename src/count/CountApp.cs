using Link.Exceptions;

namespace Link;

static class CountApp
{
    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = $"Counts the number of lines in the input sequence." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    $"\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count > 0 || argParser.Options.Count > 0)
                throw new BadUsageException();

            ulong count = 0;

            foreach (string s in IOManager.ReadLines())
                ++count;

            IOManager.WriteLine(count.ToString());
            return 0;
        }
        catch (Exception ex)
        {
            IOManager.LogError($"{AppName}: {ex.Message}");
        }

        return -1;
    }

    //private:
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;
    static string Usage => $"{AppName} [--h]";

    static string Version
    {
        get
        {
            Version? version = typeof(CountApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }
}
