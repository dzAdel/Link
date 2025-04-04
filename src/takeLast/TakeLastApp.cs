using Link.Exceptions;


namespace Link;

sealed class TakeLastApp
{
    public TakeLastApp(int count)
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
                string hlp = "Returns the last 'N' input lines." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\nN:\tThe number of lines to take from the end of the input sequence." +
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

            new TakeLastApp(count).Run();
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
            Version? version = typeof(TakeLastApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        if (m_count > 0)
            IOManager.WriteLines(IOManager.ReadLines().TakeLast(m_count));
        else
            foreach (string _ in IOManager.ReadLines())
                ; //nop
    }
}
