using Link.Exceptions;

namespace Link;

static class PutFirstApp
{
    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Returns the input sequence to which a specified string has been added at the beginning." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\nstr:\tThe string to prepend to the input sequence." +
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
    static string Usage => $"{AppName} str [--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(PutFirstApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    static void Run(string str)
    {
        require(str != null);

        IOManager.WriteLine(str);
        IOManager.WriteLines(IOManager.ReadLines());
    }
}
