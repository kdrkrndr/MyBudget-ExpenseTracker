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
    Console.WriteLine();
    Console.WriteLine("========== Menu ==========");
    Console.WriteLine("  1) Add an expense   2) View summary   3) Set monthly budget   4) Exit");
    Console.Write(">");
    selection = Console.ReadLine();
    Console.WriteLine();

    switch (selection)
    {
        case "1": 
            AddExpense();
            break;
        case "2":
            ViewSummary();
            break;
        case "3": 
            SetBudget();
            break;
        case "4": 
            Console.WriteLine("4) Exit");
            break;
        default:  
            Console.WriteLine("Invalid selection. Please choose 1-4.");
            break;
    }
    
}
while (selection!="4" && selection != null);

void ViewSummary()
{
    Console.WriteLine();
    Console.WriteLine("========== Expense Summary ==========");
    Console.WriteLine($"Number of expenses: {expenseCount}");
    Console.WriteLine($"Total spent: {BudgetRules.FormatCurrency(totalSpent)}");

    if (expenseCount==0)
    {
        Console.WriteLine("No expenses recorded yet.");
    }
    else
    {
        decimal average = totalSpent / expenseCount;

        Console.WriteLine($"Average expense: {BudgetRules.FormatCurrency(average)}");
        Console.WriteLine($"Highest expense: {BudgetRules.FormatCurrency(highestExpense)}");

    }

    Console.WriteLine();
    Console.WriteLine("---------- Category Totals ----------");

    Console.WriteLine($"Food:          {BudgetRules.FormatCurrency(foodTotal)}");
    Console.WriteLine($"Transport:     {BudgetRules.FormatCurrency(transportTotal)}");
    Console.WriteLine($"Utilities:     {BudgetRules.FormatCurrency(utilitiesTotal)}");
    Console.WriteLine($"Entertainment: {BudgetRules.FormatCurrency(entertainmentTotal)}");
    Console.WriteLine($"Other:         {BudgetRules.FormatCurrency(otherTotal)}");

    Console.WriteLine();
    Console.WriteLine("---------- Monthly Budget ----------");

    if (monthlyBudget is decimal budget)
    {
        decimal remaining = budget - totalSpent;

        Console.WriteLine($"Monthly budget: {BudgetRules.FormatCurrency(budget)}");
        Console.WriteLine($"Remaining: {BudgetRules.FormatCurrency(remaining)}");
        Console.WriteLine($"Status: {BudgetRules.BudgetStatus(remaining, budget)}");
    }
    else
    {
        Console.WriteLine("Monthly budget has not been set.");
    }

    Console.WriteLine("=====================================");
    Console.WriteLine();
}

void SetBudget()
{
    Console.WriteLine();
    Console.WriteLine("========== Set Monthly Budget ==========");

    while (true)
    {
        Console.Write("Monthly budget: ");
        string? enteredValue = Console.ReadLine();
        if (enteredValue is null) return;

        if (!decimal.TryParse(enteredValue, out decimal budget))
        {
            Console.WriteLine("Invalid number. Please try again.");
            Console.WriteLine();
            continue;
        }

        try
        {
            budget = BudgetRules.ValidateAmount(budget);
            monthlyBudget = budget;
            decimal remaining = budget - totalSpent;

            string formattedBudget = BudgetRules.FormatCurrency(budget);

            Console.WriteLine();
            Console.WriteLine($"Budget set to : {formattedBudget}");
            //Console.WriteLine($"Monthly Budget: {budget}");            
            Console.WriteLine($"Budget: {BudgetRules.FormatCurrency(remaining)} remaining of {formattedBudget} -> {BudgetRules.BudgetStatus(remaining, budget)}");

            return;
        }
        catch (InvalidExpenseException ex)
        {
            Console.WriteLine(ex.Message);
        }
        Console.WriteLine();

    }
}

void AddExpense()
{
    Console.WriteLine("========== Add An Expense ==========");

    #region Description
    string? description;

    do
    {
        Console.WriteLine();
        Console.Write("Description:");
        description = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(description))
        {
            Console.WriteLine("Description cannot be empty");
            Console.WriteLine();

        }
    } while (string.IsNullOrWhiteSpace(description));

    string cleanedDescription = description.Trim();
    string prefix = cleanedDescription[..Math.Min(3, cleanedDescription.Length)].ToUpperInvariant();
    char lastChar = char.ToLowerInvariant(cleanedDescription[^1]);

    #endregion


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
            break;
        }
        catch (InvalidExpenseException ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally 
        {
            Console.WriteLine(); 
        }

    }

    #region Category
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
    #endregion

    #region Date
    DateOnly expenseDate;
    DateOnly today = DateOnly.FromDateTime(DateTime.Today);
    while (true)
    {
        Console.Write("Date yyyy-MM-dd (blank = today): ");
        string? date = Console.ReadLine();
        if (date is null) return;

        if (string.IsNullOrEmpty(date))
        {
            expenseDate = today;
            break;
        }

        if (!DateOnly.TryParseExact(date, "yyyy-MM-dd", out expenseDate))
        {
            Console.WriteLine("Invalid date format. Please try again.");
            continue;
        }

        if (expenseDate > today)
        {
            Console.WriteLine("Future dates are not allowed.");
            continue;
        }
        break;
    }
    #endregion

    #region OptionalNote
    Console.Write("Note (optional): ");
    string? note = Console.ReadLine()?.Trim();
    string safeNote = note ?? "";

    if (note is { Length: > 0 })
    {
        Console.WriteLine($"Note: {note}");
    } 
    #endregion

    totalSpent += amount;
    expenseCount++;
    highestExpense = Math.Max(highestExpense, amount);
    string referenceCode = $"{prefix}-{lastChar}--{expenseCount:D3}";

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

    Console.WriteLine();
    Console.WriteLine("---------- Expense ----------");
    Console.WriteLine($"Reference: {referenceCode}");
    Console.WriteLine(
    $"Recorded: {BudgetRules.FormatCurrency(amount)} | {category} | {expenseDate:yyyy-MM-dd}");

    Console.WriteLine(
        $"Size band: {BudgetRules.ClassifyAmount(amount)}");

    if (safeNote.Length > 0)
        Console.WriteLine($"Note: {safeNote}");

    if (monthlyBudget is decimal budget)
    {
        decimal remaining = budget - totalSpent;

        Console.WriteLine(
            $"Budget: {BudgetRules.FormatCurrency(remaining)} remaining of {BudgetRules.FormatCurrency(budget)} -> {BudgetRules.BudgetStatus(remaining, budget)}");
        Console.WriteLine();

    }

    Console.WriteLine();
}

