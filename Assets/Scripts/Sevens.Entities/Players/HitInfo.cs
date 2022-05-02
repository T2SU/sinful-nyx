using UnityEngine;
using System;

namespace Sevens.Entities.Players
{
    [Serializable]
    public class HitInfo
    {
        public float InvincibleTime;
        public Vector2 KnockbackDistance;
        public CameraShakeOption CameraShake;
    }
}
