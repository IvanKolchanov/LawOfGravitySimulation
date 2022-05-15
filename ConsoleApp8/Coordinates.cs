using System;

namespace LawOfGravitySimulation
{
    class Coordinates
    {
        private static int width = Program.width, height = Program.height;
        private static double aspect = Program.aspect, pixelAspect = Program.pixelAspect;
        public static double deltaX = 0, deltaY = 0, zoom = 1;
        public static double normalizeX(double z)
        {
            return (double)((int)(z * (height * aspect * pixelAspect / 2))) / (height * aspect * pixelAspect / 2);
        }

        public static double normalizeY(double z)
        {
            return (double)((int)(z * (height / 2))) / (height / 2);
        }

        public static double iToX(int i)
        {
            double x = ((double)i / width) * 2 - 1;
            x *= aspect * pixelAspect;
            if (Program.followBody)
            {
                try
                {
                    Body followedBody = DrawFigure.bodies[Program.followingBodyNum];
                    deltaX = followedBody.X0;
                    return x * zoom + deltaX;
                }catch(Exception e) { }
            }
            return (x + deltaX) * zoom;
        }

        public static double jToY(int j)
        {
            double y = ((double)(height - 1 - j) / height) * 2 - 1;
            if (Program.followBody)
            {
                try
                {
                    Body followedBody = DrawFigure.bodies[Program.followingBodyNum];
                    deltaY = followedBody.Y0;
                    return y * zoom + deltaY;
                }
                catch (Exception e) { }
            }
            return (y + deltaY) * zoom;
        }
    }
}
