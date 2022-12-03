<Query Kind="Program" />

void Main()
{
    var lines = File.ReadAllLines("01_input");
    var counter = 0;
    var elves = lines.Select((line, idx) =>
    {
        if (line.IsEmptyOrWhiteSpace())
        {
            counter++;
            return new Line { Value = 0, Group = counter }; 
        }
        return new Line { Value = int.Parse(line), Group = counter };        
    }).GroupBy(l => l.Group);
    
    var elvesByCalories = elves.Select(g => g.Sum(l => l.Value)).OrderByDescending(s => s);
    
    elvesByCalories.First().Dump();
    (elvesByCalories.First() + elvesByCalories.Second() + elvesByCalories.Third()).Dump();
}

public class Line
{
    public int Value { get; set; }       
    public int Group { get; set; }
}