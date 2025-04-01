namespace Link.Exceptions;

public sealed class BadArgException : Exception
{
    public BadArgException(string arg, Exception? innerException = null) :
        base(null, innerException)
    {
        require(!string.IsNullOrEmpty(arg));

        Argument = arg;
    }

    public string Argument { get; }

    public override string Message
    {
        get
        {
            string appName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

            return $"Invalid argument '{Argument}'. Try '{appName} --h' for help.";
        }
    }
}
