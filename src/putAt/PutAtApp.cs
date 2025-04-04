using Link.Exceptions;


namespace Link;

static class PutAtApp
{
    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Returns the input sequence to which a specified string has been inserted at a specified index." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\nstr:\tThe string to insert." +
                    "\nndx:\tThe zero-based index at which 'str' should be inserted." +
                    "\n\tIt is an error to specify an index greater than the length of sequences." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }


            if (argParser.Options.Count != 0 ||
                    argParser.Parameters.Count != 2 ||
                    !int.TryParse(argParser.Parameters[1], out int ndx) || ndx < 0)
                throw new BadUsageException();

            Run(argParser.Parameters[0], ndx);
            return 0;
        }
        catch (Exception ex)
        {
            IOManager.LogError($"{AppName}: {ex.Message}");
        }

        return 0;
    }

    //private:
    static string Usage => $"{AppName} str ndx [--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(PutAtApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    static void Run(string str, int index)
    {
        using IEnumerator<string> enumerator = IOManager.ReadLines().GetEnumerator();
        int ndx = 0;

        while (ndx < index && enumerator.MoveNext())
        {
            IOManager.WriteLine(enumerator.Current);
            ++ndx;
        }

        if (ndx < index)
            throw new IndexOutOfRangeException("Index was outside the bounds of the input sequence.");

        IOManager.WriteLine(str);

        while (enumerator.MoveNext())
            IOManager.WriteLine(enumerator.Current);
    }
}
