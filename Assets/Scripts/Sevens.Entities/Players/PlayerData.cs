using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Sevens.Utils;
using System.Linq;

namespace Sevens.Entities.Players
{
    public class PlayerData
    {
        public string SceneName;
        public string SpawnPointName;
        public float HP = 100f;
        public float MaxHP = 100f;
        public float Sin = 0f;
        public float MaxSin = 100f;
        public float Stamina = 0f;
        public float MaxStamina = 100f;
        public float Soul = 0f;
        public Achievements Achievements = new Achievements();
    }

    [Serializable]
    public class Achievements
    {
        public string[] Datas = new string[(int)PlayerDataKeyType.Number];

        public void SetData(PlayerDataKeyType type, string value)
        {
            Datas[(int)type] = value;
        }

        public string GetData(PlayerDataKeyType type)
            => Datas[(int)type];

        public Achievements Copy()
        {
            var ret = new Achievements();
            ret.Datas = Datas.ToArray();
            return ret;
        }
    }
}
