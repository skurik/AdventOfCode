<Query Kind="Program">
  <Namespace>FuncSharp</Namespace>
</Query>

#load ".\Vector"
#load ".\Rectangle"

void Main()
{
    var areas = File.ReadLines("15_input").Select(ParseArea).ToList();
    
    var y = 2_000_000;
    GetCountOfUncovered(areas, y).Dump();

    var width = 4_000_000;
    var rectangle = new Rectangle(new Vector(0, 0), width, width);
    var distressedBeacon = GetUncoveredArea(rectangle, areas).First();
    
    ((distressedBeacon.TopLeft.X * 4000000L) + distressedBeacon.TopLeft.Y).Dump();
}

public int GetCountOfUncovered(IEnumerable<Area> areas, int y)
{    
    var leftmost = areas.Select(a => Leftmost(a, y)).Flatten().MinBy(v => v.X).Get();
    var rightmost = areas.Select(a => Rightmost(a, y)).Flatten().MaxBy(v => v.X).Get();
    
    return Enumerable.Range(leftmost.X, rightmost.X - leftmost.X + 1).Count(x => areas.Any(a => IsCoveredNonBeacon(new Vector(x, y), a)));
}

public bool IsCoveredNonBeacon(Vector position, Area area)
{
    return area.Covers(position) && area.Beacon.SafeNotEquals(position);
}

public IEnumerable<Rectangle> GetUncoveredArea(Rectangle rectangle, IEnumerable<Area> sensorAreas)
{
    if (rectangle.Width == 0 || rectangle.Height == 0)
        return Enumerable.Empty<Rectangle>();
        
    if (sensorAreas.Any(a => rectangle.Corners.All(c => InManhattanArea(a.Center, a.ManhattanDistance, c))))
        return Enumerable.Empty<Rectangle>();
        
    if (rectangle.Width == 1 && rectangle.Height == 1)
        return rectangle.ToEnumerable();
        
    return rectangle.Quadrants.SelectMany(q => GetUncoveredArea(q, sensorAreas));
}

public Area ParseArea(string input)
{
    var coords = Regex.Matches(input, @"(-?\d+)").Select(m => m.Groups[1].Value).ToList();
    var sensor = new Vector(int.Parse(coords[0]), int.Parse(coords[1]));
    var beacon = new Vector(int.Parse(coords[2]), int.Parse(coords[3]));

    return new Area(sensor, beacon, ManhattanDistance(sensor, beacon));
}

public static bool InManhattanArea(Vector center, int distance, Vector position)
{
    return ManhattanDistance(center, position) <= distance;
}

public static int ManhattanDistance(Vector first, Vector second)
{
    return Math.Abs(first.X - second.X) + Math.Abs(first.Y - second.Y);
}

public IOption<Vector> Leftmost(Area area, int y)
{
    var yDistance = Math.Abs(area.Center.Y - y);
    return (area.ManhattanDistance - yDistance).ToOption().Where(d => d >= 0).Map(d => new Vector(area.Center.X - d, y));
}

public IOption<Vector> Rightmost(Area area, int y)
{
    var yDistance = Math.Abs(area.Center.Y - y);
    return (area.ManhattanDistance - yDistance).ToOption().Where(d => d >= 0).Map(d => new Vector(area.Center.X + d, y));
}

public record Area(Vector Center, Vector Beacon, int ManhattanDistance)
{
    public bool Covers(Vector position) => InManhattanArea(Center, ManhattanDistance, position);
}