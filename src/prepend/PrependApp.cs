using Link.Exceptions;

namespace Link;

static class PrependApp
{
    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Adds a string to the beginning of each line in the input sequence." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\nprefix:\tThe string to prepend to each input line." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count != 1 || argParser.Options.Count != 0)
                throw new BadUsageException();

            Run(argParser.Parameters[0]);
            return 0;
        }
        catch (Exception ex)
        {
            IOManager.LogError($"{AppName}: {ex.Message}");
        }

        return -1;
    }

    //private:
    static string Usage => $"{AppName} prefix [--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(PrependApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    static void Run(string prefix)
    {
        require(prefix != null);

        foreach (string str in IOManager.ReadLines())
            IOManager.WriteLine(prefix + str);
    }
}
