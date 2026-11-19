using System;
using System.Collections.Generic;
using System.Text;

namespace MyBudget.Core;

public class ExpenseRepository:IExpenseRepository
{
    private readonly List<Expense> _expenses;
    private readonly IExpenseStore _store; // because of the Single responsibility principle
    public ExpenseRepository(IExpenseStore store)
    {
        _store = store;
        _expenses=store.Load().ToList(); //we used ToList because we need to type casting from IReadOnlyList<Expense> to List<Expense>
    }

    public IReadOnlyList<Expense> GetAll()
    {
        return _expenses
            .OrderBy(k=>k.Date)
            .ToList();
    }
    public void Add(Expense expense)
    {
        if(expense == null) throw new ArgumentNullException(nameof(expense));
        _expenses.Add(expense);
    }
    public decimal Total()
    {
        return _expenses.Sum(k => k.MonthlyImpact);
    }
    public IReadOnlyDictionary<ExpenseCategory, decimal> TotalsByCategory()
    {
        return _expenses
            .GroupBy(e => e.Category)
            .ToDictionary(
                group => group.Key,
                group => group.Sum(e => e.MonthlyImpact)
            );
    }
    public IReadOnlyList<Expense> InCategory(ExpenseCategory category)
    {
        return _expenses
            .Where(e => e.Category == category)
            .OrderBy(e => e.Date)
            .ToList();
    }
    public void Save()
    {
        _store.Save(_expenses);
    }
}