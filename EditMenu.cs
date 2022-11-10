using System.Numerics;
using Raylib_cs;

namespace GoldenKeyMK2
{
    public class EditMenu
    {
        private static int _idx = 0;

        public static void DrawEdit()
        {
            Raylib.DrawRectangle(40, 40, 800, 640, Raylib.Fade(Color.DARKGRAY, 0.7f));
            Raylib.DrawTextEx(Program.DefaultFont, "수정 중...", new Vector2(56, 56), 48, 0, Color.WHITE);

            Raylib.DrawRectangle(56, 120, 768, 176, Color.WHITE);
            int before = _idx == 0 ? Wheel.Options.Count - 1 : _idx - 1;
            int after = _idx == Wheel.Options.Count - 1 ? 0 : _idx + 1;
            Raylib.DrawRectangle(64, 128, 752, 48, Raylib.Fade(Wheel.Options[before].Color, 0.5f));
            Raylib.DrawRectangle(64, 184, 752, 48, Raylib.Fade(Wheel.Options[_idx].Color, 0.5f));
            Raylib.DrawRectangle(64, 240, 752, 48, Raylib.Fade(Wheel.Options[after].Color, 0.5f));
            Raylib.DrawTextEx(Program.DefaultFont, Wheel.Options[before].Name + " x " + Wheel.Options[before].Count.ToString(),
                new Vector2(72, 136), 32, 0, Color.BLACK);
            Raylib.DrawTextEx(Program.DefaultFont, Wheel.Options[_idx].Name + " x " + Wheel.Options[_idx].Count.ToString(),
                new Vector2(72, 192), 32, 0, Color.BLACK);
            Raylib.DrawTextEx(Program.DefaultFont, Wheel.Options[after].Name + " x " + Wheel.Options[after].Count.ToString(),
                new Vector2(72, 248), 32, 0, Color.BLACK);
            Raylib.DrawTriangle(new Vector2(56, 192), new Vector2(56, 224), new Vector2(72, 208), Color.BLACK);
            Raylib.DrawTriangle(new Vector2(824, 192), new Vector2(808, 208), new Vector2(824, 224), Color.BLACK);

        }

        public static void Control()
        {
            switch ((KeyboardKey)Raylib.GetKeyPressed())
            {
                case KeyboardKey.KEY_UP:
                    _idx = _idx == 0 ? Wheel.Options.Count - 1 : _idx - 1;
                    break;
                case KeyboardKey.KEY_DOWN:
                    _idx = _idx == Wheel.Options.Count - 1 ? 0 : _idx + 1;
                    break;
            }
        }
    }
}