using System;
using System.Drawing;

namespace Dynamic_lines
{
    internal class Axis
    {
        private Point startPoint;
        private Point endPoint;
        public readonly double angle;
        private readonly double len;

        public Axis(Point startPoint, Point endPoint, double angle)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            this.angle = angle;

            len = Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) + Math.Pow(endPoint.Y - startPoint.Y, 2));
        }
        private Point GetDivPoint(Point start, int divIndex, int divStep)
        {
            int x;
            int y;

            if (start.X + start.Y < len)
            {
                x = start.X + (int)(divIndex * divStep * Math.Sin(angle));
                y = start.Y + (int)(divIndex * divStep * Math.Cos(angle));
            }
            else
            {
                x = start.X - (int)(divIndex * divStep * Math.Sin(angle));
                y = start.Y - (int)(divIndex * divStep * Math.Cos(angle));
            }

            return new Point(x, y);
        }

        public void Draw(ref Graphics g, ref Pen pen)
        {
            g.DrawLine(pen, startPoint, endPoint);
        }

        public void InvertAxis()
        {
            (startPoint, endPoint) = (endPoint, startPoint);
        }

        public static void MergeAxes(Axis first, Axis second, int divStep, ref Graphics g, ref Pen pen, bool isParty, bool[] enabled)
        {
            int counter = (int)(first.len / 2 / divStep);

            for (int i = 0; i < counter; i++)
            {
                if (isParty)
                {
                    Random rand = new Random();
                    Color color = Color.FromArgb(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));
                    pen.Color = color;
                }
                Point fEnd = second.GetDivPoint(second.startPoint, i, divStep);
                Point sEnd = first.GetDivPoint(first.startPoint, i, divStep);
                if (enabled[0]) g.DrawLine(pen, first.startPoint, fEnd);
                if (enabled[1]) g.DrawLine(pen, second.startPoint, sEnd);

                Point backFEnd = second.GetDivPoint(second.endPoint, i, divStep);
                Point backSEnd = first.GetDivPoint(first.endPoint, i, divStep);
                if (enabled[2]) g.DrawLine(pen, first.endPoint, backFEnd);
                if (enabled[3]) g.DrawLine(pen, second.endPoint, backSEnd);
            }
        }
    }
}
