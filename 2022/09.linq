<Query Kind="Program">
  <Namespace>FuncSharp</Namespace>
</Query>

void Main()
{    
    var moves = ReadMoves(File.ReadLines("09_input"));
    var finalState = moves.Aggregate(State.Initial, Move);
    
    finalState.PositionsVisitedByTail.Dump();
}

public IEnumerable<Vector> ReadMoves(IEnumerable<string> input)
{
    return input.SelectMany(line => line.ProcessParts(' ', (d, n) => Enumerable.Repeat(Directions[d[0]], int.Parse(n))));
}

public State Move(State state, Vector move)
{    
    var newHead = state.head + move;
    var newKnots = state.knots.Aggregate(Enumerable.Empty<Vector>(), (knotsMoved, knot) => knotsMoved.Append(MoveTowards(knot, knotsMoved.LastOrDefault() ?? newHead)).AsStrict());
    
    return new State(newHead, newKnots, state.tailLog.Append(newKnots.Last()).ToHashSet());
}

public Vector MoveTowards(Vector follower, Vector master)
{    
    return follower + DirectionOf(master - follower);
}

public static Vector DirectionOf(Vector distance)
{
    var x = distance.X;
    var y = distance.Y;
    
    return distance.IsAdjacency.Match(
        t => Vector.Zero,
        f => new Vector(
            x: Math.Abs(x) > 1 ? x / 2 : x,
            y: Math.Abs(y) > 1 ? y / 2 : y
        )
    );        
}
public Dictionary<char, Vector> Directions = new Dictionary<char, Vector>
{
    { 'R', new Vector( 1,  0) },
    { 'L', new Vector(-1,  0) },
    { 'U', new Vector( 0,  1) },
    { 'D', new Vector( 0, -1) }    
};

public class Vector
{
    public Vector(int x, int y)
    {
        X = x;
        Y = y;
    }
    
    public int X { get; } 
    public int Y { get; }
    
    public bool IsAdjacency => Math.Abs(X) <= 1 && Math.Abs(Y) <= 1;    
    
    public static Vector operator-(Vector a, Vector b) => new Vector(a.X - b.X, a.Y - b.Y);
    public static Vector operator+(Vector a, Vector b) => new Vector(a.X + b.X, a.Y + b.Y);

    public override int GetHashCode() => (X, Y).GetHashCode();
    public override bool Equals(object obj) => obj.As<Vector>().Map(v => (X, Y) == (v.X, v.Y)).GetOrFalse();
    
    public static Vector Zero => new Vector(0, 0);
}

public record State(Vector head, IEnumerable<Vector> knots, HashSet<Vector> tailLog)
{    
    public int PositionsVisitedByTail => tailLog.Count;
    
    public static State Initial => new State(Vector.Zero, Enumerable.Repeat(Vector.Zero, 9), new HashSet<Vector>());    
}
