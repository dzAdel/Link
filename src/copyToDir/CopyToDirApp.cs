using Link.Exceptions;


namespace Link;

sealed class CopyToDirApp
{
    public CopyToDirApp(string dest)
    {
        require(!string.IsNullOrWhiteSpace(dest));

        m_dest = dest;
    }


    public bool Overwrite { get; set; }
    public bool ReturnInput { get; set; }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Copies existing files, specifiled in the input sequence, to another directory. " +
                    "The command returns the copied files." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\ndest: \tThe name of the directory to copy to." +
                    "\n\tAll directories specified in 'dest' will be created, unless they already exist." +
                    "\n--ri:\tFor each copied file, returns its input line instead of the destination file." +
                    "\n--ovw:\tDestination file should be replaced if it already exists." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count != 1 || argParser.Options.Count > 2)
                throw new BadUsageException();

            CopyToDirApp app = new(argParser.Parameters[0]);

            foreach (string opt in argParser.Options.Keys)
            {
                if (argParser.Options[opt].Count > 0)
                    throw new BadUsageException();

                switch (opt)
                {
                    case "ri":
                        app.ReturnInput = true;
                        break;

                    case "ovw":
                        app.Overwrite = true;
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
    readonly string m_dest;

    static string Usage => $"{AppName} dest [--ri][--ovw][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(CopyToDirApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        using IEnumerator<string> enumerator = IOManager.ReadLines().GetEnumerator();

        if (!enumerator.MoveNext())
            return;

        if (!Directory.Exists(m_dest))
            Directory.CreateDirectory(m_dest);

        do
        {
            string srcFile = enumerator.Current;
            string fileName = Path.GetFileName(enumerator.Current);
            string destFile = Path.Combine(m_dest, fileName);

            if (IOManager.SamePath(srcFile, destFile) || (!Overwrite && File.Exists(destFile)))
                continue;

            try
            {
                File.Copy(enumerator.Current, destFile, Overwrite);
            }
            catch (Exception ex)
            {
                IOManager.LogError(ex.Message);
                continue;
            }

            IOManager.WriteLine(ReturnInput ? srcFile : destFile);

        } while (enumerator.MoveNext());
    }
}
