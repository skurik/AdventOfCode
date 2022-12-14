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
	public Vector Left => new Vector(X - 1, Y);
	public Vector Right => new Vector(X + 1, Y);
	public Vector Up => new Vector(X, Y - 1);
	public Vector Down => new Vector(X, Y + 1);
	public Vector LeftDown => Left + new Vector(0, 1);
	public Vector RightDown => Right + new Vector(0, 1);
	public IEnumerable<Vector> PerpendicularNeighbors => new[] { Left, Right, Up, Down };
	public bool IsWithin(int x, int y) => X.InRange(0, x - 1) && Y.InRange(0, y - 1);
	
	public Vector MinByX(Vector other) => MinBy(other, v => v.X);
	public Vector MinByY(Vector other) => MinBy(other, v => v.Y);

	public Vector MaxByX(Vector other) => MaxBy(other, v => v.X);
	public Vector MaxByY(Vector other) => MaxBy(other, v => v.Y);

	public IEnumerable<Vector> Range(Vector end) => (X == end.X).Match(
		t =>
		{
			var byY = new[] { MinByY(end), MaxByY(end) };
			return Enumerable.Range(byY[0].Y, byY[1].Y - byY[0].Y + 1).Select(y => new Vector(X, y));
		},
		f =>
		{
			var byX = new[] { MinByX(end), MaxByX(end) };
			return Enumerable.Range(byX[0].X, byX[1].X - byX[0].X + 1).Select(x => new Vector(x, Y));
		}
	);

	public static Vector operator -(Vector a, Vector b) => new Vector(a.X - b.X, a.Y - b.Y);
	public static Vector operator +(Vector a, Vector b) => new Vector(a.X + b.X, a.Y + b.Y);

	public override int GetHashCode() => (X, Y).GetHashCode();
	public override bool Equals(object obj) => obj.As<Vector>().Map(v => (X, Y) == (v.X, v.Y)).GetOrFalse();

	public static Vector Zero => new Vector(0, 0);

	public override string ToString() => $"({X}, {Y})";
	
	private Vector MinBy(Vector other, Func<Vector, int> coord) => (coord(this) <= coord(other)).Match(t => this, f => other);
	private Vector MaxBy(Vector other, Func<Vector, int> coord) => (coord(this) > coord(other)).Match(t => this, f => other);
}