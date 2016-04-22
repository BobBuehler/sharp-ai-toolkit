using Xunit;

public class LazyListTests
{
    [Fact]
    public void Point_Equal()
    {
        var p = new Point(1, 2);
        Assert.Equal(true, p.Equals(p));
        Assert.Equal(true, new Point(p.x, p.y).Equals(p));
        Assert.Equal(false, p.Equals(new Point(2, 1)));
    }
    
    [Theory]
    [InlineData(1, 2)]
    [InlineData(0, 0)]
    [InlineData(3, 4)]
    public void Point_GetHashCode(int x, int y)
    {
        Assert.Equal(new Point(x, y).GetHashCode(), new Point(x, y).GetHashCode());
    }
}