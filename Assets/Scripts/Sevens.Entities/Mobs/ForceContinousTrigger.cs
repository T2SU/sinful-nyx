using UnityEngine;

namespace Sevens.Entities.Mobs
{
    [RequireComponent(typeof(Blow))]
    public class ForceContinousTrigger : MonoBehaviour
    {
        private Collider2D _collider;
        private Blow _blow;
        private ContactFilter2D _contactFilter;

        private bool _givenDamage;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _blow = GetComponent<Blow>();
            _contactFilter = new ContactFilter2D() { useLayerMask = true, layerMask = _blow.DamageLayer };
        }

        private void FixedUpdate()
        {
            if (_givenDamage)
                return;
            var hits = new Collider2D[1];
            if (_collider.OverlapCollider(_contactFilter, hits) == 0)
            {
                Debug.Log($"No triggered player");
                return;
            }
            var living = hits[0].GetComponent<LivingEntity>();
            if (living != null)
            {
                living.OnDamagedBy(_blow.Source, _blow.OnceOption.Damage);
                _givenDamage = true;
            }
        }
    }
}
