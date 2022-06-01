using DG.Tweening;
using Sevens.Entities.Players;
using UnityEngine;

namespace Sevens.Entities.Mobs
{
    public class LasPhase2_Totem : LivingEntity
    {
        private BoxCollider2D _collider;
        private ParticleSystem[] _particles;
        public SpriteRenderer TotemSprite;

        public override float EntitySizeX => GetComponent<BoxCollider2D>().size.x / 2;

        public override float EntitySizeY => GetComponent<BoxCollider2D>().size.y / 2;

        public void DestroyTotem()
        {
            Hp = 0f;
            foreach (var particle in _particles)
                if (particle && particle.isPlaying)
                    particle.Stop();
            if (TotemSprite != null)
                TotemSprite.DOColor(Color.clear, 0.5f);
            Destroy(gameObject, 1);
        }

        public override void OnDamagedBy(Entity source, float damage)
        {
            if (source is Player)
            {
                if (damage <= 0 && Hp <= 0f)
                    return;
                Hp = Mathf.Max(0, Hp - damage);
                if (Hp <= 0f)
                    OnDeath();
            }
        }

        protected override void Awake()
        {
            _particles = GetComponentsInChildren<ParticleSystem>();
        }

        protected override void Start()
        {
            if (TotemSprite != null)
                TotemSprite.DOColor(Color.white, 0.5f);
        }

        protected override void OnDeath()
        {
            DestroyTotem();
        }
    }
}
