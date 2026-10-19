using Day_01_G03;

namespace LinqAssignment02;

// =============================================================================
// LINQ ASSIGNMENT 02
// =============================================================================
// The 4 ideas used everywhere in this file:
//
//  1. LAMBDA EXPRESSION:   p => p.UnitsInStock == 0   ("for each p, give me ...")
//  2. DEFERRED EXECUTION:  a query runs when we loop over its result, not when we write it.
//  3. ANONYMOUS TYPE:      new { Name = "Ali", Age = 20 }  (a small object without a class name)
//  4. GROUPING:            GroupBy(key) splits a sequence into groups; every group has a .Key
//                          and contains its items, so we can loop over it or aggregate it.
// =============================================================================
public static class Assignment02
{
    // The numbers array used by several questions.
    private static readonly int[] Numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

    // Shows the menu and runs the group of questions the user chooses.
    // It repeats until the user types 0.
    public static void Run()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("===== LINQ Assignment 02 =====");
            Console.WriteLine("1 - Element operators");
            Console.WriteLine("2 - Aggregate operators (1-14)");
            Console.WriteLine("3 - Set operators");
            Console.WriteLine("4 - Partitioning operators");
            Console.WriteLine("5 - Quantifiers");
            Console.WriteLine("6 - Grouping operators");
            Console.WriteLine("0 - Exit");
            Console.Write("Your choice: ");

            string choice = Console.ReadLine() ?? "";
            if (choice == "0") return;

