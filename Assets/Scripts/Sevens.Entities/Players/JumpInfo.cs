using UnityEngine;
using System;

namespace Sevens.Entities.Players
{
    [Serializable]
    public class JumpInfo
    {
        public float Power;
        public float Cooltime;
        public float StateChangeDelay;
        public float FallingSpeed;
    }
}
