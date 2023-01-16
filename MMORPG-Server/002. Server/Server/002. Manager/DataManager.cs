using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public interface ILoader<Key, Value>
    {
        Dictionary<Key, Value> MakeDict();
    }

    public class DataManager
    {
        public static Dictionary<int, Stat> StatDict { get; private set; } = new Dictionary<int, Stat>();
        public static Dictionary<int, Skill> SkillDict { get; private set; } = new Dictionary<int, Skill>();

        public static void LoadData()
        {
            StatDict = LoadJson<StatData, int, Stat>("StatData").MakeDict();
            SkillDict = LoadJson<SkillData, int, Skill>("SkillData").MakeDict();
        }

        static Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
        {
            string text = File.ReadAllText($"{ConfigManager.Config.dataPath}/{path}.json");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(text);
        }
    }
}
