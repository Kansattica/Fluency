public class Line
{
    public int Number {get; private set;}
    public string Contents {get; private set;}

    private Line() { }
    public static Line Create(string contents, int number)
    {
        return new Line()
        {
            Number = number,
            Contents = contents
        };
    }

    public override string ToString()
    {
        return Number + ":" + Contents;
    }
}