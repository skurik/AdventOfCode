<Query Kind="Program">
  <Namespace>FuncSharp</Namespace>
</Query>

using Grid = System.Collections.Generic.List<System.Collections.Generic.List<int>>;
using TreeLine = System.Collections.Generic.IEnumerable<int>;
using TreeHeight = System.Int32;

void Main()
{
	var inputLines = File.ReadLines("08_input");
	var grid = ReadGrid(inputLines);
	
	NumberOfVisibleTrees(grid).Dump();	
	GetMaxVisibilityScore(grid).Dump();
}

public Grid ReadGrid(IEnumerable<string> rows)
{
	return rows.Select(row => row.Select(c => (int)(c - '0')).ToList()).ToList();
}

public int NumberOfVisibleTrees(Grid grid)
{
	return Aggregate(grid, IsVisible, rowScores => rowScores.Count(t => t), Enumerable.Sum);
}

public int GetMaxVisibilityScore(Grid grid)
{
	return Aggregate(grid, VisibilityScore, Enumerable.Max, Enumerable.Max);
}

public bool IsVisible(Grid grid, int row, int column)
{
	return AggregateOverDirections(grid, row, column, false, IsVisible, (v1, v2) => v1 || v2);
}

public int VisibilityScore(Grid grid, int row, int column)
{
	return AggregateOverDirections(grid, row, column, 1, GetDirectionalVisibility, (score1, score2) => score1 * score2);
}

public bool IsVisible(TreeHeight height, TreeLine treeLine)
{
	return treeLine.All(h => h < height);
}

public int GetDirectionalVisibility(TreeHeight height, TreeLine treeLine)
{	
	var smallerTrees = treeLine.TakeWhile(h => h < height);
	var sameHeightTree = treeLine.Skip(smallerTrees.Count()).Take(1).Where(h => h == height);

	return smallerTrees.Count() + sameHeightTree.Count();
}

public TreeLine GetTreeLine(Grid grid, int row, int column, Direction direction)
{
	return direction.Match(
		Direction.Left, _ => grid[row].Take(column).Reverse(),
		Direction.Top, _ => grid.Take(row).Select(r => r[column]).Reverse(),
		Direction.Right, _ => grid[row].Skip(column + 1),
		Direction.Bottom, _ => grid.Skip(row + 1).Select(r => r[column])
	);
}

public int Aggregate<T>(Grid grid, Func<Grid, int, int, T> projection, Func<IEnumerable<T>, int> rowAggregate, Func<IEnumerable<int>, int> totalAggregate)
{
	return totalAggregate(
		Enumerable.Range(0, grid.Count).Select(
			row => rowAggregate(Enumerable.Range(0, grid[row].Count).Select(
				column => projection(grid, row, column))
			)
		)
	);
}

public T AggregateOverDirections<T>(Grid grid, int row, int column, T startValue, Func<TreeHeight, TreeLine, T> projection, Func<T, T, T> aggregator)
{
	return Directions.Aggregate(startValue, (acc, next) => aggregator(acc, projection(grid[row][column], GetTreeLine(grid, row, column, next))));
}

public enum Direction
{
	Left,
	Top,
	Right,
	Bottom
}

public static IEnumerable<Direction> Directions = Enum<Direction>.GetValues();