using System;

namespace LawOfGravitySimulation
{
    public class Vector
    {
        public double x, y;

        public Vector(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public void multiply(double k)
        {
            x *= k;
            y *= k;
        }

        public double getLength()
        {
            return Math.Sqrt(x * x + y * y);
        }

        public void add(Vector vector)
        {
            x += vector.x;
            y += vector.y;
        }

    }
}
