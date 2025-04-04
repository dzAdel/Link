using Link.Exceptions;
using System.Text.RegularExpressions;

namespace Link;

static class DelFilesApp
{
    public static bool EnableCasing { get; set; }
    public static bool ReturnInput { get; set; }


    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Deletes the specified files in the input sequence. Returns the successfully deleted files. " +
                    "Non-existent files are ignored." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\n--rx:\tUses an optional regular expression to extract, from the input line, the target file." +
                    "\nptrn:\tRepresents a regular expression pattern to match against each input line." +
                    "\n\tThe input line for which ‘ptrn’ is not matched are ignored." +
                    "\nfile:\tThe file to delete." +
                    "\n\tIt can consist of any combination of literal text and substitutions based on 'ptrn'," +
                    "\n\tsuch as capturing group that is identified by a number or a name." +
                    "\n--cs:\tSpecifies case-sensitive matching." +
                    "\n\tIt's an error to specify this option without the '--rx' switch." +
                    "\n--ri:\tFor each file deleted, returns its input line instead of the target file." +
                    "\n\tIt's an error to specify this option without the '--rx' switch." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count > 0)
                throw new BadUsageException();


            string? rxPtrn = null, rxFile = null;

            foreach (string opt in argParser.Options.Keys)
            {
                int optCount = argParser.Options[opt].Count;

                switch (opt)
                {
                    case "rx":
                        if (optCount != 2)
                            throw new BadUsageException();

                        rxPtrn = argParser.Options["rx"][0];
                        rxFile = argParser.Options["rx"][1];

                        break;

                    case "cs":
                        if (optCount != 0)
                            throw new BadUsageException();

                        EnableCasing = true;
                        break;

                    case "ri":
                        if (optCount != 0)
                            throw new BadUsageException();

                        ReturnInput = true;
                        break;

                    default:
                        throw new BadArgException($"--{opt}");
                }
            }



            if (rxPtrn != null)
            {
                assert(rxFile != null);
                Run(rxPtrn, rxFile);
            }
            else
            {
                if (EnableCasing || ReturnInput)
                    throw new BadUsageException();

                Run();
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
    static string Usage => $"{AppName} [--rx ptrn file [--cs]][--ri]] [--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(DelFilesApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    static void Run()
    {
        foreach (string file in IOManager.ReadLines())
            try
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                    IOManager.WriteLine(file);
                }
            }
            catch (Exception ex)
            {
                IOManager.LogError(ex.Message);
            }
    }

    static void Run(string rxPattern, string rxFile)
    {
        RegexOptions rxOpt = RegexOptions.Compiled;

        if (!EnableCasing)
            rxOpt |= RegexOptions.IgnoreCase;


        Regex rx = new(rxPattern, rxOpt);

        foreach (string str in IOManager.ReadLines())
            try
            {
                Match m = rx.Match(str);

                if (rx.IsMatch(str))
                {
                    string file = m.Result(rxFile);

                    if (File.Exists(file))
                    {
                        File.Delete(file);
                        IOManager.WriteLine(ReturnInput ? str : file);
                    }
                }
            }
            catch (Exception ex)
            {
                IOManager.LogError(ex.Message);
            }
    }
}
