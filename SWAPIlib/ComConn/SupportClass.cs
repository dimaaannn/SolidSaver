using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.ComConn
{
    /// <summary>
    /// Abstract point structure
    /// </summary>
    public class Point
    {
        public double x;
        public double y;
        public double z;
        static int unitCorrect = 1000;
        public bool IsNull = true;

        public Point(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            IsNull = false;
        }
        public Point(IEnumerable<double> coord)
        {
            //if (coord.Count() > 3)
            //{
            //    throw new ArrayTypeMismatchException(
            //        "Point: array must contain less than 3 elements");
            //}
            if(coord != null)
            {
                x = coord.ElementAtOrDefault(0);
                y = coord.ElementAtOrDefault(1);
                z = coord.ElementAtOrDefault(2);
                IsNull = false;
            }
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

    public class MaterialProperty
    {
        public MaterialProperty()
        {
            Red = 0;
            Green = 0;
            Blue = 0;

            Ambient = 0;
            Diffuse = 0;
            Specular = 0;
            Shininess = 0;
            Transparency = 0;
            Emission = 0;
        }
        public MaterialProperty(double[] arr)
        {
            Red = Convert.ToInt32(arr[0] * 255);
            Green = Convert.ToInt32(arr[1] * 255);
            Blue = Convert.ToInt32(arr[2] * 255);

            Ambient = arr[3]; Diffuse = arr[4]; Specular = arr[5];
            Shininess = arr[6]; Transparency = arr[7]; Emission = arr[8];
        }
        public int Red;
        public int Green;
        public int Blue;

        public double Ambient;
        public double Diffuse;
        public double Specular;
        public double Shininess;
        public double Transparency;
        public double Emission;

        public static implicit operator double[](MaterialProperty mp)
        {
            return new double[] {mp.Red / 255.0, mp.Green / 255.0, mp.Blue / 255.0,
            mp.Ambient, mp.Diffuse, mp.Specular, mp.Shininess, mp.Transparency, mp.Emission };
        }

        public double[] ToComData() { return (double[])this; }
    }

    public struct Box
    {
        public Point p1;
        public Point p2;
        public static readonly int unitCorrect = 1000;

        public double dimX { get => Math.Abs(p1.x - p2.x) * unitCorrect; }
        public double dimY { get => Math.Abs(p1.y - p2.y) * unitCorrect; }
        public double dimZ { get => Math.Abs(p1.z - p2.z) * unitCorrect; }

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
            p1 = new Point(coord.Take(3));
            p2 = new Point(coord.Skip(3));
        }

        public override string ToString()
        {
            return $"Box[{p1}|{p2}]";
        }
    }
}
