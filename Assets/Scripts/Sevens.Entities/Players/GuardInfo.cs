using UnityEngine;
using System;

namespace Sevens.Entities.Players
{
    [Serializable]
    public class GuardInfo
    {
        public float Duration;
        public float Stamina;
        public float StateChangeDelay;
        public float Cooltime;
        public float ReduceDamage;
        public float IncreaseUltGaugeOnParrying;
    }
}
