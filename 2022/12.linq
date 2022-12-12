<Query Kind="Program">
  <Namespace>FuncSharp</Namespace>
</Query>

#load ".\Vector"

void Main()
{
    var map = ReadMap(File.ReadLines("12_input"));        
    var startingPositions = map.Grid.Traverse().Where(t => map.Grid[t.Row, t.Column] == 0).Select(t => new Vector(t.Row, t.Column));

    ShortestPath(map, map.Start.ToEnumerable()).Dump();
    ShortestPath(map, startingPositions).Dump();
}

public int ShortestPath(Map map, IEnumerable<Vector> startingPositions)
{
    var state = new State(new HashSet<Vector>(), new Queue<Step>(startingPositions.Select(p => new Step(p, 0))));
    var shortestPathLength = int.MaxValue;

    while (state.Queue.NonEmpty())
    {
        var (newState, pathLength) = Visit(map, state, state.Queue.Dequeue());
        pathLength.Match(
            length => shortestPathLength = shortestPathLength.SafeMin(length)
        );

        state = newState;
    }

    return shortestPathLength;
}

public Map ReadMap(IEnumerable<string> input)
{
    var start = null as Vector;
    var end = null as Vector;
    var grid = input.Traverse((symbol, row, col) =>	symbol.Match(
        'S', _ => { start = new Vector(row, col); return 0; },
        'E', _ => { end = new Vector(row, col); return (int)('z' - 'a'); },
        _ => (int)(symbol - 'a')
    ));
    
    return new Map(grid, start, end);
}

public (State newState, IOption<int> pathLength) Visit(Map map, State state, Step next)
{	
    var visited = state.Visited.Append(next.Position).ToHashSet();

    if (next.Position.Equals(map.End))
        return (new State(visited, state.Queue), next.PathLength.ToOption());

    if (state.Visited.Contains(next.Position))
        return (new State(visited, state.Queue), Option.Empty<int>());

    var destinations = map.Destinations(next.Position);
	    
    return (new State(visited, state.Queue.Enqueue(destinations.Select(p => new Step(p, next.PathLength + 1)))), Option.Empty<int>());
}

public record Map(int[,] Grid, Vector Start, Vector End)
{
    public IEnumerable<Vector> Destinations(Vector position) =>
        position.PerpendicularNeighbors.Where(p => p.IsWithin(Grid.GetLength(0), Grid.GetLength(1)) && Grid[p.X, p.Y] <= (Grid[position.X, position.Y] + 1));
}

public record State(HashSet<Vector> Visited, Queue<Step> Queue);
public record Step(Vector Position, int PathLength);