namespace DeathFortUnoCard.Scripts.Common.Loaders
{
    [System.Serializable]
    public struct TextEntry
    {
        public string id;
        public string text;

        public TextEntry(string id, string text)
        {
            this.id = id;
            this.text = text;
        }
    }
}