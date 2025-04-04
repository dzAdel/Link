using Link.Exceptions;


namespace Link;

sealed class EnumFilesApp
{
    public EnumFilesApp(IEnumerable<string> dirs)
    {
        require(dirs != null);
        require(dirs.All(Directory.Exists));

        m_dirs = dirs;
    }

    public bool EnableRecursion { get; set; }
    public bool IncludeHiddenFiles { get; set; }
    public bool RemovePath { get; set; }

    public int RecursionDepth
    {
        get => m_depth;
        set
        {
            require(EnableRecursion);
            require(value >= 0);

            m_depth = value;
        }
    }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Lists files contained in a specified directories." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\ndir_i:\tDirectory whose contents should be listed." +
                    "\n\tIf no directory is specifiled, the contents of the current folder are listed." +
                    "\n--r:\tExplore sub-directories contents recursively." +
                    "\n\t'depth' is the depth of the subfolders to be explored." +
                    "\n\tIf 'depth' is missing, all subfolders are explored." +
                    "\n--hid:\tIncludes hidden files." +
                    "\n\tThe default is to not include hidden files." +
                    "\n--dp:\tDiscards the path from each file in the output sequence and returns only the file name." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }


            IEnumerable<string> targetDirs = argParser.Parameters.Count == 0 ?
                Enumerable.Repeat(Directory.GetCurrentDirectory(), 1) :
                argParser.Parameters;


            foreach (string dir in argParser.Parameters)
                if (!Directory.Exists(dir))
                    throw new DirectoryNotFoundException($"Could not find a part of the path '{dir}'");

            EnumFilesApp app = new(targetDirs);

            foreach (string opt in argParser.Options.Keys)
            {
                IReadOnlyList<string> values = argParser.Options[opt];

                switch (opt)
                {
                    case "r":
                        if (values.Count > 1)
                            throw new BadUsageException();

                        int depth = int.MaxValue;

                        if (values.Count == 1 && (!int.TryParse(values[0], out depth) || depth < 0))
                            throw new BadUsageException();

                        app.EnableRecursion = true;
                        app.RecursionDepth = depth;
                        break;

                    case "dp":
                        if (values.Count > 0)
                            throw new BadUsageException();

                        app.RemovePath = true;
                        break;

                    case "hid":
                        if (values.Count > 0)
                            throw new BadUsageException();

                        app.IncludeHiddenFiles = true;
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
    readonly IEnumerable<string> m_dirs;
    int m_depth;

    static string Usage => $"{AppName} [dir_0 dir_1 ...] [--r [depth]][--hid][--dp][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(EnumFilesApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }


    void Run()
    {
        EnumerationOptions opts = new()
        {
            RecurseSubdirectories = EnableRecursion,
            MaxRecursionDepth = m_depth
        };

        if (IncludeHiddenFiles)
            opts.AttributesToSkip = 0;

        foreach (string dir in m_dirs)
            foreach (string file in Directory.EnumerateFiles(dir, "*", opts))
                IOManager.WriteLine(RemovePath ? Path.GetFileName(file) : file);
    }
}