            switch (choice)
            {
                case "1": Element(); break;
                case "2": AggregateBasics(); AggregateAdvanced(); break;
                case "3": SetOperators(); break;
                case "4": Partitioning(); break;
                case "5": Quantifiers(); break;
                case "6": Grouping(); break;
                default: Console.WriteLine("Invalid choice."); break;
            }
        }
    }

    // =====================================================================
    // LINQ - ELEMENT OPERATORS
    // Element operators return ONE item (not a sequence): First, FirstOrDefault, ElementAt.
    // =====================================================================
    public static void Element()
    {
        ConsoleHelper.Header("LINQ - Element Operators");
        var products = ListGenerator.ProductsList;

        // QUESTION 1: Get the first Product out of stock.
        // HOW: First() would THROW an exception if no product matched.
        //      FirstOrDefault() returns null instead, so it is the safe choice.
        //      "?." reads ProductName only if the product is not null.
        ConsoleHelper.Question("1. The first product that is out of stock");
        Product? firstOutOfStock = products.FirstOrDefault(p => p.UnitsInStock == 0);
        Console.WriteLine("  " + (firstOutOfStock?.ProductName ?? "null"));

        // QUESTION 2: Return the first product whose Price > 1000, or null if there is none.
        // HOW: FirstOrDefault. In this data no product costs more than 1000, so the answer is null.
        ConsoleHelper.Question("2. The first product with Price > 1000, or null if there is none");
        Product? expensive = products.FirstOrDefault(p => p.UnitPrice > 1000);
        Console.WriteLine("  " + (expensive?.ProductName ?? "null"));

        // QUESTION 3: Retrieve the second number greater than 5.
        // HOW: Where keeps the numbers greater than 5 in their original order: 9, 8, 6, 7.
        //      ElementAt(1) takes the item at index 1 (the second one), which is 8.
        ConsoleHelper.Question("3. The second number greater than 5");
        int second = Numbers.Where(n => n > 5).ElementAt(1);
        Console.WriteLine("  " + second);
    }

    // =====================================================================
    // LINQ - AGGREGATE OPERATORS 1-8
    // Aggregate operators squeeze a whole sequence into ONE value:
    // Count, Sum, Min, Max, Average.
    // =====================================================================
    public static void AggregateBasics()
    {
        ConsoleHelper.Header("LINQ - Aggregate Operators 1-8");
        var products = ListGenerator.ProductsList;
        var customers = ListGenerator.CustomersList;

        // QUESTION 1: Get the number of odd numbers in the array.
        // HOW: Count(condition) counts only the items that satisfy the condition.
        //      n % 2 is the remainder of dividing by 2: 1 for odd numbers, 0 for even numbers.
        ConsoleHelper.Question("1. Number of odd numbers in the array");
        Console.WriteLine("  " + Numbers.Count(n => n % 2 != 0));

        // QUESTION 2: Return a list of customers and how many orders each has.
        // HOW: Select builds, for each customer, an anonymous object with the customer id
        //      and the number of his orders (Orders.Count()).
        ConsoleHelper.Question("2. Customers and how many orders each has");
        var ordersPerCustomer = customers.Select(c => new { c.CustomerID, OrdersCount = c.Orders.Count() });
        ConsoleHelper.Print(ordersPerCustomer, x => $"{x.CustomerID}: {x.OrdersCount} orders");

        // QUESTION 3: Return a list of categories and how many products each has.
        // HOW: GroupBy puts the products that have the same Category in one group.
        //      For every group, g.Key is the category name and g.Count() is the number of its products.
        ConsoleHelper.Question("3. Categories and how many products each has");
        var productsPerCategory = products
            .GroupBy(p => p.Category)
            .Select(g => new { Category = g.Key, ProductsCount = g.Count() });
        ConsoleHelper.Print(productsPerCategory, x => $"{x.Category}: {x.ProductsCount} products");

        // QUESTION 4: Get the total of the numbers in the array.
        // HOW: Sum adds all the items: 5+4+1+3+9+8+6+7+2+0 = 45.
        ConsoleHelper.Question("4. Total of the numbers in the array");
        Console.WriteLine("  " + Numbers.Sum());

        // Questions 5-8 work on the words of dictionary_english.txt.
        // We read the file into an array of strings first (one word per element).
        string[]? words = ConsoleHelper.LoadWords();
        if (words is null) return;

        // QUESTION 5: Total number of characters of all words in the file.
        // HOW: Sum with a lambda: for each word we add its Length.
        ConsoleHelper.Question("5. Total number of characters of all words in dictionary_english.txt");
        Console.WriteLine("  " + words.Sum(w => w.Length));

        // QUESTION 6: Length of the shortest word.
        // HOW: Min over the lengths of the words.
        ConsoleHelper.Question("6. Length of the shortest word");
        Console.WriteLine("  " + words.Min(w => w.Length));

        // QUESTION 7: Length of the longest word.
        // HOW: Max over the lengths of the words.
        ConsoleHelper.Question("7. Length of the longest word");
        Console.WriteLine("  " + words.Max(w => w.Length));

        // QUESTION 8: Average length of the words.
        // HOW: Average over the lengths. ":F2" prints it with 2 digits after the decimal point.
        ConsoleHelper.Question("8. Average length of the words");
        Console.WriteLine($"  {words.Average(w => w.Length):F2}");
    }

    // =====================================================================
    // LINQ - AGGREGATE OPERATORS 9-14
    // The idea: GroupBy(category) first, then aggregate INSIDE every group.
    // =====================================================================
    public static void AggregateAdvanced()
    {
        ConsoleHelper.Header("LINQ - Aggregate Operators 9-14");
        var products = ListGenerator.ProductsList;

        // QUESTION 9: Total units in stock for each category.
        // HOW: For every category group, g.Sum(...) adds the UnitsInStock of its products.
        ConsoleHelper.Question("9. Total units in stock for each category");
        var unitsPerCategory = products
            .GroupBy(p => p.Category)
            .Select(g => new { Category = g.Key, TotalUnits = g.Sum(p => p.UnitsInStock) });
        ConsoleHelper.Print(unitsPerCategory, x => $"{x.Category}: {x.TotalUnits}");

        // QUESTION 10: The cheapest price among each category's products.
        // HOW: g.Min(...) gives the smallest UnitPrice inside the group.
        ConsoleHelper.Question("10. The cheapest price in each category");
        var cheapestPrice = products
            .GroupBy(p => p.Category)
            .Select(g => new { Category = g.Key, CheapestPrice = g.Min(p => p.UnitPrice) });
        ConsoleHelper.Print(cheapestPrice, x => $"{x.Category}: {x.CheapestPrice:F2}");

        // QUESTION 11: The products with the cheapest price in each category (use let).
        // HOW: "let minPrice = ..." stores a value inside the query so we can reuse it.
        //      First we find the cheapest price of the group, then we keep the products of the
        //      group whose price EQUALS it (there can be more than one product with that price).
        ConsoleHelper.Question("11. The products with the cheapest price in each category (using let)");
        var cheapestProducts = from p in products
                               group p by p.Category into g
                               let minPrice = g.Min(p => p.UnitPrice)
                               select new
                               {
                                   Category = g.Key,
                                   CheapestProducts = g.Where(p => p.UnitPrice == minPrice)
                               };
        foreach (var item in cheapestProducts)
        {
            Console.WriteLine($"  {item.Category}: {string.Join(", ", item.CheapestProducts.Select(p => $"{p.ProductName} ({p.UnitPrice:F2})"))}");
        }

        // QUESTION 12: The most expensive price among each category's products.
        // HOW: g.Max(...) gives the largest UnitPrice inside the group.
        ConsoleHelper.Question("12. The most expensive price in each category");
        var mostExpensivePrice = products
            .GroupBy(p => p.Category)
            .Select(g => new { Category = g.Key, MostExpensivePrice = g.Max(p => p.UnitPrice) });
        ConsoleHelper.Print(mostExpensivePrice, x => $"{x.Category}: {x.MostExpensivePrice:F2}");

        // QUESTION 13: The products with the most expensive price in each category (use let).
        // HOW: Same idea as question 11, with Max instead of Min.
        ConsoleHelper.Question("13. The products with the most expensive price in each category (using let)");
        var mostExpensiveProducts = from p in products
                                    group p by p.Category into g
                                    let maxPrice = g.Max(p => p.UnitPrice)
                                    select new
                                    {
                                        Category = g.Key,
                                        MostExpensiveProducts = g.Where(p => p.UnitPrice == maxPrice)
                                    };
        foreach (var item in mostExpensiveProducts)
        {
            Console.WriteLine($"  {item.Category}: {string.Join(", ", item.MostExpensiveProducts.Select(p => $"{p.ProductName} ({p.UnitPrice:F2})"))}");
        }

        // QUESTION 14: The average price of each category's products.
        // HOW: g.Average(...) gives the mean UnitPrice inside the group.
        ConsoleHelper.Question("14. The average price of each category's products");
        var averagePrice = products
            .GroupBy(p => p.Category)
            .Select(g => new { Category = g.Key, AveragePrice = g.Average(p => p.UnitPrice) });
        ConsoleHelper.Print(averagePrice, x => $"{x.Category}: {x.AveragePrice:F2}");
    }

    // =====================================================================
    // LINQ - SET OPERATORS
    // They treat sequences like mathematical sets:
    //   Distinct  = remove duplicates          Union     = A or B (no duplicates)
    //   Intersect = in A and in B              Except    = in A but not in B
    //   Concat    = A followed by B (KEEPS the duplicates)
    // =====================================================================
    public static void SetOperators()
    {
        ConsoleHelper.Header("LINQ - Set Operators");
        var products = ListGenerator.ProductsList;
        var customers = ListGenerator.CustomersList;

        // QUESTION 1: Find the unique Category names from the product list.
        // HOW: Select takes every product's category, Distinct removes the repeated names.
        ConsoleHelper.Question("1. The unique category names");
        ConsoleHelper.Print(products.Select(p => p.Category).Distinct());

        // QUESTION 2: The unique first letters from both product names and customer names.
        // HOW: A string can be read like an array of chars, so p.ProductName[0] is its first letter.
        //      Union merges the two sequences of letters and removes the duplicates.
        ConsoleHelper.Question("2. The unique first letters of product names AND customer names");
        var productLetters = products.Select(p => p.ProductName[0]);
        var customerLetters = customers.Select(c => c.CustomerName[0]);
        ConsoleHelper.Print(productLetters.Union(customerLetters));

        // QUESTION 3: The first letters that appear in BOTH product names and customer names.
        // HOW: Intersect keeps only the letters that exist in the two sequences.
        ConsoleHelper.Question("3. The first letters that appear in BOTH product and customer names");
        ConsoleHelper.Print(productLetters.Intersect(customerLetters));

        // QUESTION 4: First letters of product names that are NOT first letters of customer names.
        // HOW: Except keeps the letters of the first sequence that do not exist in the second.
        ConsoleHelper.Question("4. First letters of product names that are NOT first letters of customer names");
        ConsoleHelper.Print(productLetters.Except(customerLetters));

        // QUESTION 5: The last three characters of every customer name and product name
        //             (duplicates must stay).
        // HOW: Concat puts the two lists of names one after the other WITHOUT removing duplicates.
        //      Then Select takes the last 3 characters of each name with Substring
        //      (names shorter than 3 characters are kept as they are, to avoid an error).
        ConsoleHelper.Question("5. The last three characters of every customer and product name (duplicates kept)");
        var allNames = products.Select(p => p.ProductName)
                               .Concat(customers.Select(c => c.CustomerName));
        var lastThree = allNames.Select(name => name.Length >= 3 ? name.Substring(name.Length - 3) : name);
        ConsoleHelper.Print(lastThree);
    }

    // =====================================================================
    // LINQ - PARTITIONING OPERATORS
    // They cut a sequence into parts: Take(n), Skip(n), TakeWhile(condition), SkipWhile(condition).
    // =====================================================================
    public static void Partitioning()
    {
        ConsoleHelper.Header("LINQ - Partitioning Operators");
        var customers = ListGenerator.CustomersList;

        // The orders of the customers in Washington (Region "WA"), as one flat sequence.
        // The two "from" lines mean: for every customer ... for every order of that customer.
        var washingtonOrders = from c in customers
                               where c.Region == "WA"
                               from o in c.Orders
                               select new { c.CustomerID, o.OrderID, o.OrderDate };

        // QUESTION 1: Get the first 3 orders from customers in Washington.
        // HOW: Take(3) keeps only the first three items.
        ConsoleHelper.Question("1. The first 3 orders from customers in Washington");
        ConsoleHelper.Print(washingtonOrders.Take(3),
            x => $"{x.CustomerID} | order {x.OrderID} | {x.OrderDate:yyyy-MM-dd}");

        // QUESTION 2: Get all but the first 2 orders from customers in Washington.
        // HOW: Skip(2) throws away the first two items and keeps the rest.
        ConsoleHelper.Question("2. All but the first 2 orders from customers in Washington");
        ConsoleHelper.Print(washingtonOrders.Skip(2),
            x => $"{x.CustomerID} | order {x.OrderID} | {x.OrderDate:yyyy-MM-dd}");

        // QUESTION 3: Return elements starting from the beginning of the array until a number
        //             is hit that is less than its position in the array.
        // HOW: TakeWhile takes items while the condition is true and STOPS at the first false.
        //      numbers = 5, 4, 1, 3, ...   (5 >= 0 yes, 4 >= 1 yes, 1 >= 2 NO -> stop)
        //      Result: 5, 4
        ConsoleHelper.Question("3. Elements from the beginning until a number is less than its position");
        ConsoleHelper.Print(Numbers.TakeWhile((n, index) => n >= index));

        // QUESTION 4: Get the elements of the array starting from the first element divisible by 3.
        // HOW: SkipWhile throws items away while the condition is true, and from the first false
        //      it keeps EVERYTHING that follows. The condition is "not divisible by 3".
        //      Result: 3, 9, 8, 6, 7, 2, 0
        ConsoleHelper.Question("4. Elements starting from the first element divisible by 3");
        ConsoleHelper.Print(Numbers.SkipWhile(n => n % 3 != 0));

        // QUESTION 5: Get the elements starting from the first element less than its position.
        // HOW: SkipWhile again, skipping while "n >= index". It stops skipping at 1 (index 2).
        //      Result: 1, 3, 9, 8, 6, 7, 2, 0
        ConsoleHelper.Question("5. Elements starting from the first element less than its position");
        ConsoleHelper.Print(Numbers.SkipWhile((n, index) => n >= index));
    }

    // =====================================================================
    // LINQ - QUANTIFIERS
    // They answer yes/no questions about a whole sequence:
    //   Any(condition) = is there at least ONE item that satisfies it?
    //   All(condition) = do ALL the items satisfy it?
    // =====================================================================
    public static void Quantifiers()
    {
        ConsoleHelper.Header("LINQ - Quantifiers");
        var products = ListGenerator.ProductsList;

        // QUESTION 1: Determine if any of the words in dictionary_english.txt contain the substring 'ei'.
        // HOW: Any returns true as soon as it finds one word with "ei" inside it.
        ConsoleHelper.Question("1. Does any word in dictionary_english.txt contain the substring 'ei'?");
        string[]? words = ConsoleHelper.LoadWords();
        if (words is not null)
        {
            Console.WriteLine("  " + words.Any(w => w.Contains("ei")));
        }

        // QUESTION 2: A grouped list of products only for categories that have at least one
        //             product that is out of stock.
        // HOW: GroupBy(category) makes the groups. Where on the GROUPS keeps a group only if
        //      Any of its products has no stock.
        ConsoleHelper.Question("2. Products grouped by category, only categories with at least one product out of stock");
        var someOutOfStock = products
            .GroupBy(p => p.Category)
            .Where(g => g.Any(p => p.UnitsInStock == 0));
        PrintGroups(someOutOfStock);

        // QUESTION 3: A grouped list of products only for categories whose products are ALL in stock.
        // HOW: Same idea, with All: every product of the group must have stock greater than 0.
        ConsoleHelper.Question("3. Products grouped by category, only categories whose products are ALL in stock");
        var allInStock = products
            .GroupBy(p => p.Category)
            .Where(g => g.All(p => p.UnitsInStock > 0));
        PrintGroups(allInStock);
    }

    // Prints groups of products: the category name, then its products with their stock.
    private static void PrintGroups(IEnumerable<IGrouping<string, Product>> groups)
    {
        foreach (var group in groups)
        {
            Console.WriteLine($"  {group.Key}:");
            foreach (Product p in group)
            {
                Console.WriteLine($"      {p.ProductName} (stock {p.UnitsInStock})");
            }
        }
    }

    // =====================================================================
    // LINQ - GROUPING OPERATORS
    // GroupBy(key) splits a sequence into groups of items that have the same key.
    // =====================================================================
    public static void Grouping()
    {
        ConsoleHelper.Header("LINQ - Grouping Operators");

        // QUESTION 1: Use GroupBy to partition a list of numbers by their remainder when
        //             divided by 5.
        // HOW: The key is "n % 5" (0, 1, 2, 3 or 4). All the numbers with the same remainder
        //      land in the same group. OrderBy(g => g.Key) prints the groups in order of the key.
        ConsoleHelper.Question("1. Numbers grouped by their remainder when divided by 5");
        var numbers = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        foreach (var group in numbers.GroupBy(n => n % 5).OrderBy(g => g.Key))
        {
            Console.WriteLine($"  Numbers with a remainder of {group.Key} when divided by 5:");
            foreach (int n in group) Console.WriteLine("    " + n);
        }

        // QUESTION 2: Use GroupBy to partition the words of dictionary_english.txt by their
        //             first letter.
        // HOW: The key is the first character of the word (w[0]).
        //      A real dictionary is huge, so for every letter we print only the letter, how many
        //      words it has, and the first 5 words (Take(5)).
        ConsoleHelper.Question("2. Words of dictionary_english.txt grouped by their first letter");
        string[]? words = ConsoleHelper.LoadWords();
        if (words is not null)
        {
            foreach (var group in words.GroupBy(w => w[0]).OrderBy(g => g.Key))
            {
                Console.WriteLine($"  {group.Key}: {group.Count()} words (e.g. {string.Join(", ", group.Take(5))})");
            }
        }

        // QUESTION 3: Use a custom comparer so GroupBy groups together the words that are made
        //             of the same characters (anagrams).
        // HOW: GroupBy accepts a comparer object that decides when two keys are "the same".
        //      Our AnagramComparer (at the end of this file) says that "from" and "form" are the
        //      same because they have the same letters. Note that " last" starts with a space:
        //      the comparer ignores spaces, so it still matches "salt".
        ConsoleHelper.Question("3. Group words made of the same characters together (custom comparer)");
        string[] arr = { "from", "salt", "earn", " last", "near", "form" };
        foreach (var group in arr.GroupBy(w => w, new AnagramComparer()))
        {
            Console.WriteLine("  " + string.Join(", ", group.Select(w => $"\"{w}\"")));
        }
    }
}

