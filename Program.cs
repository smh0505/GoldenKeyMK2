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
            {"IsProcessing", false},
            {"IsSpinning", false},
            {"StopTriggered", false},
            {"IsSelected", false},
            {"TextShowing", false},
            {"IsExiting", false},
            {"IsEditing", false},
            {"IsLoading", false}
        };

        public static float Angle = 180;
        public static float Theta = 50;

        public static bool Halt = false;

        private static readonly ManualResetEvent ExitEvent = new ManualResetEvent(false);

        private static void Main()
        {
            Raylib.InitWindow(1280, 720, "황금열쇠");
            Raylib.SetTargetFPS(60);
            Raylib.SetExitKey(KeyboardKey.KEY_NULL);
            CurrScreen = GameScreen.Connect;
            ReadFile();

            while (!Halt)
            {
                Process(CurrScreen);
                Raylib.BeginDrawing();
                if (Raylib.IsWindowFocused())
                {
                    Raylib.ClearBackground(Color.WHITE);
                    Screen(CurrScreen);
                }
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
                Setting.Values ??= new List<string>();
                if (Setting.Records is not null && Setting.Records.Any()) Switches["IsLoading"] = true;
                else
                {
                    Setting.Records = new List<string>();
                    Wheel.WaitingOptions.AddRange(Setting.Values);
                }
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

        private static void Process(GameScreen currScreen)
        {
            if (!Switches["IsSpinning"] && !Switches["IsSelected"] && Raylib.IsKeyPressed(KeyboardKey.KEY_ESCAPE))
                Switches["IsExiting"] = !Switches["IsExiting"];
            if (currScreen == GameScreen.Wheel && !Switches["IsExiting"] && !Switches["IsSpinning"]
                && !Switches["IsSelected"] && Raylib.IsKeyPressed(KeyboardKey.KEY_TAB))
            {
                Switches["IsEditing"] = !Switches["IsEditing"];
                EditMenu.ResetOptionIndex();
                EditMenu.ResetOptionName();
            }

            switch (currScreen)
            {
                case GameScreen.Connect:
                    if (!Switches["IsProcessing"] && !Switches["IsExiting"] && !Switches["IsLoading"])
                        Login.GetPassword();
                    break;
                case GameScreen.Wheel:
                    if (!Switches["IsSpinning"] && !Switches["IsSelected"]) Wheel.UpdateWheel();
                    if (!Switches["IsEditing"] && !Switches["IsExiting"]) Wheel.TriggerWheel();
                    if (Switches["IsEditing"] && !Switches["IsExiting"]) EditMenu.Control();
                    break;
            }
        }

        private static void Screen(GameScreen currScreen)
        {
            switch (currScreen)
            {
                case GameScreen.Connect:
                    Login.DrawConnectScreen();
                    break;
                case GameScreen.Wheel:
                    CalculateAngle();
                    Wheel.DrawWheel(Angle);
                    Panel.DrawPanels();
                    Wheel.PrintOption(Angle);
                    if (Switches["IsSelected"]) Panel.Surprise();
                    break;
            }
            if (Switches["IsEditing"]) EditMenu.DrawEdit();
            if (Switches["IsLoading"]) Login.DrawLoad();
            if (Switches["IsExiting"]) ExitMenu.DrawExit();
        }

        private static void CalculateAngle()
        {
            Angle = Wheel.RotateWheel(Angle, Theta);
            if (Switches["StopTriggered"]) Theta -= (float)(1 / Math.PI);
            if (Theta <= 0)
            {
                Switches["IsSpinning"] = false;
                Switches["IsSelected"] = true;
            }
        }
    }
}