using SFML.Graphics;
using SFML.System;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Threading;

namespace graphic_lab3 {
	internal class Program {

        static uint width = 800;
        static uint height = 800;
        static uint offset = 20;

        static SFML.Graphics.Color[,] colors = new SFML.Graphics.Color[width, height];


        static void drawLine(Vector2f start, Vector2f end, SFML.Graphics.Color color) {
            if (start.X > end.X || (start.X == end.X && start.Y > end.Y))
            {
                (start, end) = (end, start);
            }

            double k = (end.Y - start.Y) / (end.X - start.X);
            double x = start.X;
            double y = start.Y;

            if (k == Double.PositiveInfinity || k == Double.NegativeInfinity)
            {
                while (y < Math.Round(end.Y))
                {
                    colors[(int)Math.Round(x), (int)Math.Round(y)] = color;
                    y++;
                }
            }
            else
            {
                while (x < Math.Round(end.X))
                {
                    int startI = (int)Math.Round(Math.Min((int)Math.Round(y), Math.Round(y + k)));
                    int endI = (int)Math.Round(Math.Max((int)Math.Round(y), Math.Round(y + k)));
                    for (int i = startI; i <= endI; i++) {
                        colors[(int)Math.Round(x), i] = color;
                    }
                    y += k;
                    x++;
                }
            }
        }

        static void metricDdaLine(Vector2f start, Vector2f end, SFML.Graphics.Color color) {
            var checkpoint = DateTime.Now.Ticks;
            ddaLine(start, end, color);
            Console.WriteLine("DDA Line ticks: " + (DateTime.Now.Ticks - checkpoint).ToString());
        }

        static void metricDrawLine(Vector2f start, Vector2f end, SFML.Graphics.Color color)
        {
            var checkpoint = DateTime.Now.Ticks;
            drawLine(start, end, color);
            Console.WriteLine("Draw Line ticks: " + (DateTime.Now.Ticks - checkpoint).ToString());
        }

        static void metricRecursiveFill(Vector2i startPoint, SFML.Graphics.Color fillColor, SFML.Graphics.Color borderColor) {
            var checkpoint = DateTime.Now.Ticks;
            recursiveFill(startPoint, fillColor, borderColor);
            Console.WriteLine("Recursive fill ticks: " + (DateTime.Now.Ticks - checkpoint).ToString());
        }

        static void metricScanningLineFill(Vector2i startPoint, SFML.Graphics.Color fillColor, SFML.Graphics.Color borderColor)
        {
            var checkpoint = DateTime.Now.Ticks;
            scanningLineFill(startPoint, fillColor, borderColor);
            Console.WriteLine("Scanning line fill ticks: " + (DateTime.Now.Ticks - checkpoint).ToString());
        }

        static void ddaLine(Vector2f start, Vector2f end, SFML.Graphics.Color color) {
            Vector2i startRounded = new Vector2i((int)Math.Round(start.X), (int) Math.Round(start.Y));
            Vector2i endRounded = new Vector2i((int)Math.Round(end.X), (int)Math.Round(end.Y));
            int L = Math.Max(Math.Abs(endRounded.X - startRounded.X), Math.Abs(endRounded.Y - startRounded.Y));

            float dx = (end.X - start.X) / L, dy = (end.Y -  start.Y) / L;
            float x = start.X, y = start.Y;
            for (int i = 0; i < L; i++) {
                colors[(int)Math.Round(x), (int)Math.Round(y)] = color;
                x += dx;
                y += dy;
            }
        }

        static void recursiveFill(Vector2i startPoint, SFML.Graphics.Color fillColor, SFML.Graphics.Color borderColor) {
            Stack<Vector2i> points = new Stack<Vector2i>();
            points.Push(startPoint);
            while (points.Count != 0) {
                var point = points.Pop();
                if (point.X >= colors.GetLength(0) || point.X < 0) {
                    continue;
                }
                if (point.Y >= colors.GetLength(1) || point.Y < 0) {
                    continue;
                }
                if (colors[point.X, point.Y] == borderColor || colors[point.X, point.Y] == fillColor) {
                    continue;
                }
                colors[point.X, point.Y] = fillColor;
                points.Push(new Vector2i(point.X-1, point.Y));
                points.Push(new Vector2i(point.X+1, point.Y));
                points.Push(new Vector2i(point.X, point.Y+1));
                points.Push(new Vector2i(point.X, point.Y-1));
            }   
        }

        static void scanningLineFill(Vector2i startPoint, SFML.Graphics.Color fillColor, SFML.Graphics.Color borderColor) {
            int left = startPoint.X, right = startPoint.X;
            while (left > 0 && colors[left, startPoint.Y] != borderColor)
                left--;
            while (right < colors.GetLength(0) && colors[right, startPoint.Y] != borderColor)
                right++;
            ddaLine(new Vector2f(left, startPoint.Y), new Vector2f(right, startPoint.Y), fillColor);
            for (int i = left; i < right; i++) {
                if (colors.GetLength(1) - 1 > startPoint.Y && colors[i, startPoint.Y + 1] != borderColor && colors[i, startPoint.Y + 1] != fillColor)
                    scanningLineFill(new Vector2i(i, startPoint.Y + 1), fillColor, borderColor);
                if (startPoint.Y >= 1 && colors[i, startPoint.Y - 1] != borderColor && colors[i, startPoint.Y - 1] != fillColor)
                    scanningLineFill(new Vector2i(i, startPoint.Y + 1), fillColor, borderColor);
            }
        }

