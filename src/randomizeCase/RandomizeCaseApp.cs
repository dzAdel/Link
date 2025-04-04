using Link.Exceptions;
using System.Text;

namespace Link;

static class RandomizeCaseApp
{
    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "For each line in the input sequence, each character gets a random case." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count != 0 || argParser.Options.Count != 0)
                throw new BadUsageException();

            Run();
            return 0;
        }
        catch (Exception ex)
        {
            IOManager.LogError($"{AppName}: {ex.Message}");
        }

        return -1;
    }

    //private:
    static string Usage => $"{AppName} [--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(RandomizeCaseApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    static void Run()
    {
        StringBuilder sb = new();
        Random rand = new();

        foreach (string str in IOManager.ReadLines())
        {
            foreach (char c in str)
                sb.Append(rand.Next() % 2 == 0 ? char.ToLower(c) : char.ToUpper(c));

            IOManager.WriteLine(sb.ToString());
            sb.Clear();
        }
    }
}
