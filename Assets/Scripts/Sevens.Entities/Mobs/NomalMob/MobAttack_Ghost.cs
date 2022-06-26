using Sevens.Entities.Players;
using System.Collections;
using UnityEngine;
using Sevens.Utils;
using UnityEngine.Serialization;
using UnityEngine.VFX;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_Ghost : MobAttackBase
    {
        [FormerlySerializedAs("Attack")]
        public GameObject AttackObj;
        public GameObject AttackPosition;
        public float AttackTimeScale;

        [SerializeField]
        private float _upDuration;
        [SerializeField]
        private VisualEffect _attackVFX; 

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            var pos = mob.transform.position;
            mob.PlayAnimation(new AnimationPlayOption("Attack", timeScale: AttackTimeScale), immediatelyTransition: true);
            _attackVFX.Play();

            yield return new WaitForSeconds(_upDuration);

            mob.PlayAudio("Attack");
            Instantiate(AttackObj, AttackPosition.transform);
            _attackVFX.Stop();

            yield return new WaitForSeconds(AttackTimeScale - _upDuration);
        }
    }
}