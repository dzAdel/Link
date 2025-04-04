using Link.Exceptions;


namespace Link;

sealed class EnumDirsApp
{
    public EnumDirsApp(IEnumerable<string> dirs)
    {
        require(dirs != null);
        require(dirs.All(Directory.Exists));

        m_dirs = dirs.ToArray();
    }

    public bool EnableRecursion { get; set; }
    public bool IncludeHiddenFolders { get; set; }
    public bool RemovePath { get; set; }

    public int RecursionDepth
    {
        get => m_depth;
        set
        {
            require(value >= 0);
            require(EnableRecursion);

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
                string hlp = "Lists subdirectories of a specified directories." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\ndir_i:\tDirectory whose contents should be listed." +
                    "\n\tIf no directory is specifiled, the contents of the current folder are listed." +
                    "\n--r:\tList sub-directories recursively." +
                    "\n\t'depth' is the depth of the subfolders to be explored." +
                    "\n\tIf 'depth' is missing, all subfolders are explored." +
                    "\n--hid:\tIncludes hidden folders." +
                    "\n\tThe default is to not include hidden folders." +
                    "\n--dp:\tDiscards the path from each subfolder in the output sequence and returns only the subfolder name." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }


            IReadOnlyList<string> targetDirs = argParser.Parameters.Count == 0 ?
                new string[] { Directory.GetCurrentDirectory() } :
                argParser.Parameters;


            foreach (string dir in targetDirs)
                if (!Directory.Exists(dir))
                    throw new DirectoryNotFoundException($"Could not find a part of the path '{dir}'");

            EnumDirsApp app = new(targetDirs);

            foreach (string opt in argParser.Options.Keys)
            {
                IReadOnlyList<string> values = argParser.Options[opt];

                switch (opt)
                {
                    case "r":
                        if (values.Count > 1)
                            throw new BadUsageException();

                        if (values.Count == 1 && (!int.TryParse(values[0], out app.m_depth) || app.m_depth < 0))
                            throw new BadUsageException();

                        app.EnableRecursion = true;
                        break;

                    case "dp":
                        if (values.Count > 0)
                            throw new BadUsageException();

                        app.RemovePath = true;
                        break;

                    case "hid":
                        if (values.Count > 0)
                            throw new BadUsageException();

                        app.IncludeHiddenFolders = true;
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
    static readonly char[] m_dirSeparators = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
    readonly IEnumerable<string> m_dirs;
    int m_depth = int.MaxValue;


    static string Usage => $"{AppName} [dir_0 dir_1 ...] [--r [depth]][--hid][--dp][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(EnumDirsApp).Assembly.GetName().Version;

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

        if (IncludeHiddenFolders)
            opts.AttributesToSkip = 0;

        foreach (string dir in m_dirs)
            foreach (string subDir in Directory.EnumerateDirectories(dir, "*", opts))
                IOManager.WriteLine(RemovePath ? GetDirName(subDir) : subDir);
    }

    static string GetDirName(string path)
    {
        if (Path.EndsInDirectorySeparator(path))
            path = Path.TrimEndingDirectorySeparator(path);

        int ndx = path.LastIndexOfAny(m_dirSeparators) + 1;
        return path[ndx..];
    }
}
