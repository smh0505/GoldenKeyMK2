using System.Numerics;
using Raylib_cs;

namespace GoldenKeyMK2
{
    public class Panel
    {
        private static int _y = 0;
        private static int _frameCount = 0;
        private static int _index = 0;

        private static void CountIndex(int count)
        {
            _frameCount++;
            if (_frameCount == 3) 
            {
                _frameCount = 0;
                _y -= 2;
            }
            if (_y == -24)
            {
                _y = 0;
                _index++;
            }
            if (_index == count) _index = 0;
        }

        public static void DrawPanels()
        {
            var optionList = Wheel.Options;
            if (optionList.Count <= 30) foreach (var option in optionList)
            {
                Raylib.DrawRectangle(880, 0 + 24 * optionList.IndexOf(option), 400, 24, Raylib.Fade(option.Color, 0.5f));
                Raylib.DrawTextEx(Program.DefaultFont, option.Name + " x " + option.Count.ToString(), 
                    new Vector2(884, 4 + 24 * optionList.IndexOf(option)), 16, 0, Color.BLACK);
            }
            else 
            {
                CountIndex(optionList.Count);

                var temp = new Option[31];
                for (int i = 0; i < 31; i++)
                {
                    if (i + _index < optionList.Count) temp[i] = optionList[i + _index];
                    else temp[i] = optionList[i + _index - optionList.Count];
                }

                for (int i = 0; i < 31; i++)
                {
                    Raylib.DrawRectangle(880, _y + 24 * i, 400, 24, Raylib.Fade(temp[i].Color, 0.5f));
                    Raylib.DrawTextEx(Program.DefaultFont, temp[i].Name + " x " + temp[i].Count.ToString(), 
                        new Vector2(884, _y + 4 + 24 * i), 16, 0, Color.BLACK);
                }
            }
            if (!Program.IsSpinning) 
                Raylib.DrawTextEx(Program.DefaultFont, "스페이스바를 눌러 돌림판 돌리기", new Vector2(8, 696), 16, 0, Color.GRAY);
            else if (!Program.StopTriggered) 
                Raylib.DrawTextEx(Program.DefaultFont, "스페이스바를 눌러 돌림판 멈추기", new Vector2(8, 696), 16, 0, Color.GRAY);
        }
    }
}