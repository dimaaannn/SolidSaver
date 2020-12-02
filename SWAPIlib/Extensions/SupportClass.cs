using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib
{
    /// <summary>
    /// Abstract point structure
    /// </summary>
    public struct Point
    {
        public double x;
        public double y;
        public double z;
        static int unitCorrect = 1000;

        public Point(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Point(IEnumerable<double> coord)
        {
            if(coord.Count() > 3)
            {
                throw new ArrayTypeMismatchException(
                    "Point: array must contain less than 3 elements");
            }
            x = coord.ElementAtOrDefault(0);
            y = coord.ElementAtOrDefault(1);
            z = coord.ElementAtOrDefault(2);
        }

        public static Box operator +(Point p1, Point p2)
        {
            return new Box(p1, p2);
        }

        public override string ToString()
        {
            return $"x={x * unitCorrect:0.00}:" +
                    $"y={y * unitCorrect:0.00}:" +
                    $"z={z * unitCorrect:0.00}";
        }

    }

    public struct Box
    {
        public Point p1;
        public Point p2;
        public static readonly int unitCorrect = 1000;

        public double dimX { get => System.Math.Abs(p1.x - p2.x) * unitCorrect; }
        public double dimY { get => System.Math.Abs(p1.y - p2.y) * unitCorrect; }
        public double dimZ { get => System.Math.Abs(p1.z - p2.z) * unitCorrect; }

        public double[] dim { get => new double[3] { dimX, dimY, dimZ }; }

        public Box(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }
        public Box(IEnumerable<double> coord)
        {
            if (coord.Count() != 6)
                throw new ArrayTypeMismatchException("Box: array must contains 6 elements");
            this.p1 = new Point(coord.Take(3));
            this.p2 = new Point(coord.Skip(3));
        }

        public override string ToString()
        {
            return $"Box[{p1}|{p2}]";
        }
    }
}
