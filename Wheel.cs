using System.Numerics;
using Raylib_cs;

namespace GoldenKeyMK2
{
    public class Wheel
    {
        public static List<Option> Options = new List<Option>();
        public static List<string> WaitingOptions = new List<string>();
        private static Random _rnd = new Random();

        public static int Sum
        {
            get
            {
                int sum = 0;
                foreach (var option in Options) sum += option.Count;
                return sum;
            }
        }

        public static void NewOption(string name)
        {
            var color = new Color(_rnd.Next(256), _rnd.Next(256), _rnd.Next(256), 255);
            var newOption = new Option(name, color, 1);
            Options.Add(newOption);
        }

        public static void AddOption(string name)
        {
            int id = -1;
            foreach (var option in Options) if (option.Name == name) id = Options.IndexOf(option);
            if (Options.Count == 0) NewOption(name);
            else if (id == -1) NewOption(name);
            else
            {
                var newOption = new Option(Options[id].Name, Options[id].Color, Options[id].Count + 1);
                Options.RemoveAt(id);
                Options.Insert(id, newOption);
            }
        }

        public static void RemoveOption()
        {
            
        }

        public static void PrintOption(float angle)
        {
            float tau = 540f - angle;
            int id = (int)Math.Floor((tau >= 360f ? tau - 360 : tau) / (360f / Sum));

            Option target = Options[0];
            int idCount = 0;
            foreach (var option in Options)
            {
                if (idCount > id) break;
                else 
                {
                    target = option;
                    idCount += option.Count;
                }
            }

            int x = 440 - (int)(Raylib.MeasureTextEx(Program.DefaultFont, target.Name, 48, 0).X / 2);

            Raylib.DrawTextEx(Program.DefaultFont, target.Name, new Vector2(x, 6), 48, 0, Color.BLACK);
        }

        public static void DrawWheel(float startAngle)
        {
            var center = new Vector2(440, 400);
            var currAngle = startAngle;
            foreach (var option in Options)
            {
                Raylib.DrawCircleSector(center, 300, currAngle, currAngle + 360f / Sum * option.Count, 0, Raylib.Fade(option.Color, 0.5f));
                Raylib.DrawCircleSectorLines(center, 300, currAngle, currAngle + 360f / Sum * option.Count, 0, option.Color);
                currAngle += 360f / Sum * option.Count;
            }

            Raylib.DrawTriangle(new Vector2(420, 60), new Vector2(440, 100), new Vector2(460, 60), Color.BLACK);
        }
    }
}