using Link.Exceptions;

namespace Link;

public sealed class ArgParser
{
    public ArgParser(string[] args)
    {
        require(args != null);

        Parse(args);
    }

    public IReadOnlyList<string> Parameters => m_params;
    public IReadOnlyDictionary<string, IReadOnlyList<string>> Options => m_opts;


    //private:
    readonly List<string> m_params = new();
    readonly Dictionary<string, IReadOnlyList<string>> m_opts = new();

    void Parse(string[] args)
    {
        //param0 param1 ... --k0 opt00 opt01 ... --k1 opt10 opt11 ...

        if (args.Length == 0)
            return;

        int ndx = 0;

        for (; ndx < args.Length; ++ndx)
        {
            if (args[ndx].StartsWith("--"))
                break;

            m_params.Add(args[ndx]);
        }

        string? opt = null;
        List<string> optParams = new();

        for (; ndx < args.Length; ++ndx)
        {
            if (args[ndx].StartsWith("--"))
            {
                if (opt != null)
                    if (!m_opts.TryAdd(opt[2..], optParams.ToArray()))
                        throw new BadArgException(opt);

                opt = args[ndx];

                if (opt.Length == 2)
                    throw new BadArgException(opt);

                optParams.Clear();
            }
            else
                optParams.Add(args[ndx]);
        }

        //add the last arg
        if (opt != null)
        {
            if (opt.Length == 2)
                throw new BadArgException(opt);

            if (!m_opts.TryAdd(opt[2..], optParams.ToArray()))
                throw new BadArgException(opt);
        }
    }
}
