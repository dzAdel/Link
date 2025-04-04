﻿using Link.Exceptions;
using System.Text.RegularExpressions;



namespace Link;

sealed class FullPathsApp
{
    public bool EnableCasing { get; set; }
    public bool PrependInput { get; set; }

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
                string hlp = "Returns the absolute path for every file or directory in the input sequence. " +
                    "The path need not exist." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\n--rx:\tUse regular expression to extract, from the input line, the target path." +
                    "\n\t‘ptrn’ represents a regular expression pattern to match against each input line." +
                    "\n\tThe input lines for which ‘ptrn’ does not match are ignored." +
                    "\n\t‘path’ is a transform pattern to apply to each input line to extract the target path." +
                    "\n\tIt can consist of any combination of literal text and substitutions based on 'ptrn'," +
                    "\n\tsuch as capturing group that is identified by a number or a name." +
                    "\n--cs:\tSpecifies case-sensitive matching." +
                    "\n\tIt's an error to specify this option without the '--rx' switch." +
                    "\n--pi:\tFor each line in the output sequence, prepend its input line, using 'sep' as separator." +
                    "\n\tIf 'sep' is missing, a colon is used." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            string? rxPattern = null;
            string? rxPath = null;
            FullPathsApp app = new();

            foreach (string opt in argParser.Options.Keys)
            {
                IReadOnlyList<string> values = argParser.Options[opt];

                switch (opt)
                {
                    case "rx":
                        if (values.Count != 2)
                            throw new BadUsageException();

                        rxPattern = values[0];
                        rxPath = values[1];
                        break;

                    case "pi":
                        if (values.Count > 1)
                            throw new BadUsageException();

                        app.PrependInput = true;

                        if (values.Count == 1)
                            app.Separator = values[0];
                        break;

                    case "cs":
                        if (values.Count != 0)
                            throw new BadUsageException();

                        app.EnableCasing = true;
                        break;

                    default:
                        throw new BadArgException($"--{opt}");
                }
            }


            if (rxPattern != null)
            {
                assert(rxPath != null);
                app.Run(rxPattern, rxPath);
            }
            else
            {
                if (app.EnableCasing)
                    throw new BadUsageException();

                app.Run();
            }

            return 0;
        }
        catch (Exception ex)
        {
            IOManager.LogError($"{AppName}: {ex.Message}");
        }

        return -1;
    }

    //private:
    string m_sep = ":";

    static string Usage => $"{AppName} [--rx ptrn path [--cs]] [--pi [sep]] [--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(FullPathsApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        foreach (string path in IOManager.ReadLines())
            try
            {
                string fullPath = Path.GetFullPath(path);

                IOManager.WriteLine(PrependInput ? $"{path}{m_sep}{fullPath}" : fullPath);
            }
            catch (Exception ex)
            {
                IOManager.LogError($"{AppName}: {ex.Message}");
            }
    }

    void Run(string rxPattern, string rxPath)
    {
        require(rxPattern != null);
        require(rxPath != null);

        RegexOptions opts = RegexOptions.Compiled | RegexOptions.CultureInvariant;

        if (!EnableCasing)
            opts |= RegexOptions.IgnoreCase;

        Regex rx = new(rxPattern, opts);

        foreach (string str in IOManager.ReadLines())
        {
            try
            {
                Match m = rx.Match(str);

                if (m.Success)
                {
                    string path = m.Result(rxPath);
                    string fullPath = Path.GetFullPath(path);

                    IOManager.WriteLine(PrependInput ? $"{str}{m_sep}{fullPath}" : fullPath);
                }
            }
            catch (Exception ex)
            {
                IOManager.LogError($"{AppName}: {ex.Message}");
            }
        }
    }
}
