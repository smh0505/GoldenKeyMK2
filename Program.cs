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
        public static DefaultSet Setting = new DefaultSet(string.Empty, new List<string>(), new List<string>());

        public static Font DefaultFont = Raylib.LoadFont("neodgm.fnt");
        public static Texture2D LogoImage = Raylib.LoadTexture("Logo_RhythmMarble.png");

        public static Dictionary<string, bool> Switches = new Dictionary<string, bool>()
        {
            {"IsProcessing", false},
            {"TextShowing", false},
            {"IsExiting", false},
            {"IsLoading", false}
        };
        public static GameState State = GameState.Idle;

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
                if (Setting.Records.Any()) Switches["IsLoading"] = true;
                else
                {
                    Setting.Records = new List<string>();
                    foreach (var option in Setting.Values)
                        Wheel.WaitingOptions.Add(option);
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
            if (State < (GameState)2)
                if (Raylib.WindowShouldClose()) Switches["IsExiting"] = true;
                else if (Raylib.IsKeyPressed(KeyboardKey.KEY_ESCAPE))Switches["IsExiting"] = !Switches["IsExiting"];
            switch (currScreen)
            {
                case GameScreen.Connect:
                    if (!Switches["IsProcessing"] && !Switches["IsExiting"] && !Switches["IsLoading"])
                        Login.GetPassword();
                    break;
                case GameScreen.Wheel:
                    if (State < (GameState)2) Wheel.UpdateWheel();
                    if (!Switches["IsExiting"])
                    {
                        switch (State)
                        {
                            case GameState.Idle:
                                IdleControl();
                                break;
                            case GameState.Editing:
                                EditMenu.Control();
                                break;
                            case GameState.Spinning:
                                if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE)) State = GameState.Stopping;
                                break;
                        }
                    }
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
                    if (State is >= GameState.Spinning and < GameState.Saving) CalculateAngle();
                    Wheel.DrawWheel(Angle);
                    Panel.DrawPanels();
                    Panel.DrawControl();
                    Wheel.PrintOption(Angle);
                    if (State == GameState.Result) Panel.Surprise();
                    if (State == GameState.Editing) EditMenu.DrawEdit();
                    if (State == GameState.Saving) SaveMenu.DrawSave();
                    break;
            }
            if (Switches["IsLoading"]) Login.DrawLoad();
            if (Switches["IsExiting"]) ExitMenu.DrawExit();
        }

        private static void CalculateAngle()
        {
            Angle = Angle < Theta ? Angle - Theta + 360f : Angle - Theta;
            if (State == GameState.Stopping) Theta = Theta > 0 ? Theta - (float)(1 / Math.PI) : 0;
            if (Theta == 0) State = GameState.Result;
        }

        private static void IdleControl()
        {
            switch ((KeyboardKey)Raylib.GetKeyPressed())
            {
                case KeyboardKey.KEY_SPACE:
                    State = GameState.Spinning;
                    break;
                case KeyboardKey.KEY_TAB:
                    State = GameState.Editing;
                    EditMenu.ResetOptionIndex();
                    EditMenu.ResetOptionName();
                    break;
                case KeyboardKey.KEY_F2:
                    State = GameState.Saving;
                    break;
            }

            if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), Panel.EditButton))
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    State = GameState.Editing;
                    EditMenu.ResetOptionIndex();
                    EditMenu.ResetOptionName();
                }
                else Panel.EditColor = Color.GREEN;
            }
            else Panel.EditColor = Raylib.Fade(Color.GREEN, 0.5f);

            if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), Panel.SaveButton))
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) State = GameState.Saving;
                else Panel.SaveColor = Color.DARKGREEN;
            }
            else Panel.SaveColor = Raylib.Fade(Color.DARKGREEN, 0.5f);
        }
    }
}