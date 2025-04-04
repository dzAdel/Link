using Link.Exceptions;



namespace Link;

sealed class MoveToDirApp
{
    public MoveToDirApp(string dir)
    {
        require(!string.IsNullOrEmpty(dir));

        m_dest = dir;
    }

    public bool Overwrite { get; set; }
    public bool ReturnInput { get; set; }

    public void Run()
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
                File.Move(enumerator.Current, destFile, Overwrite);
                IOManager.WriteLine(ReturnInput ? srcFile : destFile);
            }
            catch (Exception ex)
            {
                IOManager.LogError($"{AppName}: {ex.Message}");
            }

        } while (enumerator.MoveNext());
    }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = $"Moves each file in the input sequence to a specified directory. Returns successfully moved files." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    $"\ndir:\tThe destinition directory." +
                    $"\n\tAll directories specified in 'dir' will be created, unless they already exist." +
                    $"\n--ri:\tFor each file moved, returns its input line instead of the destination file." +
                    $"\n--ovw:\tDestination file should be replaced if it already exists" +
                    $"\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count != 1 || argParser.Options.Count > 2)
                throw new BadUsageException();

            MoveToDirApp app = new(argParser.Parameters[0]);

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

    static string Usage => $"{AppName} dir [--ri][--ovw][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(MoveToDirApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }
}
