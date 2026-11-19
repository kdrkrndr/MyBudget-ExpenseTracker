using System;
using System.Collections.Generic;
using System.Text.Json;
using MyBudget.Core;

namespace MyBudget.Data;

public class JsonExpenseStore:IExpenseStore
{
    private readonly string _path;

    public JsonExpenseStore(string path)
    {
        _path = path;
    }

    public IReadOnlyList<Expense> Load()
    {
        if (!File.Exists(_path))
        {
            return new List<Expense>();
        }

        string json = File.ReadAllText(_path);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<Expense>();
        }

        List<Expense>? expenses=JsonSerializer.Deserialize<List<Expense>>(json);

        return expenses ?? new List<Expense>();
    }

    public void Save(IEnumerable<Expense> expenses)
    {
        JsonSerializerOptions options = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        string json=JsonSerializer.Serialize(expenses, options);

        File.WriteAllText(_path, json);
    }
}