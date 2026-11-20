using MyBudget.Core;
using MyBudget.Tests.Fakes;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MyBudget.Tests;

public class AdditionalTests
{
    [Fact]
    public void GetAll_OrdersExpensesByDate()
    {
        // Arrange
        var repo = new ExpenseRepository(new InMemoryExpenseStore());

        var later = ExpenseFactory.CreateOneTime(
            "Later",
            20m,
            ExpenseCategory.Other,
            new DateOnly(2026, 11, 20));

        var earlier = ExpenseFactory.CreateOneTime(
            "Earlier",
            10m,
            ExpenseCategory.Food,
            new DateOnly(2026, 11, 10));

        repo.Add(later);
        repo.Add(earlier);

        // Act
        var result = repo.GetAll();

        // Assert
        Assert.Equal("Earlier", result[0].Description);
        Assert.Equal("Later", result[1].Description);
    }

    [Fact]
    public void SetMonthlyLimit_RoundsToTwoDecimals()
    {
        // Arrange
        var budget = new BudgetService();

        // Act
        budget.SetMonthlyLimit(100.456m);

        // Assert
        Assert.Equal(100.46m, budget.MonthlyLimit);
    }

    [Theory]
    [InlineData("  Gym  ", "Gym")]
    [InlineData("  Bus Pass ", "Bus Pass")]
    public void CreateRecurring_TrimsDescription(
        string description,
        string expected)
    {
        // Arrange & Act
        var expense = ExpenseFactory.CreateRecurring(
            description,
            20m,
            ExpenseCategory.Transport,
            new DateOnly(2026, 11, 19),
            3);

        // Assert
        Assert.Equal(expected, expense.Description);
    }
}