<Query Kind="Program">
  <Namespace>FuncSharp</Namespace>
</Query>

void Main()
{
    var rounds = ReadRounds("02_input");
    var scores = rounds.Select(GetScore);
    
    scores.Sum().Dump();

    var rounds2 = ReadRoundsPart2("02_input");
    var scores2 = rounds2.Select(GetScore);

    scores2.Sum().Dump();
}

public IEnumerable<Round> ReadRounds(string path)
{
    var lines = File.ReadAllLines(path);
    var rounds = lines.Select(line => new Round
    {
        Opponent = GetShape(line[0]),
        Me = GetShape(line[2])
    }).ToList();
    
    return rounds;
}

public IEnumerable<Round> ReadRoundsPart2(string path)
{
    var lines = File.ReadAllLines(path);
    var rounds = lines.Select(line => new Round
    {
        Opponent = GetShape(line[0]),
        Me = GetShape(GetShape(line[0]), GetOutcome(line[2]))
    }).ToList();

    return rounds;
}

public int GetScore(Round round)
{
    var resultScore = round.Opponent.Match(
        Shape.Rock, _ => round.Me.Match(
            Shape.Rock, _ => 3,
            Shape.Paper, _ => 6,
            Shape.Scissors, _ => 0
        ),
        Shape.Paper, _ => round.Me.Match(
            Shape.Rock, _ => 0,
            Shape.Paper, _ => 3,
            Shape.Scissors, _ => 6
        ),
        Shape.Scissors, _ => round.Me.Match(
            Shape.Rock, _ => 6,
            Shape.Paper, _ => 0,
            Shape.Scissors, _ => 3
        )
    );

    var shapeScore = round.Me.Match(
        Shape.Rock, _ => 1,
        Shape.Paper, _ => 2,
        Shape.Scissors, _ => 3
    );
    
    return resultScore + shapeScore;
}

public Shape GetShape(char code)
{
    return code.Match(
        'A', _ => Shape.Rock,
        'B', _ => Shape.Paper,
        'C', _ => Shape.Scissors,
        'X', _ => Shape.Rock,
        'Y', _ => Shape.Paper,
        'Z', _ => Shape.Scissors
    );
}

public Outcome GetOutcome(char code)
{
    return code.Match(        
        'X', _ => Outcome.Loss,
        'Y', _ => Outcome.Draw,
        'Z', _ => Outcome.Win
    );
}

public Shape GetShape(Shape opponent, Outcome outcome)
{
    return opponent.Match(
        Shape.Rock, _ => outcome.Match(
            Outcome.Draw, _ => Shape.Rock,
            Outcome.Loss, _ => Shape.Scissors,
            Outcome.Win, _ => Shape.Paper
        ),
        Shape.Paper, _ => outcome.Match(
            Outcome.Draw, _ => Shape.Paper,
            Outcome.Loss, _ => Shape.Rock,
            Outcome.Win, _ => Shape.Scissors
        ),
        Shape.Scissors, _ => outcome.Match(
            Outcome.Draw, _ => Shape.Scissors,
            Outcome.Loss, _ => Shape.Paper,
            Outcome.Win, _ => Shape.Rock
        )
    );
}

public class Round
{
    public Shape Me { get; set; }
    public Shape Opponent { get; set; }
}

public enum Shape
{
    Rock,
    Paper,
    Scissors
}

public enum Outcome
{
    Draw,
    Loss,
    Win
}