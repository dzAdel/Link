using Link.Exceptions;

namespace Link;

static class RepeatApp
{
    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Generates a sequence that contains one repeated string." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\nstr:\tThe string to be repeated." +
                    "\nN:\tThe number of times to repeat the string in the generated sequence." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count != 2 || argParser.Options.Count != 0)
                throw new BadUsageException();

            if (!int.TryParse(argParser.Parameters[1], out int count) || count < 0)
                throw new BadArgException(argParser.Parameters[1]);

            Run(argParser.Parameters[0], count);
            return 0;
        }
        catch (Exception ex)
        {
            IOManager.LogError($"{AppName}: {ex.Message}");
        }

        return -1;
    }

    //private:
    static string Usage => $"{AppName} str N [--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(RepeatApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    static void Run(string str, int count)
    {
        require(str != null);
        require(count >= 0);

        IOManager.WriteLines(Enumerable.Repeat(str, count));
    }
}
