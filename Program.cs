using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Raylib_cs;
using Websocket.Client;

namespace GoldenKeyMK2
{
    public class Program
    {
        public static string Input = string.Empty;

        public static GameScreen CurrScreen;
        public static DefaultSet Setting;

        public static Font DefaultFont = Raylib.LoadFont("neodgm.fnt");
        public static Texture2D LogoImage = Raylib.LoadTexture("Logo_RhythmMarble.png");

        public static Dictionary<string, bool> Switches = new Dictionary<string, bool>()
        {
            {"KeyProcessing", false},
            {"IsSpinning", false},
            {"StopTriggered", false},
            {"OptionSelected", false},
            {"TextShowing", false}
        };

        public static float Angle = 180;
        public static float Theta = 50;

        private static readonly ManualResetEvent ExitEvent = new ManualResetEvent(false);

        private static void Main()
        {
            Raylib.InitWindow(1280, 720, "황금열쇠");
            Raylib.SetTargetFPS(60);
            CurrScreen = GameScreen.Connect;
            ReadFile();

            while (!Raylib.WindowShouldClose())
            {
                Process(CurrScreen);
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);
                Screen(CurrScreen);
                Raylib.EndDrawing();
            }
            Raylib.UnloadFont(DefaultFont);
            Raylib.UnloadTexture(LogoImage);
            ExitEvent.Set();
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
                if (Setting.Values != null) foreach (string option in Setting.Values) Wheel.AddOption(option);
                else Setting.Values = new List<string>();
                r.Close();
            }
        }

        public static void Connect()
        {
            Uri uri = new Uri("wss://toon.at:8071/" + Login.Payload);
            using (var client = new WebsocketClient(uri))
            {
                client.MessageReceived.Subscribe(msg =>
                {
                    if (msg.ToString().Contains("roulette"))
                    {
                        var roulette = Regex.Match(msg.ToString(), "\"message\":\"[^\"]* - [^\"]*\"").Value.Substring(10);
                        var rValue = roulette.Split('-')[1].Replace("\"", "").Substring(1);
                        if (rValue != "꽝") Wheel.WaitingOptions.Add(rValue);
                    }
                });
                client.Start();
                ExitEvent.WaitOne();
            }
        }

        public static void Process(GameScreen currScreen)
        {
            switch (currScreen)
            {
                case GameScreen.Connect:
                    if (!Switches["KeyProcessing"]) Login.GetPassword();
                    break;
                case GameScreen.Wheel:
                    Wheel.UpdateWheel();
                    Wheel.TriggerWheel();
                    break;
            }
        }

        public static void Screen(GameScreen currScreen)
        {
            switch (currScreen)
            {
                case GameScreen.Connect:
                    Login.DrawConnectScreen(Input);
                    break;
                case GameScreen.Wheel:
                    Angle = Wheel.RotateWheel(Angle, Theta);
                    if (Switches["StopTriggered"]) Theta -= (float)(1 / Math.PI);
                    Wheel.DrawWheel(Angle);
                    Panel.DrawPanels();
                    if (Theta <= 0)
                    {
                        Switches["IsSpinning"] = false;
                        Switches["OptionSelected"] = true;
                    }
                    Wheel.PrintOption(Angle);
                    if (Switches["OptionSelected"]) Panel.Surprise();
                    break;
            }
        }
    }
}