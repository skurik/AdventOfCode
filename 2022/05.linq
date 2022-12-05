<Query Kind="Program">
  <Namespace>FuncSharp</Namespace>
</Query>

void Main()
{
    var input = File.ReadLines("05_input");
    var stacks = ReadStacks(input);
    var moves = ReadMoves(input);
    
    ExecuteMoves(stacks, moves);    
    stacks.Select(s => s.Peek()).MkString().Dump();
    
    ExecuteMoves2(stacks, moves);    
    stacks.Select(s => s.Peek()).MkString().Dump();
}

public IReadOnlyList<Stack<char>> ReadStacks(IEnumerable<string> lines)
{
    var pattern = new Regex(@"^((\[[A-Z]\]|\s\s\s)\s?)+$");
    var rows = lines.Where(l => pattern.IsMatch(l));
    var matches = rows.Select(r => pattern.Match(r)).Reverse();
    
    var numberOfCrates = matches.Select(m => m.Groups[1].Captures.Count).Distinct().Single();
    var stacks = Enumerable.Range(0, numberOfCrates).Select(i => new Stack<char>(matches.Where(m => !m.Groups[1].Captures[i].Value.IsEmptyOrWhiteSpace()).Select(m => m.Groups[1].Captures[i].Value[1])));

    return stacks.ToList();
}

public IEnumerable<(int Source, int Target, int Count)> ReadMoves(IEnumerable<string> lines)
{
    var pattern = new Regex(@"^move (\d+) from (\d+) to (\d+)$");
    return lines.Select(line => pattern.Match(line)).Where(m => m.Success).Select(m => (
        Source: int.Parse(m.Groups[2].Value) - 1,
        Target: int.Parse(m.Groups[3].Value) - 1,
        Count: int.Parse(m.Groups[1].Value)
    ));
}

public void ExecuteMoves(IReadOnlyList<Stack<char>> stacks, IEnumerable<(int Source, int Target, int Count)> moves)
{
    foreach (var move in moves)
    {
        foreach (var crate in Enumerable.Range(0, move.Count))
        {
            stacks[move.Target].Push(stacks[move.Source].Pop());
        }
    }
}

public void ExecuteMoves2(IReadOnlyList<Stack<char>> stacks, IEnumerable<(int Source, int Target, int Count)> moves)
{
    foreach (var move in moves)
    {
        var workStack = new Stack<char>(Enumerable.Range(0, move.Count).Select(_ => stacks[move.Source].Pop()));
        foreach (var crate in Enumerable.Range(0, move.Count))
        {            
            stacks[move.Target].Push(workStack.Pop());
        }
    }
}