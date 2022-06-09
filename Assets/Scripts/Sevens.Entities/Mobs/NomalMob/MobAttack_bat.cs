using Sevens.Entities.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_bat : MobAttackBase
    {
        public GameObject Attack;
        public float AttackTimeScale;

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
            var obj = Instantiate(Attack, mob.transform);
            _objs.Add(obj);
            SetAllBlowSourceAs(obj, mob);

            var seq = DOTween.Sequence()
                .AppendCallback(() => mob.PlayAudio(nameof(Attack)))
                .Append(mob.transform.DOMoveX(player.transform.position.x, AttackTimeScale))
                .AppendInterval(0.05f)
                .AppendCallback(() => mob.PlayAnimation(new AnimationPlayOption("AfterAttack", timeScale: AttackTimeScale), immediatelyTransition: true))
                .Append(mob.transform.DOMoveX(pos.x, AttackTimeScale * 2));
            coroutines.Register("BatAttack", seq);

            yield return new WaitForSeconds(AttackTimeScale * 2);

            ClearObjects();
            attackManager.EndAttack(false);
        }
    }
}