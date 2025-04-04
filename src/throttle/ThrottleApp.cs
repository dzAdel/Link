using Link.Exceptions;
using System.Runtime.InteropServices;



namespace Link;

sealed class ThrottleApp
{
    public ThrottleApp(int nbLines)
    {
        require(nbLines > 0);

        m_nbLines = nbLines;
    }


    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Displays a maximum of 'N' lines at a time and then waits the user to press a key " +
                    "to display the next 'N' lines. The 'Q' and 'ESC' keys are used to interrupt the execution of the program." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\nN:\tThe number of lines to display each time." +
                    "\n\tThe default is 1." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            int paramCount = argParser.Parameters.Count;

            if (paramCount > 1 || argParser.Options.Count > 0)
                throw new BadUsageException();

            int nbLine = 1;

            if (paramCount == 1 && (!int.TryParse(argParser.Parameters[0], out nbLine) || nbLine <= 0))
                throw new BadUsageException();

            new ThrottleApp(nbLine).Run();
            return 0;
        }
        catch (Exception ex)
        {
            IOManager.LogError($"{AppName}: {ex.Message}");
        }

        return -1;
    }

    //private:
    readonly int m_nbLines;

    static string Usage => $"{AppName} [N] [--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(ThrottleApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        var strs = IOManager.ReadLines().ToList();
        int count = 0;

        FreeConsole();
        AttachConsole(-1);

        for (int i = 0; i < strs.Count; ++i)
        {
            string str = strs[i];
            Console.WriteLine(str);

            if (++count == m_nbLines)
            {
                if (i == strs.Count - 1)
                    return;

                ConsoleKey k = Console.ReadKey(true).Key;

                if (k is ConsoleKey.Q or ConsoleKey.Escape)
                    return;

                count = 0;
            }
        }
    }


    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool FreeConsole();

    [DllImport("kernel32", SetLastError = true)]
    static extern bool AttachConsole(int dwProcessId);
}
