using Newtonsoft.Json;
using Raylib_cs;

namespace GoldenKeyMK2
{
    public enum GameScreen
    {
        Connect = 0,
        Wheel
    }

    public class Program
    {
        public static string Input = string.Empty;
        public static GameScreen CurrScreen;
        public static DefaultSet Setting;

        private static void Main()
        {
            const int width = 1280;
            const int height = 720;
            Raylib.InitWindow(width, height, "황금열쇠");
            Font font = Raylib.LoadFontEx("neodgm.ttf", 32, null, 65535);
            CurrScreen = GameScreen.Connect;
            ReadFile();

            while (!Raylib.WindowShouldClose())
            {
                switch (CurrScreen)
                {
                    case GameScreen.Connect:
                        Ui.GetPassword();
                        break;
                }

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);
                switch (CurrScreen)
                {
                    case GameScreen.Connect:
                        Ui.DrawConnectScreen(font, Input);
                        break;
                    case GameScreen.Wheel:
                        Wheel.DrawWheel(0);
                        break;
                }
                Raylib.EndDrawing();
            }
            Raylib.UnloadFont(font);
            Raylib.CloseWindow();
        }

        private static void ReadFile()
        {
            if (File.Exists("default.json"))
            {
                StreamReader r = new StreamReader("default.json");
                Setting = JsonConvert.DeserializeObject<DefaultSet>(r.ReadToEnd());
                if (!string.IsNullOrEmpty(Setting.Key)) Input = Setting.Key;
                if (Setting.Values != null)foreach (string option in Setting.Values) Wheel.AddOption(option);
                r.Close();
            }
        }
    }
}