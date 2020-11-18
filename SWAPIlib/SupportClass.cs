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

        public static Box operator +(Point p1, Point p2)
        {
            //TODO добавить вычисление площади кубика
            return new Box(p1, p2);
        }

        public override string ToString()
        {
            return $"x={x * unitCorrect}, " +
                    $"y={y * unitCorrect}, " +
                    $"z={z * unitCorrect}";
        }

    }

    public struct Box
    {
        public Point p1;
        public Point p2;
        public static readonly int unitCorrect = 1000;

        public Box(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        public override string ToString()
        {
            return $"Box[{p1}:{p2}]";
        }
    }
}
