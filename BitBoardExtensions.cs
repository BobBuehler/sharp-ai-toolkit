using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

public static class BitBoardExtensions
{
    public static bool Get(this BitArray bits, int x, int y)
    {
        return bits.Get(Bb.GetOffset(x, y));
    }

    public static bool Get(this BitArray bits, Point p)
    {
        return bits.Get(p.x, p.y);
    }

    public static void Set(this BitArray bits, int x, int y, bool value)
    {
        bits.Set(Bb.GetOffset(x, y), value);
    }

    public static void Set(this BitArray bits, Point p, bool value)
    {
        bits.Set(p.x, p.y, value);
    }

    public static Point ToPoint(this Mappable mappable)
    {
        return new Point(mappable.X, mappable.Y);
    }

    public static Func<Point, bool> ToFunc(this BitArray bits)
    {
        return p => bits.Get(p);
    }

    public static IEnumerable<Point> ToPoints(this BitArray bits)
    {
        for (int x = 0; x < Bb.Width; ++x)
        {
            for (int y = 0; y < Bb.Height; ++y)
            {
                if (bits.Get(x, y))
                {
                    yield return new Point(x, y);
                }
            }
        }
    }

    public static BitArray ToBitArray(this IEnumerable<Point> points)
    {
        var bits = new BitArray(Bb.Width * Bb.Height);
        points.ForEach(p => bits.Set(p, true));
        return bits;
    }

    public static int ManhattanDistance(this Point source, Point target)
    {
        return Math.Abs(source.x - target.x) + Math.Abs(source.y - target.y);
    }

    public static bool IsInRange(this Point source, int range, Point target)
    {
        return range >= source.ManhattanDistance(target);
    }

    public static int Count(this BitArray bits)
    {
        int count = 0;
        for (int i = 0; i < bits.Count; ++i)
        {
            if (bits[i])
            {
                ++count;
            }
        }
        return count;
    }
}