using UnityEngine;
using System;
using System.Collections.Generic;

namespace Sevens.Entities.Players
{
    public class PlayerGuard : MonoBehaviour 
    {
        // 받을 데미지랑 깎일 스태미너 양?
        public PlayerGuardResult TryGuard(Entity source, float damage) 
        {
            var result = new PlayerGuardResult();
            return result;
        }
    }

    public struct PlayerGuardResult
    {
        public float Damage;
        public float Stamina;
        public bool Guarded;
    }
}