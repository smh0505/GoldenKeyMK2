using System.Numerics;
using Raylib_cs;

namespace GoldenKeyMK2
{
    public class SaveMenu
    {
        private static readonly Rectangle SaveButton = new Rectangle(448, 400, 160, 64);
        private static readonly Rectangle CancelButton = new Rectangle(672, 400, 160, 64);

        private static Color _saveColor;
        private static Color _cancelColor;

        public static void DrawSave()
        {
            Raylib.DrawRectangle(0, 0, 1280, 720, Raylib.Fade(Color.BLACK, 0.7f));
            Raylib.DrawRectangle(0, 240, 1280, 240, Color.BLACK);

            var text1 = "현재 상황을 저장하시겠습니까?";
            var text2 = "기본 세팅으로 저장합니다. 이미 있는 세팅은 덮어씌워집니다.";
            Raylib.DrawTextEx(Program.DefaultFont, text1, new Vector2(640 - Raylib.MeasureTextEx(Program.DefaultFont, text1, 48, 0).X / 2, 256), 48, 0, Color.WHITE);
            Raylib.DrawTextEx(Program.DefaultFont, text2, new Vector2(640 - Raylib.MeasureTextEx(Program.DefaultFont, text2, 32, 0).X / 2, 312), 32, 0, Color.WHITE);

            if (!Program.Switches["IsExiting"]) DetectCursor();
            var text3 = "네";
            var text4 = "아니요";
            Raylib.DrawRectangle(448, 400, 160, 64, _saveColor);
            Raylib.DrawRectangle(672, 400, 160, 64, _cancelColor);
            Raylib.DrawTextEx(Program.DefaultFont, text3, new Vector2(528 - Raylib.MeasureTextEx(Program.DefaultFont, text3, 32, 0).X / 2, 416), 32, 0, Color.BLACK);
            Raylib.DrawTextEx(Program.DefaultFont, text4, new Vector2(752 - Raylib.MeasureTextEx(Program.DefaultFont, text4, 32, 0).X / 2, 416), 32, 0, Color.BLACK);
        }

        private static void DetectCursor()
        {
            if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), SaveButton))
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    Program.Setting.Values.Clear();
                    foreach (var option in Wheel.Options)
                        for (int i = 0; i < option.Count; i++)
                            Program.Setting.Records.Add(option.Name);
                    Program.State = GameState.Idle;
                }
                else _saveColor = Color.RED;
            }
            else _saveColor = Raylib.Fade(Color.RED, 0.5f);

            if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), CancelButton))
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) Program.State = GameState.Idle;
                else _cancelColor = Color.GREEN;
            }
            else _cancelColor = Raylib.Fade(Color.GREEN, 0.5f);
        }
    }
}