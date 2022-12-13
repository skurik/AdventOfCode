<Query Kind="Program">
  <Namespace>FuncSharp</Namespace>
</Query>

//void Main()
//{
//    var input = File.ReadLines(@"c:\Users\twoflower\OneDrive\Devel\LINQPad\Queries\AdventOfCode\2022\13_input");
//    var pairs = input.Chunk(3).Select(chunk => (Left: Parse(chunk.First()), Right: Parse(chunk.Second()))).ToList();
//
//    var t = pairs.Indexed().Where(t => Compare(t.element.Left, t.element.Right).SafeEquals(-1)).Sum(t => t.index + 1);
//    t.Dump();
//
//    var dividerPackets = new[] { "[[2]]", "[[6]]" }.Select(Parse).ToList();
//    var allPackets = pairs.SelectMany(p => new[] { p.Left, p.Right }).Concat(dividerPackets);
//    var ordered = allPackets.OrderBy(p => p, new FuncComparer<Tree<int>>(Compare));
//
//    ordered.IndexOf(dividerPackets.First()).Dump();
//    ordered.IndexOf(dividerPackets.Second()).Dump();
//}
//
//public int Compare(Tree<int> left, Tree<int> right)
//{
//    return left.Match(
//        leftSubtree => right.Match(
//            rightSubtree => Compare(leftSubtree.Descendants, rightSubtree.Descendants),
//            rightLeaf => Compare(left, rightLeaf.ToTree())
//        ),        
//        leftLeaf => right.Match(
//            rightSubtree => Compare(leftLeaf.ToTree(), right),
//            rightLeaf => leftLeaf.value.CompareTo(rightLeaf.value)
//        )
//    );
//}
//
//public int Compare(IEnumerable<Tree<int>> left, IEnumerable<Tree<int>> right)
//{
//    var lengthDiff = left.Count() - right.Count();
//    var commonPartDifference = left.Zip(right, Compare).FirstOption(c => c.SafeNotEquals(0));
//    return commonPartDifference.GetOrElse(lengthDiff.Match(0, _ => 0, Math.Sign));
//}
//
//public Tree<int> Parse(string input)
//{
//    if (int.TryParse(input, out var value))
//        return new Tree<int>(new Leaf<int>(value));    
//    
//    return new Tree<int>(new Node<int>(
//        Descendants: ToSubtrees(input.Substring(1, input.Length - 2)).Select(Parse).ToList())
//    );
//}
//
//public ParserState Transition(ParserState state, char c)
//{
//    return c.Match(
//        '[', _ => new ParserState(state.Depth + 1, state.Buffer + c, state.Tokens),
//        ']', _ => new ParserState(state.Depth - 1, state.Buffer + c, state.Tokens),
//        ',', _ => state.Depth.Match(
//            0, _ => new ParserState(state.Depth, "", state.Tokens.Append(state.Buffer)),
//            _ => new ParserState(state.Depth, state.Buffer + c, state.Tokens)
//        ),
//        c => new ParserState(state.Depth, state.Buffer + c, state.Tokens)
//    );
//}
//
//public record ParserState(int Depth, string Buffer, IEnumerable<string> Tokens)
//{
//    public static ParserState Initial => new ParserState(0, "", Enumerable.Empty<string>());
//    public IEnumerable<string> AllTokens => Tokens.Concat(Buffer.ToNonEmptyOption().ToEnumerable());
//}
//
//public IEnumerable<string> ToSubtrees(string input)
//{
//    var parserState = input.Aggregate(ParserState.Initial, Transition);
//    return parserState.AllTokens;
//}

//public IEnumerable<string> ToSubtrees(string input)
//{
//    var depth = 0;
//    var buffer = "";
//    
//    foreach (var c in input)
//    {
//        switch (c)
//        {
//            case '[':                
//                buffer += c;
//                depth++;
//                break;
//            case ']':
//                depth--;                
//                buffer += c;
//                break;
//            case ',':
//                if (depth == 0)
//                {
//                    yield return buffer;
//                    buffer = "";
//                }
//                else
//                    buffer += ',';
//                break;
//            default:
//                buffer += c;
//                break;                
//        }
//    }
//    
//    if (buffer.NonEmpty())
//        yield return buffer;
//}

//public string TreeString(string input, int position)
//{
//    var depth = 0;
//    var closingPosition = input.Indexed().Skip(position + 1).First(t =>
//    {
//        if (t.element == ']')
//        {
//            if (depth == 0)
//                return true;
//            
//            depth--;
//        }
//        else if (t.element == '[')
//        {
//            depth++;
//        }
//        return false;
//    }).index;
//    
//    return input.Substring(position, closingPosition - position + 1);
//}

//public Tree<string> Parse(string input)
//{
//    var root = new Tree<string>(new Node<string>("", null, null));
//    var current = root;
//
//    foreach (var c in input)
//    {
//        switch (c)
//        {
//            case '[':
//                current = new Tree<string>(new Node<string>("", null, null));
//                //current.Parent.Get().Descendants.Add(current);
//                break;
//            case ']':
//                current = current.Parent.Get();
//                break;
//            case ',':
//                current = new Node<string> { Parent = current.Parent, Value = "", Descendants = new List<Node<string>>() };
//                current.Parent.Get().Descendants.Add(current);
//                break;
//            default:
//                current.Value += c;
//                break;
//        }
//    }
//
//    return root;
//}

public record Leaf<T>(T value)
{
    public Tree<T> ToTree() => new Tree<T>(new Node<T>(new Tree<T>(this).ToEnumerable()));
}

public record Node<T>(IEnumerable<Tree<T>> Descendants);

public class Tree<T> : Coproduct2<Node<T>, Leaf<T>>
{
    public Tree(Node<T> node) : base(node) { }
    public Tree(Leaf<T> leaf) : base(leaf) { }
}

//public record Leaf<T>(T value);
//
//public record Node<T>(T Value, List<Tree<T>> Descendants);
//
//public class Tree<T> : Coproduct2<Node<T>, Leaf<T>>
//{
//    public Tree(Node<T> node) : base(node) { }
//    public Tree(Leaf<T> leaf) : base(leaf) { }
//
//    public Tree<R> Map<R>(Func<T, R> map) => Match(
//        node => new Tree<R>(new Node<R>(map(node.Value), node.Descendants.Select(t => t.Map(map)).ToList())),
//        leaf => new Tree<R>(new Leaf<R>(map(leaf.value)))
//    );
//}

//public record BinaryTreeNode<T>(T Value, BinaryTree<T> Left, BinaryTree<T> Right);
//
//public class BinaryTree<T> : Coproduct2<BinaryTreeNode<T>, Leaf<T>>
//{
//    public BinaryTree(BinaryTreeNode<T> node) : base(node) { }    
//    public BinaryTree(Leaf<T> leaf) : base(leaf) { }
//    
//    public BinaryTree<R> Map<R>(Func<T, R> map) => Match(
//        node => new BinaryTree<R>(new BinaryTreeNode<R>(map(node.Value), node.Left.Map(map), node.Right.Map(map))),
//        leaf => new BinaryTree<R>(new Leaf<R>(map(leaf.value)))
//    );
//}