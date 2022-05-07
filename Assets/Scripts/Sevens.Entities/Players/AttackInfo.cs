using UnityEngine;
using System;

namespace Sevens.Entities.Players
{
    [Serializable]
    public class AttackInfo
    {
        public float Speed; // spine timescale
        public Collider2D Range;
        public string Damage;
        public float StateChangeDelay;
        public float Cooltime;
        public Vector2 KnockbackDistance;


        private float _damage;

        public float EvalDamage()
        {
            //~~ 공식을 넣을 수 있으면 공식을 파싱하는걸 넣을거.
            _damage = int.Parse(Damage);

            return _damage;
        }
    }
}
