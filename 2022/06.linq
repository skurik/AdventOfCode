<Query Kind="Program">
  <Namespace>FuncSharp</Namespace>
</Query>

void Main()
{
    var input = File.ReadAllText(@"c:\Users\twoflower\OneDrive\Devel\LINQPad\Queries\AdventOfCode\2022\06_input");
    
    FindUniqueMarkerPosition(input, 4).Dump();
    FindUniqueMarkerPosition(input, 14).Dump();
}

public int FindUniqueMarkerPosition(string stream, int length)
{
    var zeroBased = length - 1;
    return Enumerable.Range(zeroBased, stream.Length - zeroBased).First(index => new HashSet<char>(stream.Substring(index - zeroBased, length)).Count == length) + 1;
}