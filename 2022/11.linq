<Query Kind="Program">
  <Namespace>FuncSharp</Namespace>
</Query>

void Main()
{
	var monkeys = ReadMonkeys(File.ReadLines(@"c:\Users\StanislavKuřík\OneDrive\Devel\LINQPad\Queries\AdventOfCode\2022\11_input"));
	var divisorsLcm = monkeys.Select(m => m.Divisor).Multiply();

	MonkeyBusiness(LetTheMonkeysPlay(monkeys,    20, l => l / 3)).Dump();
	MonkeyBusiness(LetTheMonkeysPlay(monkeys, 10000, l => l % divisorsLcm)).Dump();
}

public int MonkeyBusiness(IDictionary<Monkey, int> inspectionCounts)
{
	return inspectionCounts.Values.OrderByDescending(v => v).Take(2).Multiply();
}

public Dictionary<Monkey, int> LetTheMonkeysPlay(List<Monkey> monkeys, int rounds, Func<long, long> worryLevelAdjuster)
{
	var inspectionCounts = new Dictionary<Monkey, int>();

	foreach (var round in Enumerable.Range(0, rounds))
	{
		foreach (var monkey in monkeys)
		{
			foreach (var item in monkey.Items)
			{			
				inspectionCounts[monkey] = inspectionCounts.Get(monkey).GetOrZero() + 1;
				var worryLevel = monkey.Operation.Evaluate(item);
				var reducedWorryLevel = worryLevelAdjuster(worryLevel);
				var targetMonkey = monkey.Dispatch(reducedWorryLevel);
								
				monkeys.ElementAt(targetMonkey).Items.Add(reducedWorryLevel);
			}
			monkey.Items.Clear();
		}
	}
	
	return inspectionCounts;
}

public List<Monkey> ReadMonkeys(IEnumerable<string> input)
{
	return input.Where(line => !line.IsEmptyOrWhiteSpace()).Chunk(6).Select(ReadMonkey).ToList();
}

public Monkey ReadMonkey(IEnumerable<string> input)
{
	var dispatch = ParseDispatch(input.Skip(3));
	
	return new Monkey(ParseItems(input.Second()), ParseOperation(input.Third()), dispatch.Item1, dispatch.Item2);
}

public List<long> ParseItems(string line)
{
	return line.Words().Skip(2).Select(long.Parse).ToList();
}

public Operation ParseOperation(string line)
{
	return line.ProcessTokens(t => t[4].Match(
		"+", _ => new Operation(new Addition(ParseOperand(t[3]), ParseOperand(t[5]))),
		"*", _ => new Operation(new Multiplication(ParseOperand(t[3]), ParseOperand(t[5])))
	));
}

public Operand ParseOperand(string operand)
{
	return operand.Match("old", _ => new Operand(), _ => new Operand(int.Parse(operand)));
}

public (Dispatch, int) ParseDispatch(IEnumerable<string> input)
{
	var divisor = int.Parse(input.First().Words().Last());
	var trueMonkey = int.Parse(input.Second().Words().Last());
	var falseMonkey = int.Parse(input.Third().Words().Last());

	return (v => (v % divisor).SafeEquals(0).Match(t => trueMonkey, f => falseMonkey), divisor);
}

public delegate int Dispatch(long currentValue);
public record Monkey(List<long> Items, Operation Operation, Dispatch Dispatch, int Divisor);

public class Operand : Coproduct2<int, Unit>
{
	public Operand(int value) : base(value) { }
	public Operand() : base(Unit.Value) { }
}

public record Addition(Operand addend1, Operand addend2)
{
	public long Evaluate(long currentValue) => addend1.Match(v => v, _ => currentValue) + addend2.Match(v => v, _ => currentValue);	
}

public record Multiplication(Operand factor1, Operand factor2)
{
	public long Evaluate(long currentValue) => factor1.Match(v => v, _ => currentValue) * factor2.Match(v => v, _ => currentValue);
}

public class Operation : Coproduct2<Addition, Multiplication>
{
	public Operation(Addition addition) : base(addition) { }
	public Operation(Multiplication multiplication) : base(multiplication) { }
	
	public long Evaluate(long currentValue) => Match(
		addition => addition.Evaluate(currentValue),
		multiplication => multiplication.Evaluate(currentValue)
	);
}