// =============================================================================
// CUSTOM COMPARER (used by Grouping question 3)
// =============================================================================
// Two words are "equal" when they are made of the same characters (anagrams), ignoring
// spaces and upper/lower case.
//
// GroupBy uses BOTH methods below:
//   GetHashCode : finds the "bucket" of a word quickly
//   Equals      : confirms that two words of the same bucket really match
// So they must agree: words that are Equal MUST produce the same hash code.
// We make sure of it by using the same Normalize() in both.
public class AnagramComparer : IEqualityComparer<string>
{
    // Are the two words anagrams?
    public bool Equals(string? x, string? y)
    {
        return Normalize(x) == Normalize(y);
    }

    // Hash code of the normalized word, so anagrams get the same hash code.
    public int GetHashCode(string obj)
    {
        return Normalize(obj).GetHashCode();
    }

    // Removes the spaces, makes the letters lowercase and sorts them.
    //   "from"  -> "fmor"        "form"  -> "fmor"     (same result = anagrams)
    //   " last" -> "alst"        "salt"  -> "alst"
    private static string Normalize(string? word)
    {
        char[] letters = (word ?? string.Empty)
            .Where(c => !char.IsWhiteSpace(c))
            .Select(c => char.ToLower(c))
            .OrderBy(c => c)
            .ToArray();
        return new string(letters);
    }
}
