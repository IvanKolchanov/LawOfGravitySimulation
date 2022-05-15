using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LawOfGravitySimulation
{
    class Program
    {
        public static int width = 120;
        public static int height = 30;
        public static double aspect = (double)width / height;
        public static double pixelAspect = (double)8 / 16;
        public static bool followBody = false;
        public static int followingBodyNum = -1;
        public static char[,] screen = new char[width, height];
        public static bool simulation, ESC_WAS_PRESSED = false;

        public static List<Body> startSimulationBodies = new List<Body>(); 

        private static void setup()
        {
            Console.CursorVisible = false;
            Console.Title = "Law of gravity simulation";
            Console.SetWindowSize(width, height);
            Console.BufferWidth = width;
            for (int j = height - 1; j >= 0; j--)
            {
                for (int i = 0; i < width; i++)
                {
                    screen[i, j] = ' ';
                    DrawFigure.emptyArray[i, j] = ' ';
                }
            }
        }

        private static void startScreen()
        {
            Console.WriteLine("This is a law of gravity simulater");
            Console.WriteLine("There are a few features that you can use for more convinient use:" + "\n" + "\n" +
                "*** You can use '-' key to zoom out of the picture and '+' key to zoom in the picture" + "\n" + "\n" + 
                "*** While not in spectator mode you can move the camera buy pressing Up, Down, Left and Right Arrows on the keyboard" + "\n" + "\n" +
                "*** You can press H to enter spectator mode, that will allow you to follow a certain sphere, you would be able to zoom in or out, but Up, Down Arrows won't work and Left, Right arrow will change the body you are spectating" + "\n" + "\n" + 
                "*** If you press H again after entering spectator mode, it will be turned off and you'll stay on the point where the body you follow was at the last moment");
            Console.WriteLine("\n" + "Press any key to continue");
            Console.ReadKey();
            bodyMenu();
        }

        private static void bodyMenu()
        {
            Console.Clear();
            bool startSimulation = false;
            simulation = false;
            followBody = false;
            followingBodyNum = -1;
            for (int i = 0; i <= 7; i++)
            {
                Console.SetCursorPosition(105, i);
                Console.Write("              ");
            }
            while (!startSimulation)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Body menu:");
                Console.WriteLine("1. See current bodies" + "\n" + 
                    "2. Add a body" + "\n" +
                    "3. Delete a body" + "\n" + 
                    "4. Start simulation");
                ConsoleKey key = Console.ReadKey(true).Key;
                Console.Clear();
                switch (key)
                {
                    case ConsoleKey.D1:
                        for (int i = 0; i < startSimulationBodies.Count; i++)
                        {
                            Body bodyI = startSimulationBodies[i];
                            Console.WriteLine("Id: " + i + " " + "X0: " + bodyI.X0 + " " + "Y0: " + bodyI.Y0 + "\n" +
                                "Mass: " + bodyI.mass + " " + "Speed vector: " + "(" + bodyI.vector.x + "; " + bodyI.vector.y + ")" + "\n");
                        }
                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ConsoleKey.D2:
                        try
                        {
                            Console.WriteLine("You'll have to enter the variables to create a body");
                            Console.Write("XO: "); double X0 = Convert.ToDouble(Console.ReadLine().Replace('.', ','));
                            Console.Write("YO: "); double Y0 = Convert.ToDouble(Console.ReadLine().Replace('.', ','));
                            Console.Write("Radius: "); double Radius = Convert.ToDouble(Console.ReadLine().Replace('.', ','));
                            Console.Write("Mass: "); double mass = Convert.ToDouble(Console.ReadLine().Replace('.', ','));
                            Console.Write("Speed vector: ( "); double vectorx = Convert.ToDouble(Console.ReadLine().Replace('.', ','));
                            Console.Write(" ; "); double vectory = Convert.ToDouble(Console.ReadLine().Replace('.', ',')); Console.Write(")");
                            Console.ReadLine();
                            Body body = new Body(X0, Y0, Radius, mass, new Vector(vectorx, vectory), true);
                        }catch (Exception e) {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case ConsoleKey.D3:
                        for (int i = 0; i < startSimulationBodies.Count; i++)
                        {
                            Body bodyI = startSimulationBodies[i];
                            Console.WriteLine("Id: " + i + " " + "X0: " + bodyI.X0 + " " + "Y0: " + bodyI.Y0 + "\n" +
                                "Mass: " + bodyI.mass + " " + "Speed vector: " + "(" + bodyI.vector.x + "; " + bodyI.vector.y + ")");
                        }
                        Console.WriteLine("Enter the id of the body, you want to delete: ");
                        try
                        {
                            int input = Convert.ToInt32(Console.ReadLine());
                            startSimulationBodies.RemoveAt(input);
                        }catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        Console.ReadKey();
                        break;
                    case ConsoleKey.D4:
                        for (int i = 0; i < startSimulationBodies.Count; i++)
                        {
                            DrawFigure.bodies.Add(startSimulationBodies[i].clone());
                        }
                        simulation = true;
                        DrawFigure.firstFrame = true;
                        startSimulation = true;
                        break;
                }
                Console.Clear();
            }
            
        }

        static void Main(string[] args)
        {
            Body body1 = new Body(2, 1, 0.05, 0.1, new Vector(-0.1, 0), true);
            Body body2 = new Body(-2, -1, 0.2, 5, new Vector(0.1, 0), true);
            Body body3 = new Body(1, -1, 0.1, 1, new Vector(0.3, 0.2), true);
            setup();
            startScreen();
            while (simulation)
            {
                DrawFigure.refresh();
                if (Console.KeyAvailable)
                {
                    Task.Factory.StartNew(() =>
                    {
                        if (simulation)
                        {
                            ConsoleKey key = Console.ReadKey(true).Key;
                            switch (key)
                            {
                                case ConsoleKey.RightArrow:
                                    if (!followBody)
                                    {
                                        Coordinates.deltaX += 0.2;
                                    }
                                    else
                                    {
                                        followingBodyNum++;
                                        if (followingBodyNum >= DrawFigure.bodies.Count) followingBodyNum -= DrawFigure.bodies.Count;
                                        Console.SetCursorPosition(105, 0);
                                        Console.Write("--------------");
                                        Console.SetCursorPosition(105, 1);
                                        Console.Write("Speed:        ");
                                        Console.SetCursorPosition(105, 2);
                                        Console.Write("Mass:         ");
                                        Console.SetCursorPosition(105, 3);
                                        Console.Write("X0:           ");
                                        Console.SetCursorPosition(105, 4);
                                        Console.Write("Y0:           ");
                                        Console.SetCursorPosition(105, 5);
                                        Console.Write("Body id:      ");
                                        Console.SetCursorPosition(105, 6);
                                        Console.Write("Bodies num:   ");
                                        Console.SetCursorPosition(105, 7);
                                        Console.Write("--------------");
                                    }
                                    break;
                                case ConsoleKey.LeftArrow:
                                    if (!followBody)
                                    {
                                        Coordinates.deltaX -= 0.2;
                                    }
                                    else
                                    {
                                        followingBodyNum--;
                                        if (followingBodyNum < 0) followingBodyNum = DrawFigure.bodies.Count + followingBodyNum;
                                        Console.SetCursorPosition(105, 0);
                                        Console.Write("--------------");
                                        Console.SetCursorPosition(105, 1);
                                        Console.Write("Speed:        ");
                                        Console.SetCursorPosition(105, 2);
                                        Console.Write("Mass:         ");
                                        Console.SetCursorPosition(105, 3);
                                        Console.Write("X0:           ");
                                        Console.SetCursorPosition(105, 4);
                                        Console.Write("Y0:           ");
                                        Console.SetCursorPosition(105, 5);
                                        Console.Write("Body id:      ");
                                        Console.SetCursorPosition(105, 6);
                                        Console.Write("Bodies num:   ");
                                        Console.SetCursorPosition(105, 7);
                                        Console.Write("--------------");
                                    }
                                    break;
                                case ConsoleKey.UpArrow:
                                    if (!followBody) Coordinates.deltaY += 0.2;
                                    break;
                                case ConsoleKey.DownArrow:
                                    if (!followBody) Coordinates.deltaY -= 0.2;
                                    break;
                                case ConsoleKey.OemPlus:
                                    Coordinates.zoom -= 0.2;
                                    Coordinates.zoom = Math.Max(Coordinates.zoom, 0.2);                        
                                    break;
                                case ConsoleKey.OemMinus:
                                    Coordinates.zoom += 0.2;
                                    break;
                                case ConsoleKey.H:
                                    if (!followBody)
                                    {
                                        followBody = true;
                                        followingBodyNum = 0;
                                        Coordinates.zoom = 1;
                                    }
                                    else
                                    {
                                        followBody = false;
                                        followingBodyNum = -1;
                                        for (int i = 0; i <= 7; i++)
                                        {
                                            Console.SetCursorPosition(105, i);
                                            Console.Write("              ");
                                        }
                                    }
                                    break;
                                case ConsoleKey.Escape:
                                    ESC_WAS_PRESSED = true;
                                    break;
                            }
                        }  
                    });
                }
                
                if (ESC_WAS_PRESSED)
                {
                    ESC_WAS_PRESSED = false;
                    DrawFigure.bodies.Clear();
                    bodyMenu();
                }
                Thread.Sleep(50);
            }
        }
    }
}
