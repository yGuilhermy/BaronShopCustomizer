using System;
using BaronShopCustomizer;

static void Check(bool condition, string name)
{
    if (!condition) throw new Exception(name);
}

var items = new[] { "sword", null, "bow", "axe" };
Check(StockSelection.FindCompatible<string>(null, x => true, 0) == null, "Null pool");
Check(StockSelection.FindCompatible(Array.Empty<string>(), x => true, 0) == null, "Empty pool");
Check(StockSelection.FindCompatible(items, x => x == "staff", 0) == null, "No compatible item");
Check(StockSelection.FindCompatible(items, x => true, 1) == "bow", "Skip null entries");
Check(StockSelection.FindCompatible(items, x => x == "sword", 2) == "sword", "Wrap around");
Check(StockSelection.FindCompatible(items, x => true, -1) == "axe", "Negative offset");
Check(StockSelection.FindCompatible(items, x => true, int.MaxValue) == "axe", "Large offset");
Console.WriteLine("7 stock selection checks passed.");
