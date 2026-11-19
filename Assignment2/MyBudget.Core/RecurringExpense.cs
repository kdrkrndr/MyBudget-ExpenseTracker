using System;
using System.Collections.Generic;
using System.Text;

namespace MyBudget.Core;

public record RecurringExpense : Expense
{
    public int TimesPerMonth { get; init; }

    public RecurringExpense(
        Guid id,
        string description,
        decimal amount,
        ExpenseCategory category,
        DateOnly date,
        int timesPerMonth
    ) : base(id, description, amount, category, date)
    {
        TimesPerMonth = timesPerMonth;
    }

    public override decimal MonthlyImpact
    {
        get
        {
            return Amount * TimesPerMonth;
        }
    }

    public override string ToReportLine()
    {
        return base.ToReportLine() + $" (x{TimesPerMonth}/month)";
    }
}
