using Sevens.Entities.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_Salamander : MobAttackBase
    {
        public GameObject Attack;
        public float AttackTimeScale;
        [SerializeField]
        private float _attackDelay;

        public override void Execute(Player player, MobAttackable attackManager)
        {
            var key = nameof(MobAttack_bat);
            attackManager.AttackCoroutines.Register(key, AttackTimeline(attackManager));
        }

        public override void Cancel(MobAttackable attackManager)
        {
            var mob = attackManager.Mob;
            attackManager.EndAttack(true);
            ClearObjects();
        }
        private IEnumerator AttackTimeline(MobAttackable attackManager)
        {
            var mob = attackManager.Mob;
            var coroutines = attackManager.AttackCoroutines;
            var player = attackManager.Player;
            var pos = mob.transform.position;

            mob.PlayAnimation(new AnimationPlayOption("Attack", timeScale: AttackTimeScale), immediatelyTransition: true);


            yield return new WaitForSeconds(_attackDelay);

            mob.PlayAudio(nameof(Attack));
            var obj = Instantiate(Attack, mob.transform);
            _objs.Add(obj);
            SetAllBlowSourceAs(obj, mob);

            yield return new WaitForSeconds(AttackTimeScale - _attackDelay);

            ClearObjects();
            attackManager.EndAttack(false);
        }

    }
}