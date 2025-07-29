using System.Collections.Generic;

namespace DeathFortUnoCard.Scripts.Common.Loaders
{
    [System.Serializable]
    public class TextSection
    {
        public string name;
        public List<TextEntry> entries = new();

        public TextSection(string name)
        {
            this.name = name;
        }
    }
}