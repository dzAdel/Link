namespace Link.Exceptions;

public sealed class BadUsageException : Exception
{
    public BadUsageException(Exception? innerException = null):
        base(null, innerException)
    { }

    public override string Message
    {
        get
        {
            string appName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

            return $"The syntax of the command is incorrect. Try '{appName} --h' for help.";
        }
    }
}
