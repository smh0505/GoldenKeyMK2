using System.Numerics;
using System.Text.RegularExpressions;
using Raylib_cs;
using Newtonsoft.Json;

namespace GoldenKeyMK2
{
    public class Ui
    {
        public static string? Payload;
        private static string _alert = "Enter 키를 눌러 연결합니다.";

        public static async void GetPassword()
        {
            int key = Raylib.GetCharPressed();
            if (key is >= 32 and <= 125) Program.Input += ((char)key).ToString();
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_BACKSPACE) && Program.Input.Length > 0) Program.Input = Program.Input.Remove(Program.Input.Length - 1);
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER))
            {
                Program.KeyProcessing = true;
                await LoadPayload(Program.Input);
                if (!string.IsNullOrEmpty(Payload)) 
                {
                    Program.CurrScreen = GameScreen.Wheel;
                    Program.Setting.Key = Program.Input;
                    StreamWriter w = new StreamWriter("default.json");
                    w.Write(JsonConvert.SerializeObject(Program.Setting));
                    w.Close();
                }
                else _alert = "연결에 실패했습니다. 다시 시도해주세요.";
                Program.KeyProcessing = false;
            }
        }

        public static void DrawConnectScreen(string input)
        {
            string secret = "".PadLeft(input.Length, '*');

            Raylib.DrawTextEx(Program.DefaultFont, "투네이션 통합 위젯 URL", new Vector2(12, 12), 32, 0, Color.BLACK);
            Raylib.DrawTextEx(Program.DefaultFont, "https://toon.at/widget/alertbox/", new Vector2(12, 44), 32, 0, Color.BLACK);

            Raylib.DrawRectangle(12, 76, 650, 40, Color.LIGHTGRAY);
            Raylib.DrawRectangleLines(12, 76, 650, 40, Color.DARKGRAY);
            Raylib.DrawTextEx(Program.DefaultFont, 
                secret.Substring(secret.Length >= 40 ? secret.Length - 40 : 0, secret.Length >= 40 ? 40 : secret.Length), 
                new Vector2(16, 80), 32, 0, Color.BLACK);

            Raylib.DrawTextEx(Program.DefaultFont, _alert, new Vector2(20, 124), 16, 0, Color.GRAY);

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