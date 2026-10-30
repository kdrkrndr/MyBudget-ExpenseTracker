// =====================================================================
//  Program.cs  —  the interactive console UI for MyBudget (Assignment 1).
//  Target framework: .NET 10 (LTS), language C# 14.
//
//  >>> BUILD THE MENU-DRIVEN UI HERE (Modules 1-3). <<<
//
//  Once you have implemented BudgetRules.cs (so the unit tests pass), wire it
//  up to a console interface that meets the assignment brief:
//
//    * Print a banner (try a raw string literal).
//    * Loop a menu until the user exits, using a switch on the choice:
//        1) Add an expense   2) View summary   3) Set monthly budget   4) Exit
//    * Read and VALIDATE input, re-prompting on bad data (decimal.TryParse,
//      BudgetRules.NormalizeCategory, a date parse, non-empty text).
//    * Keep running totals in simple variables (no collections / no classes).
//    * Use BudgetRules.ValidateAmount / ClassifyAmount / BudgetStatus /
//      FormatCurrency for all logic and formatting.
//    * Handle bad input with try / catch / finally and InvalidExpenseException.
//
//  See section 6 of the assignment brief for a sample run to aim for.
// =====================================================================
using ExpenseTracker;

// Delete the line above and implement the application here.

Console.WriteLine("""
    **********************************************
             MyBudget Expense Tracker
    **********************************************
    """);


string? selection;
decimal? monthlyBudget = null;
decimal totalSpent = 0m;
int expenseCount = 0;
decimal highestExpense = 0m;

decimal foodTotal = 0m;
decimal transportTotal = 0m;
decimal utilitiesTotal = 0m;
decimal entertainmentTotal = 0m;
decimal otherTotal = 0m;

do
{
    Console.WriteLine("  1) Add an expense   2) View summary   3) Set monthly budget   4) Exit");
    Console.Write(">");
    selection = Console.ReadLine();
    switch (selection)
    {
        case "1": 
            AddExpense();
            break;
        case "2":
            Console.WriteLine("2) View summary"); 
            break;
        case "3": 
            monthlyBudget= SetBudget();
            break;
        case "4": Console.WriteLine("4) Exit");
            break;
        default:  Console.WriteLine("Invalid selection. Please choose 1-4.");
            break;
    }
    
}
while (selection!="4" && selection != null);

static decimal SetBudget()
{
    Console.WriteLine("3) Set monthly budget");

    while (true)
    {
        Console.Write("Enter monthly budget: ");
        string? enteredValue = Console.ReadLine();

        if (!decimal.TryParse(enteredValue, out decimal budget))
        {
            Console.WriteLine("Invalid number. Please try again.");
            continue;
        }

        try
        {
            budget = BudgetRules.ValidateAmount(budget);
            string formattedBudget = BudgetRules.FormatCurrency(budget);
            //decimal remaining = monthlyBudget - totalSpent;
            //string status = BudgetRules.BudgetStatus(remaining, monthlyBudget);

            Console.WriteLine($"Monthly Budget: {budget}");
            Console.WriteLine($"Budget set to : {formattedBudget}");
            Console.WriteLine($"Budget: {formattedBudget} remaining of {formattedBudget} -> {BudgetRules.BudgetStatus(budget, budget)}");

            return budget;
        }
        catch (InvalidExpenseException ex)
        {
            Console.WriteLine(ex.Message);
        }



    }
}

 void AddExpense()
{
    Console.WriteLine("1) Add an expense");

    string? description;

    do
    {
        Console.WriteLine("Description:");
        description = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(description))
        {
            Console.WriteLine("Description cannot be empty");
        } 
    } while (string.IsNullOrWhiteSpace(description));

    decimal amount;

    while (true)
    {
        Console.Write("Amount: ");
        string? enteredValue = Console.ReadLine();

        if (!decimal.TryParse(enteredValue, out decimal enteredAmount))
        {
            Console.WriteLine("Invalid number. Please try again.");
            continue;
        }

        try
        {
            amount = BudgetRules.ValidateAmount(enteredAmount);
            Console.WriteLine($"Amount: {amount}");

            break;
        }
        catch (InvalidExpenseException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    string? category;
    do
    {
        Console.Write("Category [Food/Transport/Utilities/Entertainment/Other]: ");

        string? input = Console.ReadLine();

        category = BudgetRules.NormalizeCategory(input);

        if (category is null)
        {
            Console.WriteLine("Invalid category. Please try again.");
        }

    } while (category is null);


    DateOnly expenseDate;
    Console.WriteLine("Date yyyy-MM-dd (blank = today): ");
    string date= Console.ReadLine();
    while (true)
    {
        if (string.IsNullOrEmpty(date))
        {
            expenseDate = DateOnly.FromDateTime(DateTime.Today);
            break;
        }

        if (!DateOnly.TryParseExact(date, "yyyy-MM-dd", out expenseDate))
        {
            Console.WriteLine("Invalid date format. Please try again.");
            continue;
        }

        if (expenseDate > DateOnly.FromDateTime(DateTime.Today))
        {
            Console.WriteLine("Future dates are not allowed.");
            continue;
        }
        break;
    }

    Console.Write("Note (optional): ");
    string? note = Console.ReadLine()?.Trim();

    if (note is { Length: > 0 })
    {
        Console.WriteLine($"Note: {note}");
    }

    totalSpent += amount;
    expenseCount++;
    highestExpense = Math.Max(highestExpense, amount);

    switch (category)
    {
        case "Food":
            foodTotal += amount;
            break;
        case "Transport":
            transportTotal += amount;
            break;
        case "Utilities":
            utilitiesTotal += amount;
            break;
        case "Entertainment":
            entertainmentTotal += amount;
            break;
        case "Other":
            otherTotal += amount;
            break;
    }

    Console.WriteLine(
    $"Recorded: {BudgetRules.FormatCurrency(amount)} | {category} | {expenseDate:yyyy-MM-dd}");

    Console.WriteLine(
        $"Size band: {BudgetRules.ClassifyAmount(amount)}");

    if (note.Length > 0)
        Console.WriteLine($"Note: {note}");

    if (monthlyBudget is decimal budget)
    {
        decimal remaining = budget - totalSpent;

        Console.WriteLine(
            $"Budget: {BudgetRules.FormatCurrency(remaining)} remaining of {BudgetRules.FormatCurrency(budget)} -> {BudgetRules.BudgetStatus(remaining, budget)}");
    }

    Console.WriteLine();
}
