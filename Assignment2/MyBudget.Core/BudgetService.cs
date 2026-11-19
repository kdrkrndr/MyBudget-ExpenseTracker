using System;
using System.Collections.Generic;
using System.Text;

namespace MyBudget.Core;

public class BudgetService:IBudgetService
{
    public decimal MonthlyLimit { get; private set; }
    public void SetMonthlyLimit(decimal limit)
    {
        if (limit <= 0)
        {
            throw new InvalidExpenseException("Monthly limit must be greater than zero."
            );
        }

        MonthlyLimit = Math.Round(limit, 2);
    }
    public decimal Remaining(decimal totalSpent)
    {
        return MonthlyLimit - totalSpent;
    }
    public BudgetStatus Evaluate(decimal totalSpent)
    {
        if (MonthlyLimit == 0)
        {
            return BudgetStatus.NotSet;
        }

        decimal remaining = Remaining(totalSpent);

        if (remaining < 0)
        {
            return BudgetStatus.OverBudget;
        }

        if (remaining < MonthlyLimit * 0.10m)
        {
            return BudgetStatus.AlmostOut;
        }

        return BudgetStatus.OnTrack;
    }
}

