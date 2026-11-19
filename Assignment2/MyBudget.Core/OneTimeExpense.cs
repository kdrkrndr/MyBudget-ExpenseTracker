using System;
using System.Collections.Generic;
using System.Text;

namespace MyBudget.Core
{
    public record OneTimeExpense : Expense
    {
        public OneTimeExpense(
            Guid id,
            string description,
            decimal amount,
            ExpenseCategory category,
            DateOnly date
        ) : base(id, description, amount, category, date)
        {

        }

        public override decimal MonthlyImpact
        {
            get { return Amount; }
        }
    }
}
