using Sevens.Utils;
using System;
using UnityEngine;

namespace Sevens.Entities
{
    public class Blow : MonoBehaviour
    {
        [Serializable]
        public class AoEDamageOption
        {
            public bool Enabled;
            public float Damage;
            public float StartAffectingDelay;
            public float DelayTime = 2f;
            public float MaxAffectingDuration;
        }

        [Serializable]
        public class OnceDamageOption
        {
            public bool Enabled;
            public float Damage;
        }

        public Entity Source;

        [field: SerializeField]
        public AoEDamageOption AOEOption { get; private set; }

        [field: SerializeField]
        public OnceDamageOption OnceOption { get; private set; }

        [field: SerializeField]
        public LayerMask DamageLayer { get; private set; }

        [SerializeField]
        private float _destoryTime;

        private float _nextAoEDamageTime;
        private float _maxAoEDamageTime;
        private Collider2D _collider;
        private ContactFilter2D _contactFilter;
        private bool _onceAttacked;

        public void ApplyOnceDamage(Component other)
        {
            if (!OnceOption.Enabled)
                return;
            if (_onceAttacked)
                return;
            if (!other.gameObject.IsLayerMatched(DamageLayer))
                return;
            var targetEntity = other.GetComponent<LivingEntity>();
            if (targetEntity != null)
            {
                _onceAttacked = true;
                targetEntity.OnDamagedBy(Source, OnceOption.Damage);
                Debug.Log($"[Blow] Send once hit to {other.name} damage {OnceOption.Damage}");
            }
        }

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _contactFilter = new ContactFilter2D() { useLayerMask = true, layerMask = DamageLayer };
            _onceAttacked = false;
        }

        private void OnEnable()
        {
            if (_destoryTime >= 0)
                Destroy(transform.gameObject, _destoryTime);
            _nextAoEDamageTime = Time.time + AOEOption.StartAffectingDelay;
            _maxAoEDamageTime = _nextAoEDamageTime + AOEOption.MaxAffectingDuration;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            ApplyOnceDamage(other);
        }

        private void Update()
        {
            TryAffectInArea();
        }

        private void TryAffectInArea()
        {
            if (!AOEOption.Enabled)
                return;
            if (!IsTimingForAffectingArea())
                return;
            var colliders = new Collider2D[1];
            var hits = Physics2D.OverlapCollider(_collider, _contactFilter, colliders);
            if (hits == 0)
                return;
            foreach (var other in colliders)
            {
                var pivot = other.transform.GetLowestPivot();
                //if (pivot != null)
                //{
                //    Debug.DrawLine(pivot.Value, pivot.Value + new Vector2(0.5f, 0f), Color.white);
                //    Debug.DrawLine(pivot.Value, pivot.Value + new Vector2(-0.5f, 0f), Color.white);
                //    Debug.DrawLine(pivot.Value, pivot.Value + new Vector2(0f, 0.5f), Color.white);
                //    Debug.DrawLine(pivot.Value, pivot.Value + new Vector2(0f, -0.5f), Color.white);
                //    Debug.Break();
                //}
                if (pivot == null || !_collider.OverlapPoint(pivot.Value))
                {
                    Debug.Log($"No player in area");
                    continue;
                }
                ApplyAoEDamage(other);
            }
        }

        private void ApplyAoEDamage(Component other)
        {
            LivingEntity targetEntity = other.GetComponent<LivingEntity>();
            if (targetEntity != null)
            {
                targetEntity.OnDamagedBy(Source, AOEOption.Damage);
                Debug.Log($"[Blow] Send aoe hit to {other.name} damage {AOEOption.Damage}");
            }
        }

        private bool IsTimingForAffectingArea()
        {
            if (_maxAoEDamageTime <= Time.time)
                return false;
            if (_nextAoEDamageTime > Time.time)
                return false;
            _nextAoEDamageTime = Time.time + AOEOption.DelayTime;
            return true;
        }

        public static void SetAllBlowSourceAs(GameObject attackObject, Entity source)
        {
            foreach (var blow in attackObject.GetComponentsInChildren<Blow>())
                blow.Source = source;
        }
    }
}