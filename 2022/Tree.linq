<Query Kind="Program">
  <Namespace>FuncSharp</Namespace>
</Query>

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