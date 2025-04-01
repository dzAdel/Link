namespace Link;

public static class Formater
{
    public static string FormatSize(long size)
    {
        const long kilo = 1024;
        const long mega = kilo * 1024;
        const long giga = mega * 1024;
        const long tera = giga * 1024;
        double sz = size;

        return sz > tera ? $"{sz / tera:N}T"
            : sz > giga ? $"{sz / giga:N}G"
            : sz > mega ? $"{sz / mega:N}M"
            : sz > kilo ? $"{sz / kilo:N}K"
            : $"{size:N}B";
    }
}
