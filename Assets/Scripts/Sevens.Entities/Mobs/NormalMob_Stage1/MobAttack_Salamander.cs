using Sevens.Entities.Players;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Sevens.Utils;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_Salamander : MobAttackBase
    {
        [FormerlySerializedAs("Attack")]
        public GameObject AttackObj;

        public float AttackTimeScale;
        [SerializeField]
        private float _attackDelay;

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            mob.PlayAnimation(new AnimationPlayOption("Attack", timeScale: AttackTimeScale), immediatelyTransition: true);

            yield return new WaitForSeconds(_attackDelay);

            mob.PlayAudio("Attack");
            Instantiate(AttackObj, mob.transform);

            yield return new WaitForSeconds(AttackTimeScale - _attackDelay);
        }
    }
}