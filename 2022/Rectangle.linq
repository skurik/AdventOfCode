<Query Kind="Program" />

#load ".\Vector"

public record Rectangle(Vector TopLeft, int Width, int Height)
{
    public IEnumerable<Vector> Corners => new[]
    {
        TopLeft,
        new Vector(TopLeft.X + Width - 1, TopLeft.Y),
        new Vector(TopLeft.X + Width - 1, TopLeft.Y + Height - 1),
        new Vector(TopLeft.X, TopLeft.Y + Height - 1)
    };

    public IEnumerable<Rectangle> Quadrants => new[]
    {
        new Rectangle(TopLeft, Width / 2, Height / 2),
        new Rectangle(new Vector(TopLeft.X + Width / 2, TopLeft.Y), Width / 2 + 1, Height / 2 + 1),
        new Rectangle(new Vector(TopLeft.X, TopLeft.Y + Height / 2), Width / 2 + 1, Height / 2 + 1),
        new Rectangle(new Vector(TopLeft.X + Width / 2, TopLeft.Y + Height / 2), Width / 2 + 1, Height / 2 + 1),
    };
}