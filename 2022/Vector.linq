<Query Kind="Program">
  <Namespace>FuncSharp</Namespace>
</Query>

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
	public Vector Left => new Vector(X, Y - 1);
	public Vector Right => new Vector(X, Y + 1);
	public Vector Up => new Vector(X - 1, Y);
	public Vector Down => new Vector(X + 1, Y);
	public IEnumerable<Vector> PerpendicularNeighbors => new[] { Left, Right, Up, Down };
	public bool IsWithin(int x, int y) => X.InRange(0, x - 1) && Y.InRange(0, y - 1);

	public static Vector operator -(Vector a, Vector b) => new Vector(a.X - b.X, a.Y - b.Y);
	public static Vector operator +(Vector a, Vector b) => new Vector(a.X + b.X, a.Y + b.Y);

	public override int GetHashCode() => (X, Y).GetHashCode();
	public override bool Equals(object obj) => obj.As<Vector>().Map(v => (X, Y) == (v.X, v.Y)).GetOrFalse();

	public static Vector Zero => new Vector(0, 0);
}