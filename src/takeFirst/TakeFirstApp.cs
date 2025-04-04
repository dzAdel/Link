using Link.Exceptions;


namespace Link;

sealed class TakeFirstApp
{
    public TakeFirstApp(int count)
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
                string hlp = "Returns a specified number of input lines and then skips the remaining ones." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\nN:\tThe number of input lines to return." +
                    "\n\tThe default is 1." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count > 1 || argParser.Options.Count != 0)
                throw new BadUsageException();

            int count = 1;

            if (argParser.Parameters.Count == 1)
                if (!int.TryParse(argParser.Parameters[0], out count) || count < 0)
                    throw new BadUsageException();

            new TakeFirstApp(count).Run();
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
            Version? version = typeof(TakeFirstApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        using IEnumerator<string> it = IOManager.ReadLines().GetEnumerator();
        int count = m_count;

        if (count > 0)
            while (it.MoveNext())
            {
                IOManager.WriteLine(it.Current);

                if (--count == 0)
                    break;
            }

        while (it.MoveNext())
            ;   //nop
    }
}
