<Query Kind="Program">
  <Namespace>FuncSharp</Namespace>
</Query>

#load ".\Tree"

void Main()
{
    var input = File.ReadLines("13_input");
    var pairs = input.Chunk(3).Select(chunk => (Left: Parse(chunk.First()), Right: Parse(chunk.Second()))).ToList();

    var correctOrderTotal = pairs.Indexed().Where(t => Compare(t.element.Left, t.element.Right).SafeEquals(-1)).Sum(t => t.index + 1);
    correctOrderTotal.Dump();

    var dividerPackets = new[] { "[[2]]", "[[6]]" }.Select(Parse).ToList();
    var allPackets = pairs.SelectMany(p => new[] { p.Left, p.Right }).Concat(dividerPackets);
    var ordered = allPackets.OrderBy(p => p, new FuncComparer<Tree<int>>(Compare));

    ordered.IndexOf(dividerPackets.First()).Dump();
    ordered.IndexOf(dividerPackets.Second()).Dump();
}

public int Compare(Tree<int> left, Tree<int> right)
{
    return left.Match(
        leftSubtree => right.Match(
            rightSubtree => Compare(leftSubtree.Descendants, rightSubtree.Descendants),
            rightLeaf => Compare(left, rightLeaf.ToTree())
        ),        
        leftLeaf => right.Match(
            rightSubtree => Compare(leftLeaf.ToTree(), right),
            rightLeaf => leftLeaf.value.CompareTo(rightLeaf.value)
        )
    );
}

public int Compare(IEnumerable<Tree<int>> left, IEnumerable<Tree<int>> right)
{
    var lengthDiff = left.Count() - right.Count();
    var commonPartDifference = left.Zip(right, Compare).FirstOption(c => c.SafeNotEquals(0));
    return commonPartDifference.GetOrElse(lengthDiff.Match(0, _ => 0, Math.Sign));
}

public Tree<int> Parse(string input)
{
    if (int.TryParse(input, out var value))
        return new Tree<int>(new Leaf<int>(value));    
    
    return new Tree<int>(new Node<int>(
        Descendants: ToSubtrees(input.Substring(1, input.Length - 2)).Select(Parse).ToList())
    );
}

public IEnumerable<string> ToSubtrees(string input)
{
    var parserState = input.Aggregate(ParserState.Initial, Transition);
    return parserState.AllTokens;
}

public ParserState Transition(ParserState state, char c)
{
    return c.Match(
        '[', _ => new ParserState(state.Depth + 1, state.Buffer + c, state.Tokens),
        ']', _ => new ParserState(state.Depth - 1, state.Buffer + c, state.Tokens),
        ',', _ => state.Depth.Match(
            0, _ => new ParserState(state.Depth, "", state.Tokens.Append(state.Buffer)),
            _ => new ParserState(state.Depth, state.Buffer + c, state.Tokens)
        ),
        c => new ParserState(state.Depth, state.Buffer + c, state.Tokens)
    );
}

public record ParserState(int Depth, string Buffer, IEnumerable<string> Tokens)
{
    public static ParserState Initial => new ParserState(0, "", Enumerable.Empty<string>());
    public IEnumerable<string> AllTokens => Tokens.Concat(Buffer.ToNonEmptyOption().ToEnumerable());
}