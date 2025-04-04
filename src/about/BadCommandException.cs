namespace Link;

sealed class BadCommandException : Exception
{
    public BadCommandException(string cmd, Exception? innerException = null) :
        base(null, innerException)
    {
        require(!string.IsNullOrWhiteSpace(cmd));

        Command = cmd;
    }

    public string Command { get; }
}
