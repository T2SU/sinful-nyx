using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

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

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {

        }
    }
}