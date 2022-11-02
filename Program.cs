﻿using System.Text.RegularExpressions;
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

        public static Font DefaultFont = Raylib.LoadFontEx("neodgm.ttf", 32, null, 65535);
        public static Texture2D LogoImage = Raylib.LoadTexture("Logo_RhythmMarble.png");

        public static bool KeyProcessing = false;
        public static bool IsSpinning;
        public static bool StopTriggered;
        public static bool OptionSelected;

        public static float Angle = 180;
        public static float Theta = 50;

        public static Thread DonationThread = new Thread(Connect);
        private static readonly ManualResetEvent ExitEvent = new ManualResetEvent(false);

        private static void Main()
        {
            Raylib.InitWindow(1280, 720, "황금열쇠");
            Raylib.SetTargetFPS(60);
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
                        if (!IsSpinning && !OptionSelected && Wheel.WaitingOptions.Count > 0)
                        {
                            foreach (var option in Wheel.WaitingOptions) Wheel.AddOption(option);
                            Wheel.WaitingOptions.Clear();
                        }
                        if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
                        {
                            if (!IsSpinning && !OptionSelected) IsSpinning = true;
                            else if (!StopTriggered) StopTriggered = true;
                        }
                        break;
                }

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);
                switch (CurrScreen)
                {
                    case GameScreen.Connect:
                        Ui.DrawConnectScreen(Input);
                        break;
                    case GameScreen.Wheel:
                        Raylib.DrawFPS(8, 8);
                        Angle = Wheel.RotateWheel(Angle, Theta);
                        if (StopTriggered) Theta -= (float)(1 / Math.PI);
                        Wheel.DrawWheel(Angle);
                        Panel.DrawPanels();
                        if (Theta <= 0)
                        {
                            IsSpinning = false;
                            OptionSelected = true;
                        }
                        Wheel.PrintOption(Angle);
                        if (OptionSelected) Panel.Surprise();
                        break;
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
                if (Setting.Values != null)foreach (string option in Setting.Values) Wheel.AddOption(option);
                r.Close();
            }
        }

        public static void Connect()
        {
            Uri uri = new Uri("wss://toon.at:8071/" + Ui.Payload);
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
    }
}