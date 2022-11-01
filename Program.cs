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
        public static bool KeyProcessing = false;
        public static bool IsSpinning = false;
        public static bool StopTriggered = false;
        public static float Angle = 0;
        public static float Theta = 50;

        private static void Main()
        {
            const int width = 1280;
            const int height = 720;
            Raylib.InitWindow(width, height, "황금열쇠");
            Raylib.SetTargetFPS(60);
            Font font = Raylib.LoadFontEx("neodgm.ttf", 32, null, 65535);
            CurrScreen = GameScreen.Connect;
            ReadFile();

            while (!Raylib.WindowShouldClose())
            {
                switch (CurrScreen)
                {
                    case GameScreen.Connect:
                        if (!KeyProcessing) Ui.GetPassword();
                        break;
                    case GameScreen.Wheel:
                        if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
                        {
                            if (!IsSpinning) IsSpinning = true;
                            else if (!StopTriggered) StopTriggered = true;
                        }
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
                        Raylib.DrawFPS(8, 8);
                        if (IsSpinning) Angle += Theta;
                        if (StopTriggered) Theta -= (float)(1 / Math.PI);
                        Wheel.DrawWheel(Angle);
                        Panel.DrawPanels(font);
                        if (Theta <= 0) 
                        {
                            IsSpinning = false;
                            StopTriggered = false;
                            Theta = 50;
                        }
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
                var data = r.ReadToEnd();
                if (!string.IsNullOrEmpty(data)) Setting = JsonConvert.DeserializeObject<DefaultSet>(data);
                if (!string.IsNullOrEmpty(Setting.Key)) Input = Setting.Key;
                if (Setting.Values != null)foreach (string option in Setting.Values) Wheel.AddOption(option);
                r.Close();
            }
        }
    }
}