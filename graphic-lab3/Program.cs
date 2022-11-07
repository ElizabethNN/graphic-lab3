using SFML.Graphics;
using SFML.System;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Threading;

namespace graphic_lab3 {
	internal class Program {

        static Color[,] colors = new SFML.Graphics.Color[800, 800];

        static void drawLine(Vector2i start, Vector2i end, Color color) {

            double k = (end.Y - start.Y) * 1.0 / (end.X - start.X);
            double x = start.X;
            double y = start.Y;
            if (k == Double.PositiveInfinity || k == Double.NegativeInfinity)
            {
                while (y != end.Y)
                {
                    colors[(int)Math.Round(x), (int)Math.Round(y)] = color;
                    y++;
                }
            }
            else
            {
                while (x != end.X)
                {
                    colors[(int)Math.Round(x), (int)Math.Round(y)] = color;
                    y += k;
                    x++;
                }
            }
        }

        static void ddaLine(Vector2f start, Vector2f end, Color color) {

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

        static void recursiveFill(Vector2i startPoint, Color fillColor, Color borderColor) {
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

        static void scanningLineFill(Vector2i start, Color fillColor, Color borderColor) {

            int left = start.X, right = start.X;
            while (left > 0 && colors[left, start.Y] != borderColor)
                left--;
            while (right < colors.GetLength(0) && colors[right, start.Y] != borderColor)
                right++;
            drawLine(new Vector2i(left, start.Y), new Vector2i(right, start.Y), fillColor);
            for (int i = left; i < right; i++) {
                if (colors.GetLength(1) - 1 > start.Y && colors[i, start.Y + 1] != borderColor && colors[i, start.Y + 1] != fillColor)
                    scanningLineFill(new Vector2i(i, start.Y + 1), fillColor, borderColor);
                if (start.Y >= 1 && colors[i, start.Y - 1] != borderColor && colors[i, start.Y - 1] != fillColor)
                    scanningLineFill(new Vector2i(i, start.Y + 1), fillColor, borderColor);
            }
        }

        static void Main(string[] args)
		{
            var renderWindow = new RenderWindow(new SFML.Window.VideoMode(800, 800), "Test");
            renderWindow.SetVerticalSyncEnabled(true);

            ddaLine(new Vector2f(100, 100), new Vector2f(200, 100), Color.Red);
            drawLine(new Vector2i(100, 200), new Vector2i(200, 200), Color.Red);

            Image image = new Image(colors);
            Texture texture = new Texture(image);
            Sprite sprite = new Sprite(texture);

            while (renderWindow.IsOpen)
            {
                renderWindow.DispatchEvents();
                renderWindow.Clear(Color.Black);
                renderWindow.Draw(sprite);
                renderWindow.Display();
            }
        }
    }
}
