using System.Numerics;
using Raylib_cs;

namespace GoldenKeyMK2
{
    public class EditMenu
    {
        public static int Idx;

        private static readonly Rectangle ModeButton = new Rectangle(56, 616, 120, 48);
        private static bool _mode;
        private static string _modeString = string.Empty;
        private static string _optionName = String.Empty;
        private static Color _modeColor;
        private static int _optionCount = 1;

        public static void DrawEdit()
        {
            // Background
            Raylib.DrawRectangle(40, 40, 800, 640, Raylib.Fade(Color.DARKGRAY, 0.7f));
            Raylib.DrawTextEx(Program.DefaultFont, _modeString + " 중...", new Vector2(56, 56), 48, 0, Color.WHITE);

            // Updating Idx
            Index(out int before, out int after);

            // Listing 3 Items based on Idx
            Raylib.DrawRectangle(56, 120, 768, 176, Color.WHITE);
            Raylib.DrawRectangle(64, 128, 752, 48, Wheel.Options.Count > 0 ? Raylib.Fade(Wheel.Options[before].Color, 0.5f) : Color.DARKGRAY);
            Raylib.DrawRectangle(64, 184, 752, 48, Wheel.Options.Count > 0 ? Raylib.Fade(Wheel.Options[Idx].Color, 0.5f) : Color.DARKGRAY);
            Raylib.DrawRectangle(64, 240, 752, 48, Wheel.Options.Count > 0 ? Raylib.Fade(Wheel.Options[after].Color, 0.5f) : Color.DARKGRAY);
            if (Wheel.Options.Count > 0)
            {
                Raylib.DrawTextEx(Program.DefaultFont, Wheel.Options[before].Name + " x " + Wheel.Options[before].Count.ToString(),
                    new Vector2(72, 136), 32, 0, Color.BLACK);
                Raylib.DrawTextEx(Program.DefaultFont, Wheel.Options[Idx].Name + " x " + Wheel.Options[Idx].Count.ToString(),
                    new Vector2(72, 192), 32, 0, Color.BLACK);
                Raylib.DrawTextEx(Program.DefaultFont, Wheel.Options[after].Name + " x " + Wheel.Options[after].Count.ToString(),
                    new Vector2(72, 248), 32, 0, Color.BLACK);
            }
            Raylib.DrawTriangle(new Vector2(56, 192), new Vector2(56, 224), new Vector2(72, 208), Color.BLACK);
            Raylib.DrawTriangle(new Vector2(824, 192), new Vector2(808, 208), new Vector2(824, 224), Color.BLACK);

            // _optionName panel
            Raylib.DrawTextEx(Program.DefaultFont, _modeString + "할 항목 이름", new Vector2(56, 312), 16, 0, Color.WHITE);
            Raylib.DrawRectangle(56, 344, 768, 64, Color.BLACK);
            Raylib.DrawRectangle(60, 348, 760, 56, Color.WHITE);
            Raylib.DrawTextEx(Program.DefaultFont, _optionName + "_", new Vector2(64, 352), 48, 0, Color.BLACK);

            // _optionCount panel
            Raylib.DrawTextEx(Program.DefaultFont, _modeString + "할 칸 수", new Vector2(56, 424), 16, 0, Color.WHITE);
            Raylib.DrawRectangle(56, 456, 120, 64, Color.BLACK);
            Raylib.DrawRectangle(60, 460, 112, 56, Color.WHITE);
            Raylib.DrawTextEx(Program.DefaultFont, _optionCount.ToString(),
                new Vector2(64 + 52 - Raylib.MeasureTextEx(Program.DefaultFont, _optionCount.ToString(), 48, 0).X / 2, 464), 48, 0, Color.BLACK);

            // Text
            Raylib.DrawTextEx(Program.DefaultFont, "상/하 방향키: 이전/다음 항목", new Vector2(192, 456), 16, 0, Color.WHITE);
            Raylib.DrawTextEx(Program.DefaultFont, "좌/우 방향키: 개수 줄이기/늘이기", new Vector2(192, 480), 16, 0, Color.WHITE);
            Raylib.DrawTextEx(Program.DefaultFont, "Enter키를 누르면 " + _modeString + "됩니다.", new Vector2(192, 504), 16, 0, Color.WHITE);

            // _modeButton
            Raylib.DrawTextEx(Program.DefaultFont, "모드 변경 버튼", new Vector2(56, 584), 16, 0, Color.WHITE);
            Raylib.DrawRectangle(56, 616, 120, 48, _modeColor);
            Raylib.DrawTextEx(Program.DefaultFont, _modeString,
                new Vector2(116 - Raylib.MeasureTextEx(Program.DefaultFont, _modeString, 32, 0).X / 2, 624), 32, 0, Color.BLACK);
        }

