using UnityEngine;
using System;

namespace Sevens.Entities.Players
{
    [Serializable]
    public class DashInfo
    {
        public float MoveDistance;
        public float Duration;
        public float Stamina;
        public float StateChangeDelay;
        public float Cooltime;
    }
}
