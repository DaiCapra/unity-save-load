using Newtonsoft.Json;

namespace Code.Runtime
{
    public class Settings
    {
        public string Extension { get; set; }
        public JsonSerializerSettings JsonSettings { get; set; }

        public int AutoSaveCount { get; set; }
        public string AutoSaveName { get; set; }
        public char AutoSaveSeparator { get; set; }

        public Settings()
        {
            JsonSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
            };

            
            Extension = "sav";

            AutoSaveCount = 3;
            AutoSaveName = "autosave";
            AutoSaveSeparator = '_';
        }
    }
}