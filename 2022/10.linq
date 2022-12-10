<Query Kind="Program">
  <Namespace>FuncSharp</Namespace>
</Query>

void Main()
{    
    var commands = ReadCommands(File.ReadLines("10_input"));
    var result = Execute(commands);
    
    SignalStrengths(result.cycleValues, 20, 60, 100, 140, 180, 220).Sum().Dump();
    
    var screen = new Screen(6, 40);
    screen.Draw((cpuCycle, row, col) => 
    {        
        var spritePosition = result.cycleValues.ElementAt(cpuCycle);
        return Math.Abs(col - spritePosition).PreceedsOrEquals(1).Match(t => '#', f => '.');
    });
    screen.Display.Dump();
}

public IEnumerable<Command> ReadCommands(IEnumerable<string> input)
{
    return input.Select(line => line.ProcessParts(' ', (cmd, arg) => cmd.Match(
        "noop", _ => new Command(new Noop()),
        "addx", _ => new Command(new Addx(int.Parse(arg)))
    )));
}

public State Execute(IEnumerable<Command> commands)
{
    return commands.Aggregate(
        new State(Enumerable.Empty<int>(), 1),
        (current, cmd) => cmd.Match(
            noop => current.Modify(v => v, (cycles, v) => cycles.Append(v)),
            addx => current.Modify(v => v + addx.value, (cycles, v) => cycles.Append(v).Append(v))
        )
    );
}

public IEnumerable<int> SignalStrengths(IEnumerable<int> cycleValues, params int[] cycles)
{
    return cycles.Select(c => cycleValues.ElementAt(c - 1) * c);
}

public record Noop;
public record Addx(int value);
public record State(IEnumerable<int> cycleValues, int currentValue)
{
    public State Modify(Func<int, int> valueGetter, Func<IEnumerable<int>, int, IEnumerable<int>> cycleHistoryGetter) =>
        new State(cycleHistoryGetter(cycleValues, currentValue), valueGetter(currentValue));
}

public record Screen
{
    public Screen(int height, int width)
    {
        Display = new char[height, width];
    }

    public void Draw(Func<int, int, int, char> renderer)
    {
        foreach (var cycle in Enumerable.Range(0, Display.GetLength(0) * Display.GetLength(1)))
        {
            var row = cycle / Display.GetLength(1);
            var col = cycle % Display.GetLength(1);
            
            Display[row, col] = renderer(cycle, row, col);
        }
    }
        
    public char[,] Display;
}

public class Command : Coproduct2<Noop, Addx>
{
    public Command(Noop cmd) : base(cmd) { }
    public Command(Addx cmd) : base(cmd) { }
}

