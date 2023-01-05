using System.Numerics;
using System.Text.RegularExpressions;
using Raylib_cs;

namespace GoldenKeyMK2
{
    public class Login
    {
        public static string Payload;
        private static string _alert = "Enter 키를 눌러 연결합니다.";

        private static readonly Rectangle LoadDefault = new Rectangle(448, 400, 160, 64);
        private static readonly Rectangle LoadLog = new Rectangle(672, 400, 160, 64);

        public static async void GetPassword()
        {
            switch ((KeyboardKey)Raylib.GetKeyPressed())
            {
                case KeyboardKey.KEY_BACKSPACE:
                    if (Program.Input.Length > 0) Program.Input = Program.Input.Remove(Program.Input.Length - 1);
                    break;
                case KeyboardKey.KEY_V:
                    if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) || Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_CONTROL))
                        Program.Input += Raylib.GetClipboardText_();
                    break;
                case KeyboardKey.KEY_PAUSE:
                    Program.Switches["TextShowing"] = !Program.Switches["TextShowing"];
                    break;
                case KeyboardKey.KEY_ENTER:
                    Program.Switches["IsProcessing"] = true;
                    await LoadPayload(Program.Input);
                    if (!string.IsNullOrEmpty(Payload))
                    {
                        Program.CurrScreen = GameScreen.Wheel;
                        Program.Setting.Key = Program.Input;
                        Program.Connect();
                    }
                    else _alert = "연결에 실패했습니다. 다시 시도해주세요.";
                    Program.Switches["IsProcessing"] = false;
                    break;
                default:
                    var x = Raylib.GetCharPressed();
                    if (x != 0) Program.Input += ((char)x).ToString();
                    break;
            }
        }

        public static void DrawConnectScreen()
        {
            string secret = "".PadLeft(Program.Input.Length, '*');

            Raylib.DrawTextEx(Program.DefaultFont, "투네이션 통합 위젯 URL", new Vector2(12, 12), 32, 0, Color.BLACK);
            Raylib.DrawTextEx(Program.DefaultFont, "https://toon.at/widget/alertbox/", new Vector2(12, 44), 32, 0, Color.BLACK);

            Raylib.DrawRectangle(12, 76, 650, 40, Color.LIGHTGRAY);
            Raylib.DrawRectangleLines(12, 76, 650, 40, Color.DARKGRAY);
            Raylib.DrawTextEx(Program.DefaultFont,
                Program.Switches["TextShowing"]
                    ? Program.Input.Substring(Program.Input.Length >= 40 ? Program.Input.Length - 40 : 0,
                        Program.Input.Length >= 40 ? 40 : Program.Input.Length)
                    : secret.Substring(secret.Length >= 40 ? secret.Length - 40 : 0,
                        secret.Length >= 40 ? 40 : secret.Length),
                new Vector2(16, 80), 32, 0, Color.BLACK);

            Raylib.DrawTextEx(Program.DefaultFont, _alert, new Vector2(20, 124), 16, 0, Color.GRAY);
            Raylib.DrawTextEx(Program.DefaultFont, "Ctrl+V: 붙여넣기", new Vector2(20, 140), 16, 0, Color.GRAY);
            Raylib.DrawTextEx(Program.DefaultFont, "Pause: " + (Program.Switches["TextShowing"] ? "숨기기" : "표시하기"),
                new Vector2(20, 156), 16, 0, Color.GRAY);

            Raylib.DrawTexture(Program.LogoImage, 781, 248, Raylib.Fade(Color.WHITE, 0.5f));

            Raylib.DrawTextEx(Program.DefaultFont, "리듬마블 황금열쇠 Mk.2 / Golden Key Mk.2 for Kim Pyun Jip's Rhythm Marble",
                new Vector2(8, 616), 16, 0, Color.GRAY);
            Raylib.DrawTextEx(Program.DefaultFont, "Developed by BloppyHB (https://github.com/smh0505/GoldenKeyMK2)",
                new Vector2(8, 632), 16, 0, Color.GRAY);
            Raylib.DrawTextEx(Program.DefaultFont, "Logo Image by 채팅_안치는사람 & cannabee",
                new Vector2(8, 648), 16, 0, Color.GRAY);

            Raylib.DrawTextEx(Program.DefaultFont, "Copyright © 2017-2022, Eunbin Jeong (Dalgona.) <project-neodgm@dalgona.dev>", 
                new Vector2(8, 680), 16, 0, Color.GRAY);
            Raylib.DrawTextEx(Program.DefaultFont, "with reserved font name \"Neo둥근모\" and \"NeoDunggeunmo\".", 
                new Vector2(8, 696), 16, 0, Color.GRAY);
        }

        public static void DrawLoad()
        {
            Raylib.DrawRectangle(0, 0, 1280, 720, Raylib.Fade(Color.BLACK, 0.7f));
            Raylib.DrawRectangle(0, 240, 1280, 240, Color.BLACK);

            var text1 = "직전 게임 상황을 불러오시겠습니까?";
            var text2 = "\"아니요\"를 선택하시면 기본 세팅만 불러옵니다.";
            Raylib.DrawTextEx(Program.DefaultFont, text1, new Vector2(640 - Raylib.MeasureTextEx(Program.DefaultFont, text1, 48, 0).X / 2, 256), 48, 0, Color.WHITE);
            Raylib.DrawTextEx(Program.DefaultFont, text2, new Vector2(640 - Raylib.MeasureTextEx(Program.DefaultFont, text2, 32, 0).X / 2, 312), 32, 0, Color.WHITE);

            var text3 = "아니요";
            var text4 = "네";
            Raylib.DrawRectangle(448, 400, 160, 64, Raylib.Fade(Color.RED, 0.5f));
            Raylib.DrawRectangle(672, 400, 160, 64, Raylib.Fade(Color.GREEN, 0.5f));
            if (!Program.Switches["IsExiting"]) DetectCursor();
            Raylib.DrawTextEx(Program.DefaultFont, text3, new Vector2(528 - Raylib.MeasureTextEx(Program.DefaultFont, text3, 32, 0).X / 2, 416), 32, 0, Color.BLACK);
            Raylib.DrawTextEx(Program.DefaultFont, text4, new Vector2(752 - Raylib.MeasureTextEx(Program.DefaultFont, text4, 32, 0).X / 2, 416), 32, 0, Color.BLACK);
        }

        private static void DetectCursor()
        {
            if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), LoadDefault))
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    foreach (var option in Program.Setting.Values)
                        Wheel.WaitingOptions.Add(option);
                    Program.Switches["IsLoading"] = false;
                }
                else Raylib.DrawRectangle(448, 400, 160, 64, Color.RED);
            }
            if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), LoadLog))
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    foreach (var option in Program.Setting.Values)
                        Wheel.WaitingOptions.Add(option);
                    Program.Switches["IsLoading"] = false;
                }
                else Raylib.DrawRectangle(672, 400, 160, 64, Color.GREEN);
            }
        }

        private static async Task LoadPayload(string key)
        {
            HttpClient client = new HttpClient();
            using var response = await client.GetAsync("https://toon.at/widget/alertbox/" + key);
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                var line = Regex.Match(body, "\"payload\":\"[^\"]*\"").Value;
                Payload = Regex.Match(line, @"[\w]{8,}").Value;
            }
        }
    }
}