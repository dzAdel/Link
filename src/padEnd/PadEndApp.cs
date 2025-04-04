using Link.Exceptions;



namespace Link;

sealed class PadEndApp
{
    public PadEndApp(int len)
    {
        require(len >= 0);

        Length = len;
    }

    public int Length { get; }
    public char PaddingChar { get; set; } = ' ';

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "For each line in the input sequence, returns a new string of a specified length in which the end is " +
                    "padded with a specified character. This command pads the end of the target string. " +
                    "This means that, when used with right-to-left languages, it pads the left portion of the string." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\nlen:\tThe number of characters in the resulting string, " +
                    "\n\tequal to the number of original characters plus any additional padding characters. " +
                    "\n\tIf 'len' is less than or equal to the original length of the target string, " +
                    "\n\tthe line is returned unchanged." +
                    "\n--c:\tUses 'char' as padding character." +
                    "\n\tIf this flag is missing, a space is used." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count != 1 || !int.TryParse(argParser.Parameters[0], out int len) || len < 0)
                throw new BadUsageException();

            PadEndApp app = new(len);

            foreach (string opt in argParser.Options.Keys)
            {
                IReadOnlyList<string> values = argParser.Options[opt];

                switch (opt)
                {
                    case "c":
                        if (values.Count != 1 || values[0].Length != 1)
                            throw new BadUsageException();

                        app.PaddingChar = values[0][0];
                        break;

                    default:
                        throw new BadArgException($"--{opt}");
                }
            }

            app.Run();
            return 0;
        }
        catch (Exception ex)
        {
            IOManager.LogError($"{AppName}: {ex.Message}");
        }

        return -1;
    }

    //private:
    static string Usage => $"{AppName} len [--c char][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(PadEndApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        foreach (string s in IOManager.ReadLines())
            IOManager.WriteLine(s.PadRight(Length, PaddingChar));
    }
}
