<Query Kind="Program">
  <Namespace>FuncSharp</Namespace>
</Query>

void Main()
{
    var rucksacks = File.ReadLines("03_input").Select(GetRucksack);
    rucksacks.Sum(r => r.TotalPriorityOfErrors).Dump();
    
    GetGroups(rucksacks).Select(Intersection).Sum(PriorityOf).Dump();
}

public Rucksack GetRucksack(string items)
{
    var half = items.Length / 2;
    return new Rucksack
    {
        First = GetCompartment(items.Take(half)),
        Second = GetCompartment(items.Skip(half))
    };
}

public Compartment GetCompartment(IEnumerable<char> items)
{
    return new Compartment { Items = new HashSet<char>(items) };
}

public static int PriorityOf(char item)
{
    var adjustment = char.IsUpper(item).Match(t => 59, f => 1);
    return ((int)item - 'a') + adjustment;
}

public IEnumerable<IEnumerable<Rucksack>> GetGroups(IEnumerable<Rucksack> rucksacks)
{
    return rucksacks.Select((r, idx) => (rucksack: r, index: idx)).GroupBy(t => t.index / 3).Select(g => g.Select(r => r.rucksack));
}

public char Intersection(IEnumerable<Rucksack> rucksacks)
{
    return rucksacks.Aggregate(rucksacks.First().AllItems, (current, next) => current.Intersect(next.AllItems)).Single();    
}

public class Rucksack
{
    public Compartment First { get; set; }
    public Compartment Second { get; set; }
    
    public int TotalPriorityOfErrors => First.Items.Intersect(Second.Items).Select(PriorityOf).Sum();
    
    public IEnumerable<char> AllItems => First.Items.Union(Second.Items);
}

public class Compartment
{
    public HashSet<char> Items { get; set; }
}