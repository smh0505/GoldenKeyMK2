using System.Numerics;
using Raylib_cs;

namespace GoldenKeyMK2
{
    public class Panel
    {
        private static int _y;
        private static int _frameCount;
        private static int _frameCount2;
        private static int _index;
        private static float _fadeCount = 1f;

        public static Color EditColor;
        public static Color SaveColor;
        public static Rectangle EditButton = new Rectangle(0, 68, 80, 24);
        public static Rectangle SaveButton = new Rectangle(0, 96, 80, 24);

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
                _frameCount = 0;
                _y = 0;
                _index = 0;
                Raylib.DrawRectangle(880, 0 + 24 * optionList.IndexOf(option), 400, 24, Raylib.Fade(option.Color, 0.5f));
                Raylib.DrawTextEx(Program.DefaultFont, option.Name + " x " + option.Count.ToString(), 
                    new Vector2(884, 4 + 24 * optionList.IndexOf(option)), 16, 0, Color.BLACK);
            }
            else Marquee(optionList);
        }

        public static void DrawControl()
        {
            string text1 = string.Empty;
            switch (Program.State)
            {
                case GameState.Idle:
                    text1 = "스페이스바를 눌러 돌림판 돌리기";
                    Raylib.DrawTextEx(Program.DefaultFont, "Tab키를 눌러 수정 메뉴 열기", new Vector2(8, 680), 16, 0, Color.GRAY);
                    break;
                case GameState.Editing:
                    text1 = "Tab키를 눌러 수정 메뉴 닫기";
                    break;
                case GameState.Spinning:
                    text1 = "스페이스바를 눌러 돌림판 멈추기";
                    break;
            }
            Raylib.DrawTextEx(Program.DefaultFont, text1, new Vector2(8, 696), 16, 0, Color.GRAY);
            if (Program.State == GameState.Idle) DrawButtonsIdle();
        }

        private static void DrawButtonsIdle()
        {
            Raylib.DrawRectangle(0, 68, 80, 24, EditColor);
            Raylib.DrawRectangle(0, 96, 80, 24, SaveColor);
            Raylib.DrawTextEx(Program.DefaultFont, "수정",
                new Vector2(40 - Raylib.MeasureTextEx(Program.DefaultFont, "수정", 16, 0).X / 2, 72), 16, 0, Color.BLACK);
            Raylib.DrawTextEx(Program.DefaultFont, "저장",
                new Vector2(40 - Raylib.MeasureTextEx(Program.DefaultFont, "저장", 16, 0).X / 2, 100), 16, 0, Color.BLACK);
        }

        private static void Marquee(List<Option> optionList)
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

        public static void Surprise()
        {
            _frameCount2++;
            if (_frameCount2 % 2 == 0) _fadeCount -= 0.01f;
            var current = Wheel.Result(Program.Angle);
            var currVec = Raylib.MeasureTextEx(Program.DefaultFont, current.Name, 64, 0);
            Raylib.DrawRectangle(0, 0, 880, 720, Raylib.Fade(Color.BLACK, _fadeCount));
            Raylib.DrawTextEx(Program.DefaultFont, current.Name,
                new Vector2(440 - currVec.X / 2, 360 - currVec.Y), 64, 0, Raylib.Fade(Color.WHITE, _fadeCount));

            if (_fadeCount <= 0)
            {
                _frameCount2 = 0;
                _fadeCount = 1f;
                Wheel.RemoveOption(current);
                Program.State = GameState.Idle;
                Program.Theta = 50;
            }
        }
    }
}