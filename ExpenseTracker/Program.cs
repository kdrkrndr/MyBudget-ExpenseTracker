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


do
{
    Console.WriteLine("  1) Add an expense   2) View summary   3) Set monthly budget   4) Exit");
    selection = Console.ReadLine();
    switch (selection)
    {
        case "1": Console.WriteLine("1) Add an expense");
            break;
        case "2": Console.WriteLine("2) View summary"); 
            break;
        case "3": SetBudget();
            break;
        case "4": Console.WriteLine("4) Exit");
            break;
        default:  Console.WriteLine("Invalid selection. Please choose 1-4.");
            break;
    }
    ;
}
while (selection!="4" && selection != null);

static void SetBudget()
{
    Console.WriteLine("3) Set monthly budget");

    string enteredValue= Console.ReadLine();

    if (decimal.TryParse(enteredValue, out decimal budget))
    { 
        BudgetRules.ValidateAmount(budget);
        string formattedBudget = BudgetRules.FormatCurrency(budget);

        Console.WriteLine($"Monthly Budget: {budget}");
        Console.WriteLine($"Budget set to : {formattedBudget}");
        Console.WriteLine($"Budget: {formattedBudget} reamining of {formattedBudget} -> {BudgetRules.BudgetStatus(budget,budget)}");

    }
};
