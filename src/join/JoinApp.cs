using easyLib.Disposables;
using Link.Exceptions;
using System.Text;


namespace Link;

sealed class JoinApp
{
    public JoinApp(IReadOnlyList<string> files)
    {
        require(files != null);
        require(!files.Any(string.IsNullOrEmpty));
        require(files.All(File.Exists));

        m_files = files.ToList();
    }

    public string Separator
    {
        get => m_sep;
        set
        {
            require(value != null);

            m_sep = value;
        }
    }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Produces a sequence with lines from the input sequence and those from one or more " +
                    "specified files. The command merges each line of the input sequence with lines having " +
                    "the same index in the specified files. The command merges sequences until it reaches " +
                    "the end of one of them." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\nfile_i:\tFile from which lines to merge are read." +
                    "\n--sep:\tUses 'str' as separator between joined lines." +
                    "\n\tIf 'str' is missing, a colon is used as separator." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count == 0)
                throw new BadUsageException();

            foreach (string file in argParser.Parameters)
                if (!File.Exists(file))
                    throw new FileNotFoundException(null, file, null);

            JoinApp app = new(argParser.Parameters);

            if (argParser.Options.Count > 1)
                throw new BadUsageException();

            if (argParser.Options.Count == 1)
            {
                if (argParser.Options.Keys.Single() != "sep" || argParser.Options["sep"].Count != 1)
                    throw new BadUsageException();

                app.Separator = argParser.Options["sep"][0];
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
    readonly List<string> m_files;
    string m_sep = ":";

    static string Usage => $"{AppName} file_0 [file_1 file_2 ...][--sep str][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(JoinApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        using DisposableCollection disposables = new();
        List<StreamReader> readers = new(m_files.Count);

        foreach (string str in m_files)
        {
            StreamReader reader = File.OpenText(str);
            disposables.Add(reader);
            readers.Add(reader);
        }

        IEnumerator<string> enumerator = IOManager.ReadLines().GetEnumerator();
        disposables.Add(enumerator);

        StringBuilder sb = new();

        while (enumerator.MoveNext())
        {
            sb.Append(enumerator.Current);

            bool done = false;

            for (int i = 0; i < readers.Count; ++i)
            {
                string? line = readers[i].ReadLine();

                if (line == null)
                {
                    done = true;
                    break;
                }

                sb.Append(m_sep).Append(line);
            }

            if (done)
                break;

            IOManager.WriteLine(sb.ToString());
            sb.Clear();
        }

        while (enumerator.MoveNext())
            ;   //nop
    }
}
