using System;
using System.Collections.Generic;
using System.Linq;

namespace LawOfGravitySimulation
{
    class DrawFigure
    {
        private static int width = Program.width;
        private static int height = Program.height;
        public static bool firstFrame = true;
        private static DateTime dateTimeWas = DateTime.Now;
        private static int destroyedBodyId = -1;

        public static char[,] emptyArray = new char[width, height];
        public static List<Body> bodies = new List<Body>();
        private static List<PointOnScreen> pointsToDelete = new List<PointOnScreen>();
        private static List<PointOnScreen> pointsToDeleteNow = new List<PointOnScreen>();

        private class PointOnScreen
        {
            public int i { get; }
            public int j { get; }
            public PointOnScreen(int i, int j)
            {
                this.i = i;
                this.j = j;
            }
        }
        
        public static void refresh()
        {
            applyGravity();
            putFiguresTogether();
            pointsToDeleteNow = new List<PointOnScreen>(pointsToDelete);
            pointsToDelete.Clear();
            Console.SetCursorPosition(0, 0);
            for (int j = 0; j <= height - 1; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    if (Program.screen[i, j] == '@')
                    {
                        pointsToDelete.Add(new PointOnScreen(i, j));
                        Console.SetCursorPosition(i, j);
                        Console.Write('@');
                    }
                }
            }
            for (int i = 0; i < pointsToDeleteNow.Count; i++)
            {
                PointOnScreen cursor = pointsToDeleteNow[i];
                if (!pointsToDelete.Exists(point => point.i == cursor.i && point.j == cursor.j))
                {
                    Console.SetCursorPosition(cursor.i, cursor.j);
                    Console.Write(' ');
                }
            }
        }

        private static void applyGravity()
        {
            double deltaTime = (DateTime.Now - dateTimeWas).TotalMilliseconds / 40;
            dateTimeWas = DateTime.Now;
            if (firstFrame)
            {
                deltaTime = 1;
                firstFrame = false;
            }
            List<Body> newBodies = new List<Body>();
            List<int> elementsToDelete = new List<int>();
            for (int i = 0; i < bodies.Count; i++)
            {
                Body bodyI = bodies[i];
                Vector vector = new Vector(0, 0);
                for (int j = 0; j < bodies.Count; j++)
                {
                    if (j != i)
                    {
                        Body bodyJ = bodies[j];
                        Vector vector1 = new Vector(bodyJ.X0 - bodyI.X0, bodyJ.Y0 - bodyI.Y0);
                        double distance = vector1.getLength();
                        if (distance <= bodyI.Radius + bodyJ.Radius)
                        {
                            elementsToDelete.Add(i);
                            elementsToDelete.Add(j);
                            double newX = (bodyI.X0 + bodyJ.X0) / 2;
                            double newY = (bodyI.Y0 + bodyJ.Y0) / 2;
                            double newRadius = Math.Sqrt(bodyI.Radius * bodyI.Radius + bodyJ.Radius * bodyJ.Radius);
                            double newMass = bodyI.mass + bodyJ.mass;
                            Vector newVector = new Vector((bodyI.vector.x * bodyI.mass + bodyJ.vector.x * bodyJ.mass) / (bodyJ.mass + bodyI.mass), (bodyI.vector.y * bodyI.mass + bodyJ.vector.y * bodyJ.mass) / (bodyJ.mass + bodyI.mass));
                            Body body = new Body(newX, newY, newRadius, newMass, newVector, false);
                            if (Program.followingBodyNum == i) { destroyedBodyId = newBodies.Count; }
                            newBodies.Add(body);
                        }
                        double k = bodyJ.mass / (distance * distance * 200);
                        vector1.multiply(k * deltaTime);
                        vector.add(vector1);
                    }
                }
                bodyI.moveToVector(vector);
                if (Program.followingBodyNum == i)
                {
                    Console.SetCursorPosition(105, 0);
                    Console.Write("--------------");
                    Console.SetCursorPosition(105, 1);
                    Console.Write("Speed: " + Math.Round(bodyI.vector.getLength(), 2));
                    Console.SetCursorPosition(105, 2);
                    Console.Write("Mass: "  + Math.Round(bodyI.mass, 2));
                    Console.SetCursorPosition(105, 3);
                    Console.Write("X0: " + Math.Round(bodyI.X0, 2));
                    Console.SetCursorPosition(105, 4);
                    Console.Write("Y0: " + Math.Round(bodyI.Y0, 2));
                    Console.SetCursorPosition(105, 5);
                    Console.Write("Body id: " + i);
                    Console.SetCursorPosition(105, 6);
                    Console.Write("Bodies num: " + bodies.Count);
                    Console.SetCursorPosition(105, 7);
                    Console.Write("--------------");
                }
            }
            elementsToDelete.Sort();
            elementsToDelete.Reverse();
            var uniqueElementsToDelete = elementsToDelete.Distinct();
            for (int i = 0; i < uniqueElementsToDelete.Count(); i++)
            {
                bodies.RemoveAt((int)uniqueElementsToDelete.ElementAt(i));           
            }
            for (int i = 0; i < newBodies.Count; i++)
            {
                if (destroyedBodyId == i)
                {
                    Program.followingBodyNum = bodies.Count;
                }
                bodies.Add(newBodies[i]);
            }
        }

        public static void putFiguresTogether()
        {
            Array.Copy(emptyArray, Program.screen, emptyArray.Length);
            for (int z = 0; z < bodies.Count; z++)
            {
                Body body = bodies[z];
                char[,] figure = body.bodyScreen;
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (figure[i, j] == '@')
                        {
                            Program.screen[i, j] = '@';
                        }
                    }

                }
            }
        }
    }
}
