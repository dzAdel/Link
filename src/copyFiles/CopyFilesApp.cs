using Link.Exceptions;
using System.Text.RegularExpressions;


namespace Link;

sealed class CopyFilesApp
{
    public CopyFilesApp(string rxPtrn, string dest)
    {
        require(rxPtrn != null);
        require(dest != null);

        m_rxPtrn = rxPtrn;
        m_dest = dest;
    }

    public bool EnableCasing { get; set; }
    public bool OverwriteDestination { get; set; }
    public bool ReturnInput { get; set; }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Copies the files in the input sequence to another location. Unless they already exist, " +
                    "all directories specified in the destinations will be created. The command returns the copied files." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\nptrn:\tRepresents a regular expression pattern to match." +
                    "\n\tThe input lines for which 'ptrn' is not matched are ignored." +
                    "\ndest:\tA transform pattern to apply to each input line to extract the destination file." +
                    "\n\tIt can consist of any combination of literal text and substitutions based on 'ptrn'," +
                    "\n\tsuch as capturing group that is identified by a number or a name." +
                    "\nsrc:\tA transform pattern to apply to each input line to extract the source file." +
                    "\n\tIt can consist of any combination of literal text and substitutions based on 'ptrn'," +
                    "\n\tsuch as capturing group that is identified by a number or a name." +
                    "\n\tBy default, the input line is considered the source file." +
                    "\n--ri:\tFor each copied file, returns its input line. " +
                    "\n\tIf this switch is omitted, the command returns the destination file." +
                    "\n--ovw:\tDestination file should be replaced if it already exists." +
                    "\n--cs:\tSpecifies case-sensitive matching." +
                    "\n--h:\tDisplays this help and exits.";


                IOManager.WriteLine(hlp);
                return 0;
            }


            int paramCount = argParser.Parameters.Count;

            if (paramCount is < 2 or > 3)
                throw new BadUsageException();

            CopyFilesApp app = new(argParser.Parameters[0], argParser.Parameters[1]);

            foreach (string opt in argParser.Options.Keys)
            {
                if (argParser.Options[opt].Count > 0)
                    throw new BadUsageException();

                switch (opt)
                {
                    case "cs":
                        app.EnableCasing = true;
                        break;

                    case "ovw":
                        app.OverwriteDestination = true;
                        break;

                    case "ri":
                        app.ReturnInput = true;
                        break;

                    default:
                        throw new BadArgException($"--{opt}");
                }
            }

            if (paramCount == 3)
                app.Run(argParser.Parameters[2]);
            else
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
    readonly string m_rxPtrn;
    readonly string m_dest;

    static string Usage => $"{AppName} ptrn dest [src][--ri][--ovw][--cs][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(CopyFilesApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run(string src)
    {
        require(src != null);

        RegexOptions options = EnableCasing ? RegexOptions.CultureInvariant | RegexOptions.Compiled :
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant;

        Regex rx = new(m_rxPtrn, options);

        foreach (string str in IOManager.ReadLines())
        {
            Match m = rx.Match(str);

            if (m.Success)
            {
                string destFile = m.Result(m_dest);
                string srcFile = m.Result(src);

                if (IOManager.SamePath(srcFile, destFile) || (!OverwriteDestination && File.Exists(destFile)))
                    continue;

                try
                {
                    string? dir = Path.GetDirectoryName(destFile);

                    if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    File.Copy(srcFile, destFile, OverwriteDestination);
                }
                catch (Exception ex)
                {
                    IOManager.LogError($"{AppName}: {ex.Message}");
                    continue;
                }

                IOManager.WriteLine(ReturnInput ? str : destFile);
            }
        }
    }

    void Run()
    {
        RegexOptions options = EnableCasing ? RegexOptions.CultureInvariant | RegexOptions.Compiled :
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant;

        Regex rx = new(m_rxPtrn, options);

        foreach (string srcFile in IOManager.ReadLines())
        {
            Match m = rx.Match(srcFile);

            if (m.Success)
            {
                string destFile = m.Result(m_dest);

                if (IOManager.SamePath(srcFile, destFile) || (!OverwriteDestination && File.Exists(destFile)))
                    continue;

                try
                {
                    string? dir = Path.GetDirectoryName(destFile);

                    if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    File.Copy(srcFile, destFile, OverwriteDestination);
                }
                catch (Exception ex)
                {
                    IOManager.LogError($"{AppName}: {ex.Message}");
                    continue;
                }

                IOManager.WriteLine(ReturnInput ? srcFile : destFile);
            }
        }
    }
}
