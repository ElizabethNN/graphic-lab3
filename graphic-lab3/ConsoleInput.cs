using System;
using SFML.Graphics;
using SFML.System;

namespace graphic_lab3
{
    public class ConsoleInput
    {
        public double angle { get; private set; }
        public uint baseLength { get; private set; }

        public ConsoleInput()
        {
            ClearData();
        }

        private void ClearData()
        {
            this.angle = 0;
            this.baseLength = 0;
        }

        public void RequestData()
        {
            while (true)
            {
                ClearData();

                try
                {
                    Console.Write("Введите длину основания: ");
                    baseLength = UInt32.Parse(Console.ReadLine());
                    Console.Write("Введите угол в градусах (от 0 до 90): ");
                    angle = Double.Parse(Console.ReadLine());
                    angle = angle * Math.PI / 180;
                    break;
                }
                catch
                {
                    Console.WriteLine();
                    Console.WriteLine("Входные данные должны быть положительными целыми числами");

                }
            }
        }

        public Vector2f[] GetTrianglePoints(Vector2f basePosition)
        {
            Vector2f[] points = {
                new Vector2f(basePosition.X, basePosition.Y),
                new Vector2f(basePosition.X + this.baseLength, basePosition.Y),
                new Vector2f(basePosition.X + this.baseLength / 2, (float)(basePosition.Y - Math.Tan(angle) * baseLength / 2))
            };

            return points;
        }
    }
}

