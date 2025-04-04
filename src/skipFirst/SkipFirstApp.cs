using Link.Exceptions;



namespace Link;

sealed class SkipFirstApp
{
    public SkipFirstApp(int count)
    {
        require(count >= 0);

        m_count = count;
    }


    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Bypasses a specified number of input lines and then returns the remaining lines." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\nN:\tThe number of lines to skip before returning the remaining lines." +
                    "\n\tThe default is 1." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            int count = 1;

            if (argParser.Parameters.Count > 1 ||
                    argParser.Options.Count != 0 ||
                    (argParser.Parameters.Count == 1 && (!int.TryParse(argParser.Parameters[0], out count) ||
                    count < 0)))
                throw new BadUsageException();

            new SkipFirstApp(count).Run();
            return 0;
        }
        catch (Exception ex)
        {
            IOManager.LogError($"{AppName}: {ex.Message}");
        }

        return -1;
    }

    //private:
    readonly int m_count;

    static string Usage => $"{AppName} [N] [--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(SkipFirstApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        foreach (string str in IOManager.ReadLines().Skip(m_count))
            IOManager.WriteLine(str);
    }
}
