using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sevens.Entities.Players
{
    [Serializable]
    public class PlayerData : ISerializable
    {
        public float MaxHp;
        public float StaminaMax;
        public float Sin;
        public Dictionary<int, Dictionary<string, string>> QuestInfo;

        public PlayerData() {}
        protected PlayerData(SerializationInfo info, StreamingContext context)
        {
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
