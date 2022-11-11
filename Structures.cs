using Raylib_cs;

namespace GoldenKeyMK2
{
    public enum GameScreen
    {
        Connect = 0,
        Wheel
    }

    public struct Option
    {
        public string Name;
        public Color Color;
        public int Count;

        public Option(string name, Color color, int count)
        {
            this.Name = name;
            this.Color = color;
            this.Count = count;
        }
    }

    public struct DefaultSet
    {
        public string Key;
        public List<string> Values;
        public List<string> Records;

        public DefaultSet(string key, List<string> values, List<string> records)
        {
            this.Key = key;
            this.Values = values;
            this.Records = records;
        }
    }
}