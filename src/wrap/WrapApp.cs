using Link.Exceptions;
using System.Globalization;



namespace Link;

static class WrapApp
{
    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "For each input line, puts a line feed every 'N' characters, if it does not " +
                    "reach a new line before that point." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\nN:\tA positive integer which correspond to the maximum length of a line." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count != 1 || !int.TryParse(argParser.Parameters[0], out int count) || count <= 0)
                throw new BadUsageException();

            if (argParser.Options.Count != 0)
                throw new BadUsageException();

            Run(count);
            return 0;
        }
        catch (Exception ex)
        {
            IOManager.LogError($"{AppName}: {ex.Message}");
        }

        return -1;
    }

    //private:
    static string Usage => $"{AppName} N [--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(WrapApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    static void Run(int count)
    {
        require(count > 0);

        foreach (string str in IOManager.ReadLines())
        {
            StringInfo si = new(str);
            int len = si.LengthInTextElements;

            if (len <= count)
                IOManager.WriteLine(str);
            else
            {
                int ndx = 0;

                while (ndx + count < len)
                {
                    IOManager.WriteLine(si.SubstringByTextElements(ndx, count));
                    ndx += count;
                }

                if (ndx != len)
                    IOManager.WriteLine(si.SubstringByTextElements(ndx));
            }
        }
    }
}
