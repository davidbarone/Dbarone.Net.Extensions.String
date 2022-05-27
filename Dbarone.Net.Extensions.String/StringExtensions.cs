namespace Dbarone.Net.Extensions.String;
using System.Text;

public enum Justification
{
    LEFT,
    CENTRE,
    RIGHT
}

public static class StringExtensions
{
    /// <summary>
    /// Allows a short (TimeLow) guid or full guid to be converted to Guid
    /// </summary>
    /// <param name="str">The input string value.</param>
    /// <returns></returns>
    public static Guid ToGuid(this string str)
    {
        if (str.Length == 8)
            return new Guid(string.Format("{0}-0000-0000-0000-000000000000", str));
        else
            return new Guid(str);
    }

    /// <summary>
    /// Justifies text.
    /// </summary>
    /// <param name="str">The input string to justify.</param>
    /// <param name="length">The length of text.</param>
    /// <param name="justification">The justification style.</param>
    /// <returns></returns>
    public static string Justify(this string str, int length, Justification justification)
    {
        if (str.Length > length)
            str = str.Substring(0, length);

        if (justification == Justification.LEFT)
            return str.PadRight(length);
        else if (justification == Justification.CENTRE)
            return (" " + str.PadRight(length / 2).PadLeft(length / 2)).PadRight(length);
        else
            return str.PadLeft(length);
    }

    /// <summary>
    /// Parses a string for arguments. Arguments can be separated by whitespace. Single or double quotes
    /// can be used to delimit fields that contain space characters.
    /// </summary>
    /// <param name="str">The input string to parse.</param>
    /// <returns></returns>
    public static string[] ParseArgs(this string str)
    {
        List<string> args = new List<string>();
        string currentArg = "";
        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(str ?? "")))
        {
            using (var sr = new StreamReader(ms))
            {
                bool inWhiteSpace = false;
                char inQuoteChar = '\0';
                char nextChar;
                while (!sr.EndOfStream)
                {
                    nextChar = (char)sr.Read();
                    if (inQuoteChar == '\0' && (nextChar == '\'' || nextChar == '"'))
                    {
                        // Start of quoted field
                        inQuoteChar = nextChar;
                        currentArg = "";
                    }
                    else if (nextChar == inQuoteChar && nextChar != '\0')
                    {
                        // End of quoted field
                        // The end of quoted field MUST be followed by whitespace.
                        args.Add(currentArg);
                        inQuoteChar = '\0';
                    }
                    else if (!inWhiteSpace && inQuoteChar == '\0' && string.IsNullOrWhiteSpace(nextChar.ToString()))
                    {
                        // Start of whitespace, not in quoted field
                        args.Add(currentArg);
                        inWhiteSpace = true;
                    }
                    else if (inWhiteSpace && inQuoteChar == '\0' && !string.IsNullOrWhiteSpace(nextChar.ToString()))
                    {
                        // Start of new argument
                        currentArg = nextChar.ToString();
                        inWhiteSpace = false;
                    }
                    else
                    {
                        currentArg += nextChar.ToString();
                    }
                }
                if (!string.IsNullOrEmpty(currentArg))
                    args.Add(currentArg);
            }
        }
        return args.ToArray();
    }

    /// <summary>
    /// Wrapper for .NET IsNullOrWhiteSpace method.
    /// </summary>
    /// <param name="str">Input value to test.</param>
    /// <returns></returns>
    public static bool IsNullOrWhiteSpace(this string str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    /// <summary>
    /// Wrapper for .NET IsNullOrEmpty method.
    /// </summary>
    /// <param name="str">input value to test.</param>
    /// <returns></returns>
    public static bool IsNullOrEmpty(this string str)
    {
        return string.IsNullOrEmpty(str);
    }

    /// <summary>
    /// Removes characters from the right end of a string
    /// </summary>
    /// <param name="str">The input string.</param>
    /// <param name="length">The required length of the string.</param>
    /// <returns></returns>
    public static string RemoveRight(this string str, int length)
    {
        return str.Remove(str.Length - length);
    }

    /// <summary>
    /// Removes characters from the left end of a string
    /// </summary>
    /// <param name="str">The input string.</param>
    /// <param name="length">The required length of the string.</param>
    /// <returns></returns>
    public static string RemoveLeft(this string str, int length)
    {
        return str.Remove(0, length);
    }

    /// <summary>
    /// Converts a string value to a stream.
    /// </summary>
    /// <param name="str">The input string.</param>
    /// <returns></returns>
    public static Stream ToStream(this string str)
    {
        MemoryStream stream = new MemoryStream();
        StreamWriter writer = new StreamWriter(stream);
        writer.Write(str);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    /// <summary>
    /// Splits a string into chunks of [length] characters. Word breaks are avoided.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static IEnumerable<string> WordWrap(this string str, int length)
    {
        if (str == null)
            throw new ArgumentNullException("s");
        if (length <= 0)
            throw new ArgumentException("Part length has to be positive.", "partLength");

        var i = 0;
        while (i < str.Length)
        {
            // remove white space at start of line
            while (i < str.Length && char.IsWhiteSpace(str[i]))
                i++;

            var j = length;   // add extra character to check white space just after line.

            while (j >= 0)
            {
                if (i + j < str.Length && char.IsWhiteSpace(str[i + j]))
                    break;
                else if (i + j == str.Length)
                    break;
                j--;
            }
            if (j <= 0 || j > length)
                j = length;

            if (i + j >= str.Length)
                j = str.Length - i;

            var result = str.Substring(i, j);
            i += j;
            yield return result;
        }
    }

    /// <summary>
    /// Converts a string to snake case.
    /// </summary>
    /// <param name="str">The input string value.</param>
    /// <returns></returns>
    public static string ToSnakeCase(this string str)
    {
        return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
    }
}
