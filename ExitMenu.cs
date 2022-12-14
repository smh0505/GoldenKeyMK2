using System.Numerics;
using Newtonsoft.Json;
using Raylib_cs;

namespace GoldenKeyMK2
{
    public class ExitMenu
    {
        private static readonly Rectangle ExitButton = new Rectangle(448, 400, 160, 64);
        private static readonly Rectangle CancelButton = new Rectangle(672, 400, 160, 64);

        private static Color _exitColor;
        private static Color _cancelColor;

        public static void DrawExit()
        {
            Raylib.DrawRectangle(0, 0, 1280, 720, Raylib.Fade(Color.BLACK, 0.7f));
            Raylib.DrawRectangle(0, 240, 1280, 240, Color.BLACK);

            var text1 = "프로그램을 종료하시겠습니까?";
            var text2 = "종료시 현재 진행 상황이 저장됩니다.";
            Raylib.DrawTextEx(Program.DefaultFont, text1, new Vector2(640 - Raylib.MeasureTextEx(Program.DefaultFont, text1, 48, 0).X / 2, 256), 48, 0, Color.WHITE);
            Raylib.DrawTextEx(Program.DefaultFont, text2, new Vector2(640 - Raylib.MeasureTextEx(Program.DefaultFont, text2, 32, 0).X / 2, 312), 32, 0, Color.WHITE);

            DetectCursor();
            var text3 = "네";
            var text4 = "아니요";
            Raylib.DrawRectangle(448, 400, 160, 64, _exitColor);
            Raylib.DrawRectangle(672, 400, 160, 64, _cancelColor);
            Raylib.DrawTextEx(Program.DefaultFont, text3, new Vector2(528 - Raylib.MeasureTextEx(Program.DefaultFont, text3, 32, 0).X / 2, 416), 32, 0, Color.BLACK);
            Raylib.DrawTextEx(Program.DefaultFont, text4, new Vector2(752 - Raylib.MeasureTextEx(Program.DefaultFont, text4, 32, 0).X / 2, 416), 32, 0, Color.BLACK);
        }

        private static void DetectCursor()
        {
            if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), ExitButton))
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    Program.Setting.Records.Clear();
                    foreach (var option in Wheel.Options)
                        for (int i = 0; i < option.Count; i++)
                            Program.Setting.Records.Add(option.Name);
                    StreamWriter w = new StreamWriter("default.json");
                    w.Write(JsonConvert.SerializeObject(Program.Setting));
                    w.Close();
                    Program.Halt = true;
                }
                else _exitColor = Color.RED;
            }
            else _exitColor = Raylib.Fade(Color.RED, 0.5f);

            if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), CancelButton))
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) Program.Switches["IsExiting"] = false;
                else _cancelColor = Color.GREEN;
            }
            else _cancelColor = Raylib.Fade(Color.GREEN, 0.5f);
        }
    }
}