        public static void ResetOptionIndex()
        {
            if (Idx < 0) Idx = 0;
            else if (Idx >= Wheel.Options.Count) Idx = Wheel.Options.Count - 1;
        }

        public static void ResetOptionName()
        {
            _optionName = (_mode && Wheel.Options.Any()) ? Wheel.Options[Idx].Name : string.Empty;
            _optionCount = (_mode && Wheel.Options.Any()) ? Wheel.Options[Idx].Count : 1;
        }

        public static void Control()
        {
            switch ((KeyboardKey)Raylib.GetKeyPressed())
            {
                case KeyboardKey.KEY_UP:
                    if (Wheel.Options.Count > 0) Idx = Idx == 0 ? Wheel.Options.Count - 1 : Idx - 1;
                    ResetOptionName();
                    break;
                case KeyboardKey.KEY_DOWN:
                    if (Wheel.Options.Count > 0) Idx = Idx == Wheel.Options.Count - 1 ? 0 : Idx + 1;
                    ResetOptionName();
                    break;
                case KeyboardKey.KEY_LEFT:
                    if (_mode) _optionCount = _optionCount == 0 ? 0 : _optionCount - 1;
                    else _optionCount = _optionCount == 1 ? 1 : _optionCount - 1;
                    break;
                case KeyboardKey.KEY_RIGHT:
                    _optionCount++;
                    break;
                case KeyboardKey.KEY_BACKSPACE:
                    if (_optionName.Length > 0) _optionName = _optionName.Remove(_optionName.Length - 1);
                    break;
                case KeyboardKey.KEY_ENTER:
                    if (_mode) Modify();
                    else for (int _ = 0; _ < _optionCount; _++) Wheel.WaitingOptions.Add(_optionName);
                    Program.Switches["IsEditing"] = false;
                    break;
                default:
                    int x = Raylib.GetCharPressed();
                    if (x != 0) _optionName += ((char)x).ToString();
                    break;
            }

            if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), ModeButton))
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    _mode = !_mode;
                    ResetOptionName();
                }
                else _modeColor = Color.YELLOW;
            }
            else _modeColor = Raylib.Fade(Color.YELLOW, 0.5f);
            _modeString = _mode ? "수정" : "추가";
        }

        private static void Index(out int before, out int after)
        {
            if (Wheel.Options.Count > 0)
            {
                before = Idx == 0 ? Wheel.Options.Count - 1 : Idx - 1;
                after = Idx == Wheel.Options.Count - 1 ? 0 : Idx + 1;
            }
            else
            {
                before = 0;
                after = 0;
            }
        }

        private static void Modify()
        {
            if (Wheel.Options.Any(x => x.Name == _optionName))
            {
                int i = Wheel.Options.FindIndex(x => x.Name == _optionName);
                Color c = Wheel.Options[i].Color;
                Wheel.Options.RemoveAt(i);
                if (_optionCount != 0)
                {
                    Option newOption = new Option(_optionName, c, _optionCount);
                    Wheel.Options.Insert(i, newOption);
                }
            }
            else
            {
                Color c = Wheel.Options[Idx].Color;
                Wheel.Options.RemoveAt(Idx);
                if (_optionCount != 0)
                {
                    Option newOption = new Option(_optionName, c, _optionCount);
                    Wheel.Options.Insert(Idx, newOption);
                }
            }
        }
    }
}