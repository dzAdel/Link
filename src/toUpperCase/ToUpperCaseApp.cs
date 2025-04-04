﻿using Link.Exceptions;



namespace Link;

static class ToUpperCaseApp
{
    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Returns the input string converted to uppercase. " +
                    "This command uses the case rules of the current culture to convert each character in the input " +
                    "to its uppercase equivalent. If a character has no uppercase equivalent, " +
                    "it is included unchanged in the returned string." +
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
            Version? version = typeof(ToUpperCaseApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    static void Run()
    {
        foreach (string str in IOManager.ReadLines())
            IOManager.WriteLine(str.ToUpper());
    }
}
