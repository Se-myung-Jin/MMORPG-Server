﻿using Google.Protobuf.Protocol;
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
        public static Dictionary<int, StatInfo> StatDict { get; private set; } = new Dictionary<int, StatInfo>();
        public static Dictionary<int, Skill> SkillDict { get; private set; } = new Dictionary<int, Skill>();
        public static Dictionary<int, ItemData> ItemDict { get; private set; } = new Dictionary<int, ItemData>();
        public static Dictionary<int, MonsterData> MonsterDict { get; private set; } = new Dictionary<int, MonsterData>();

        public static void LoadData()
        {
            StatDict = LoadJson<StatData, int, StatInfo>("StatData").MakeDict();
            SkillDict = LoadJson<SkillData, int, Skill>("SkillData").MakeDict();
            ItemDict = LoadJson<ItemLoader, int, ItemData>("ItemData").MakeDict();
            MonsterDict = LoadJson<MonsterLoader, int, MonsterData>("MonsterData").MakeDict();
        }

        static Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
        {
            string text = File.ReadAllText($"{ConfigManager.Config.dataPath}/{path}.json");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(text);
        }
    }
}
