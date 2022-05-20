using Sevens.Cameras;
using UnityEngine;
using UnityEngine.Events;

namespace Sevens.Entities
{
    public abstract class LivingEntity : Entity
    {
        [SerializeField]
        private float _hp;

        public float Hp
        {
            get => _hp;
            set
            {
                _hp = value;
                OnHpRatioChanged?.Invoke(HpRatio);
            }
        }

        [field: SerializeField]
        public float MaxHp { get; protected set; }

        public float HpRatio => (float)Hp / MaxHp;

        public UnityEvent<float> OnHpRatioChanged;

        public UnityEvent<float> OnHpRatioInitialSet;

        public abstract float EntitySizeX { get; }

        public abstract float EntitySizeY { get; }

        public abstract void OnDamagedBy(Entity source, float damage);

        public virtual void SetInitialHp(float hp)
        {
            _hp = hp;
            OnHpRatioInitialSet?.Invoke(HpRatio);
        }

        public virtual void Heal(float delta)
        {
            Hp += delta;
            if (Hp > MaxHp)
                Hp = MaxHp;
        }

        protected abstract void OnDeath();
    }
}