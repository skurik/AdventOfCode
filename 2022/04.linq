<Query Kind="Program">
  <Namespace>FuncSharp</Namespace>
</Query>

void Main()
{
    var pairs = Read("04_input");
    pairs.Count(p => p.first.IsContainedIn(p.second) || p.second.IsContainedIn(p.first)).Dump();
    pairs.Count(p => p.first.Overlaps(p.second)).Dump();
}

public IEnumerable<(Range first, Range second)> Read(string path)
{
    return File.ReadLines(path).Select(ReadLine);
}

public (Range first, Range second) ReadLine(string line)
{
    var ranges = line.Split(',').Select(ReadRange);
    return (ranges.First(), ranges.Second());
}

public Range ReadRange(string text)
{
    var boundaries = text.Split('-').Select(int.Parse);
    return new Range
    {
        Start = boundaries.First(),
        End = boundaries.Second()
    };
}

public class Range
{
    public int Start { get; set; }
    public int End { get; set; }
    
    public bool IsContainedIn(Range other)
    {
        return Start >= other.Start && End <= other.End;
    }
    
    public bool Overlaps(Range other)
    {
        return Start.CompareTo(other.Start).Match(
            -1, _ => End >= other.Start,
            0, _ => true,
            1, _ => Start <= other.End
        );
    }
}