        static void CreateTriangle(Vector2f[] points, Action<Vector2f, Vector2f, SFML.Graphics.Color> drawLine, Action<Vector2i, SFML.Graphics.Color, SFML.Graphics.Color> fillArea)
        {
            if (points.Length != 3)
            {
                Console.WriteLine("Треугольник строится по 3 точкам");
                return;
            }

            drawLine?.Invoke(points[0], points[1], SFML.Graphics.Color.Red);
            drawLine?.Invoke(points[2], points[1], SFML.Graphics.Color.Red);
            drawLine?.Invoke(points[2], points[0], SFML.Graphics.Color.Red);

            Vector2i center = (Vector2i)GetMediansIntersection(points);
            fillArea?.Invoke(center, SFML.Graphics.Color.Yellow, SFML.Graphics.Color.Red);
        }

        static Vector2f GetLineMiddle(Vector2f start, Vector2f end)
        {
            float midX = (start.X + end.X) / 2;
            float midY = (start.Y + end.Y) / 2;
            return new Vector2f(midX, midY);
        }

        static List<Vector2f[]> SplitTriangleWithMedians(Vector2f[] points)
        {

            List<Vector2f[]> triangles = new List<Vector2f[]>();

            Vector2f center = GetMediansIntersection(points);

            Vector2f[] sideMiddles = {
                GetLineMiddle(points[0], points[1]),
                GetLineMiddle(points[1], points[2]),
                GetLineMiddle(points[2], points[0])
            };

            Vector2f currentMiddle = sideMiddles[2];
            for (int i=0; i<points.Length; i++)
            {
                Vector2f[] triangle1 =
                {
                    center,
                    points[i],
                    currentMiddle
                };

                triangles.Add(triangle1);

                currentMiddle = sideMiddles[i];

                Vector2f[] triangle2 =
                {
                    center,
                    points[i],
                    currentMiddle
                };

                triangles.Add(triangle2);
            }

            return triangles;
        }

        static Vector2f GetMediansIntersection(Vector2f[] points)
        {
            if (points.Length != 3)
            {
                Console.WriteLine("Это не координаты вершин треугольника!");
                return new Vector2f(0, 0);
            }

            float intesectionX = (points[0].X + points[1].X + points[2].X) / 3;
            float intesectionY = (points[0].Y + points[1].Y + points[2].Y) / 3;
            return new Vector2f(intesectionX, intesectionY);
        }

        static void Main(string[] args)
		{
            ConsoleInput input = new ConsoleInput();
            input.RequestData();
            Vector2f[] pointsTriangleA = input.GetTrianglePoints(new Vector2f(offset, height - offset));
            Vector2f[] pointsTriangleB = input.GetTrianglePoints(new Vector2f(offset * 2 + input.baseLength, height - offset));
            var renderWindow = new RenderWindow(new SFML.Window.VideoMode(width, height), "Test");
            renderWindow.SetVerticalSyncEnabled(true);

            List<Vector2f[]> triangles = SplitTriangleWithMedians(pointsTriangleA);

            List<Vector2f> centers = new List<Vector2f>();


            CreateTriangle(pointsTriangleA, drawLine, recursiveFill);
            foreach (var triangle in triangles)
            {
                centers.Add(GetMediansIntersection(triangle));
                List<Vector2f[]> microTriangles = SplitTriangleWithMedians(triangle);
                foreach (var microTriangle in microTriangles)
                {
                    CreateTriangle(microTriangle, drawLine, recursiveFill);
                }
            }

            ddaLine(centers[5], centers[0], SFML.Graphics.Color.Blue);
            ddaLine(centers[0], centers[1], SFML.Graphics.Color.Blue);
            ddaLine(centers[1], centers[2], SFML.Graphics.Color.Blue);
            ddaLine(centers[2], centers[3], SFML.Graphics.Color.Blue);
            ddaLine(centers[3], centers[4], SFML.Graphics.Color.Blue);
            ddaLine(centers[4], centers[5], SFML.Graphics.Color.Blue);


            foreach (var triangle in triangles)
            {
                List<Vector2f[]> microTriangles = SplitTriangleWithMedians(triangle);
                foreach (var microTriangle in microTriangles)
                {
                    List<Vector2f[]> microMicroTriangles = SplitTriangleWithMedians(microTriangle);
                    foreach (var microMicroTriangle in microMicroTriangles)
                    {
                        List<Vector2f[]> microMicroMicroTriangles = SplitTriangleWithMedians(microMicroTriangle);
                        foreach (var microMicroMicroTriangle in microMicroMicroTriangles)
                        {
                            CreateTriangle(microMicroMicroTriangle, ddaLine, metricScanningLineFill);
                        }
                    }
                }
            }


            Image image = new Image(colors);
            Texture texture = new Texture(image);
            Sprite sprite = new Sprite(texture);

            while (renderWindow.IsOpen)
            {
                renderWindow.DispatchEvents();
                renderWindow.Clear(SFML.Graphics.Color.Black);
                renderWindow.Draw(sprite);
                renderWindow.Display();
            }
        }
    }
}
