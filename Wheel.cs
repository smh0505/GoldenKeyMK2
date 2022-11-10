using System.Numerics;
using Raylib_cs;

namespace GoldenKeyMK2
{
    public class Wheel
    {
        public static List<Option> Options = new List<Option>();
        public static List<string> WaitingOptions = new List<string>();
        private static readonly Random Rnd = new Random();

        public static int Sum
        {
            get
            {
                int sum = 0;
                foreach (var option in Options) sum += option.Count;
                return sum;
            }
        }

        public static void UpdateWheel()
        {
            if (!Program.Switches["IsSpinning"] && !Program.Switches["OptionSelected"] && WaitingOptions.Count > 0)
            {
                foreach (var option in WaitingOptions) Wheel.AddOption(option);
                WaitingOptions.Clear();
            }
        }

        public static void TriggerWheel()
        {
            if (!Program.Switches["IsExiting"] && Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
            {
                if (!Program.Switches["IsSpinning"] && !Program.Switches["OptionSelected"]) Program.Switches["IsSpinning"] = true;
                else if (!Program.Switches["StopTriggered"]) Program.Switches["StopTriggered"] = true;
            }
        }

        public static void NewOption(string name)
        {
            var color = new Color(Rnd.Next(256), Rnd.Next(256), Rnd.Next(256), 255);
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

        public static void RemoveOption(Option option)
        {
            int id = Options.IndexOf(option);
            Options.Remove(option);
            if (option.Count > 1)
            {
                var newOption = new Option(option.Name, option.Color, option.Count - 1);
                Options.Insert(id, newOption);
            }
        }

        public static Option Result(float angle)
        {
            if (Options.Count > 0)
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
                return target;
            }
            return new Option(string.Empty, Color.WHITE, 1);
        }

        public static void PrintOption(float angle)
        {
            Option target = Result(angle);
            int x = 440 - (int)(Raylib.MeasureTextEx(Program.DefaultFont, target.Name, 48, 0).X / 2);
            Raylib.DrawTextEx(Program.DefaultFont, target.Name, new Vector2(x, 6), 48, 0, Color.BLACK);
        }

        public static void DrawWheel(float startAngle)
        {
            var center = new Vector2(440, 380);
            var currAngle = startAngle;
            foreach (var option in Options)
            {
                Raylib.DrawCircleSector(center, 300, currAngle, currAngle + 360f / Sum * option.Count, 0, Raylib.Fade(option.Color, 0.5f));
                Raylib.DrawCircleSectorLines(center, 300, currAngle, currAngle + 360f / Sum * option.Count, 0, option.Color);
                currAngle += 360f / Sum * option.Count;
            }

            Raylib.DrawTriangle(new Vector2(420, 60), new Vector2(440, 100), new Vector2(460, 60), Color.BLACK);
        }

        public static float RotateWheel(float startAngle, float rotateAngle)
        {
            float a = startAngle;
            if (Program.Switches["IsSpinning"])
            {
                a -= rotateAngle;
                if (a < 0) a += 360f;
            }
            return a;
        }
    }
}