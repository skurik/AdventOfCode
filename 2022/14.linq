<Query Kind="Program">
  <Namespace>FuncSharp</Namespace>
  <IncludeUncapsulator>false</IncludeUncapsulator>
</Query>

#load ".\Vector"

void Main()
{
    var paths = File.ReadLines("14_input").Select(ParsePath).ToList();
    var floor = Floor(paths);
    
    Simulate(paths, new Vector(500, 0), floor, false).Dump();
    Simulate(paths, new Vector(500, 0), floor, true).Dump();
}

public int Simulate(IEnumerable<Path> paths, Vector sandStart, Path floor, bool sandAccumulatesOnFloor)
{    
    var occupied = new HashSet<Vector>(paths.SelectMany(p => p.Segments.SelectMany(s => s.Points)));
    var rocks = occupied.Count();
    var floorPoints = new HashSet<Vector>(floor.Segments.SelectMany(s => s.Points));
    var abyssReached = false;

    while (!occupied.Contains(sandStart) && !abyssReached)
    {        
        var sandPosition = GetSandDestination(occupied, sandStart, floorPoints.Contains, sandAccumulatesOnFloor);
        sandPosition.Match(
            p => occupied.Add(p),
            _ => abyssReached = true
        );
    }    

    return occupied.Count() - rocks;
}

public IOption<Vector> GetSandDestination(HashSet<Vector> occupied, Vector start, Func<Vector, bool> isFloor, bool sandAccumulatesOnFloor)
{
    var position = start;
    while (true)
    {
        var target = NextMoves(position).FirstOption(t => !occupied.Contains(t));
        if (target.IsEmpty)
            return position.ToOption();

        var nextPosition = target.Get();
        if (isFloor(nextPosition))
            return sandAccumulatesOnFloor.Match(
                t => position.ToOption(),
                f => Option.Empty<Vector>()
            );

        position = nextPosition;
    }
}

public Path Floor(IEnumerable<Path> paths)
{
    var lowestPoint = paths.Max(p => p.Points.Max(c => c.Y));
    return new Path(new[] { new Vector(-2000, lowestPoint + 2), new Vector(2000, lowestPoint + 2) });
}

public Path ParsePath(string input)
{
    return new Path(input.Words().Chunk(3).Select(c => new Vector(int.Parse(c.First()), int.Parse(c.Second()))));
}

public IEnumerable<Vector> NextMoves(Vector position) => new[]{ position.Down, position.LeftDown, position.RightDown };

public record Segment(Vector Start, Vector End)
{
    private HashSet<Vector> points;

    public bool Contains(Vector position)
    {
        points = points ?? new HashSet<Vector>(Points);
        return points.Contains(position);
    }

    public IEnumerable<Vector> Points => Start.Range(End);
}

public record Path(IEnumerable<Vector> Points)
{
    public IEnumerable<Segment> Segments => Points.Zip(Points.Skip(1), (start, end) => new Segment(start, end));
}