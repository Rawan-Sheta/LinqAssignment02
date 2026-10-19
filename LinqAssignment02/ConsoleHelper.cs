namespace LinqAssignment02;

// Small helper methods used by the assignment file, only to print results nicely.
public static class ConsoleHelper
{
    // Prints a big title line, for example the name of an operator group.
    public static void Header(string text)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 64));
        Console.WriteLine(text);
        Console.WriteLine(new string('=', 64));
    }

    // Prints the text of one question.
    public static void Question(string text)
    {
        Console.WriteLine();
        Console.WriteLine("--- " + text);
    }

    // Prints every item of a sequence on its own line.
    //   items  : the result of a LINQ query
    //   format : (optional) says HOW to write one item. Without it, the item's ToString() is used.
    // If the sequence is empty it prints "(no results)".
    public static void Print<T>(IEnumerable<T> items, Func<T, string>? format = null)
    {
        bool any = false;

        // The query is really executed HERE (deferred execution): the foreach asks it for its items.
        foreach (T item in items)
        {
            any = true;
            Console.WriteLine("  " + (format != null ? format(item) : item?.ToString() ?? "null"));
        }

        if (!any) Console.WriteLine("  (no results)");
    }

    // Reads dictionary_english.txt into an array of strings (one word per line).
    // The file is searched next to the .exe (the csproj copies it there automatically).
    // Blank lines are removed so they do not count as words of length 0.
    // Returns null (and prints a message) if the file is missing.
    public static string[]? LoadWords()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "dictionary_english.txt");

        if (!File.Exists(path))
        {
            Console.WriteLine($"  The file was not found: {path}");
            return null;
        }

        return File.ReadAllLines(path)
                   .Select(line => line.Trim())
                   .Where(line => line.Length > 0)
                   .ToArray();
    }
}
