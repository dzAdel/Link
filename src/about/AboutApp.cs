using Link.Exceptions;
using System.Diagnostics;

namespace Link;

static class AboutApp
{
    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = $"Provides help information for Link package commands." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\ncmd:\tCommand on which to display information." +
                    "\n\tIf this parameter is not provided, all the commands available in the package are listed." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count > 1 || argParser.Options.Count > 0)
                throw new BadUsageException();

            if (argParser.Parameters.Count == 0)
                IOManager.WriteLines(m_commands.OrderBy(e => e));
            else
            {
                string cmd = argParser.Parameters[0].Trim();

                if (cmd.Length == 0)
                    throw new BadUsageException();

                if (!m_commands.Contains(cmd))
                    throw new BadCommandException(cmd);

                ProcessStartInfo psi = new(cmd, "--h");
                Process.Start(psi)?.WaitForExit(10000);
            }

            return 0;
        }
        catch (BadCommandException ex)
        {
            IOManager.LogError($"{AppName}: '{ex.Command}' is not recognized as a Link command." +
                $"\nUse {AppName} without arguments to display the list of available commands.");
        }
        catch (Exception ex)
        {
            IOManager.LogError($"{AppName}: {ex.Message}");
        }

        return -1;
    }

    //private:
    static readonly HashSet<string> m_commands = new(StringComparer.InvariantCultureIgnoreCase)
    { "about", "repeat", "count", "endsWith", "startsWith", "append", "prepend", "insert", "range",
        "padEnd", "padStart", "reverse", "shuffle", "remove", "dump", "emit", "project",
        "soak", "length", "distinct", "merge", "groupBy", "countFrq", "join", "max",
        "min", "randomizeCase", "serialize", "slice", "split", "putAt", "putFirst", "substitute",
        "putLast", "putWhen", "copyFiles", "enumFiles", "enumDirs", "copyToDir", "createDirs",
        "delDirs", "delFiles", "dirName", "dirSize", "fileExt", "fileName", "fileSize", "fullPath",
        "moveToDir", "pathDate", "relativePath", "renFiles", "reorder", "skip", "skipFirst",
        "skipLast", "skipUntil", "skipWhile", "take", "takeFirst", "takeLast", "takeUntil", "takeWhile",
        "throttle", "toLowerCase", "toUpperCase", "wrap"
    };

    static string Usage => $"{AppName} [cmd] [--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(AboutApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }
}
