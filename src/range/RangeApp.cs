using Link.Exceptions;


namespace Link;

sealed class RangeApp
{
    public RangeApp(int count, int start, int step)
    {
        require(count >= 0);
        require(start + ((long)count * step) <= int.MaxValue);
        require(start + ((long)count * step) >= int.MinValue);

        m_count = count;
        m_start = start;
        m_step = step;
    }

    public bool EnablePading { get; set; }

    static int Main(string[] args)
    {
        try
        {
            ArgParser argParser = new(args);

            if (argParser.Options.ContainsKey("h"))
            {
                string hlp = "Generates a sequence of integral numbers." +
                    $"\nVersion {Version}" +
                    $"\nUsage:\t{Usage}" +
                    "\nN:\tA nonnegative integer value that represent the number of sequential integers to generate." +
                    "\n--init:\tSpecifes the value of the first integer in the sequence." +
                    "\n\t'start', the initial value, must be greater than or equal to -2147483648 and " +
                    "\n\t'start' + 'N' * 'step' must be less than or equal to 2147483647." +
                    "\n\tThe default value is 0." +
                    "\n--stp:\tSpecifes an optional increment." +
                    "\n\tThe default is 1." +
                    "\n\t'step', the increment, can be any non-zero integer number as long as " +
                    "\n\tthe value 'start' + 'N' * 'step' is in the interval [-2147483648, 2147483647]." +
                    "\n--pad:\tPads the inserted number with leading zeros according to the longest inserted number." +
                    "\n--h:\tDisplays this help and exits.";

                IOManager.WriteLine(hlp);
                return 0;
            }

            if (argParser.Parameters.Count != 1)
                throw new BadUsageException();

            if (!long.TryParse(argParser.Parameters[0], out long count) || count < 0 || count > int.MaxValue)
                throw new BadUsageException();

            int start = 0;
            int step = 1;
            bool pad = false;

            foreach (string opt in argParser.Options.Keys)
            {
                IReadOnlyList<string> values = argParser.Options[opt];

                switch (opt)
                {
                    case "init":
                        if (values.Count != 1 || !int.TryParse(values[0], out start))
                            throw new BadUsageException();
                        break;

                    case "stp":
                        if (values.Count != 1 || !int.TryParse(values[0], out step) || step == 0)
                            throw new BadUsageException();
                        break;

                    case "pad":
                        if (values.Count != 0)
                            throw new BadUsageException();

                        pad = true;
                        break;

                    default:
                        throw new BadArgException($"--{opt}");
                }
            }

            if (start + (count * step) is > int.MaxValue or < int.MinValue)
                throw new BadUsageException();

            RangeApp app = new((int)count, start, step)
            {
                EnablePading = pad
            };

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
    readonly int m_count;
    readonly int m_start;
    readonly int m_step;

    static string Usage => $"{AppName} N [--init start][--stp step][--pad][--h]";
    static string AppName => System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    static string Version
    {
        get
        {
            Version? version = typeof(RangeApp).Assembly.GetName().Version;

            assert(version != null);
            return version.ToString(3);
        }
    }

    void Run()
    {
        if (EnablePading)
        {
            int padLen = GetPaddingLength();

            for (int i = 0, n = m_start; i < m_count; ++i, n += m_step)
                IOManager.WriteLine(n.ToString($"D{padLen}"));
        }
        else
            for (int i = 0, n = m_start; i < m_count; ++i, n += m_step)
                IOManager.WriteLine(n.ToString());
    }

    int GetPaddingLength()
    {
        long start = m_start;
        long end = start + (m_step * m_count);
        int startLen = Math.Abs(start).ToString().Length;
        int endLen = Math.Abs(end).ToString().Length;

        return Math.Max(startLen, endLen);
    }
}
