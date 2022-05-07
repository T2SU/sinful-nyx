using UnityEngine;
using System;

namespace Sevens.Entities.Players
{
    [Serializable]
    public class AttackInfo : MonoBehaviour
    {
        // 플레이어 공격에 관한 베이스 능력치
        [Header("Player Attack Status Base")]
        public Collider2D Range;
        public string Damage;
        public float StateChangeDelay;
        public float Cooltime;
        public Vector2 KnockbackDistance;

        // 플레이어 공격 관련 보정치
        [Header("Player Attack Status Multiplier")]
        public float RangeMultiplier;
        public float DamageMultiplier;
        public float CooltimeMultiplier;
        public float KnockbackMultiplier;


        private float _damage;

        public void EvaluateDamage()
        {
            _damage = int.Parse(Damage);
        }
    }
}
