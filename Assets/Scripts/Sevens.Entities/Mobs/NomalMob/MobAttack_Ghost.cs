using Sevens.Entities.Players;
using System.Collections;
using UnityEngine;
using Sevens.Utils;
using UnityEngine.Serialization;

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

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            var pos = mob.transform.position;
            mob.PlayAnimation(new AnimationPlayOption("Attack", timeScale: AttackTimeScale), immediatelyTransition: true);

            yield return new WaitForSeconds(_upDuration);

            mob.PlayAudio("Attack");
            Instantiate(AttackObj, AttackPosition.transform);

            yield return new WaitForSeconds(AttackTimeScale - _upDuration);
        }
    }
}