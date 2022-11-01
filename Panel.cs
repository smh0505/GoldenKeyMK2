using System.Numerics;
using Raylib_cs;

namespace GoldenKeyMK2
{
    public class Panel
    {
        public static void DrawPanels(Font font)
        {
            var optionList = Wheel.Options;
            foreach (var option in optionList)
            {
                Raylib.DrawRectangle(880, 0 + 24 * optionList.IndexOf(option), 400, 24, Raylib.Fade(option.Color, 0.5f));
                Raylib.DrawTextEx(font, option.Name + " x " + option.Count.ToString(), 
                    new Vector2(884, 4 + 24 * optionList.IndexOf(option)), 16, 0, Color.BLACK);

                if (!Program.IsSpinning) 
                    Raylib.DrawTextEx(font, "스페이스바를 눌러 돌림판 돌리기", new Vector2(8, 696), 16, 0, Color.GRAY);
                else if (!Program.StopTriggered) 
                    Raylib.DrawTextEx(font, "스페이스바를 눌러 돌림판 멈추기", new Vector2(8, 696), 16, 0, Color.GRAY);
            }
        }
    }
}