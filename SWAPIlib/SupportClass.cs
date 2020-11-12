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
        public byte dimensions;
        static int unitCorrect = 1000;

        public Point(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            dimensions = 3;
        }

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
            dimensions = 2;
        }

        public static Box operator +(Point a, Point b)
        {
            //TODO добавить вычисление площади кубика
            return new Box();
        }

        public override string ToString()
        {
            if (dimensions == 2)
                return $"p x={x * unitCorrect}, " +
                    $"y={y * unitCorrect}";

            else
                return $"p x={x * unitCorrect}, " +
                    $"y={y * unitCorrect}, " +
                    $"z={z * unitCorrect}";
        }


    }

    public struct Box
    {
        public double x, y, z;
        public readonly byte dimensions;
        public static readonly int unitCorrect = 1000;

        public Box(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            dimensions = 3;
        }
    }
}
