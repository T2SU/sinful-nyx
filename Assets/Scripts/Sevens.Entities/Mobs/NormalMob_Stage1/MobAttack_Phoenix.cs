using Sevens.Entities.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sevens.Utils;
using UnityEngine.Serialization;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_Phoenix : MobAttackBase
    {
        [FormerlySerializedAs("Attack")]
        public GameObject AttackObj;
        public Transform AttackPosition;
        public float AttackTimeScale;
        public Transform NewPivot;

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            var pos = mob.transform.position;

            mob.PlayAnimation(new AnimationPlayOption("Attack", timeScale: AttackTimeScale), immediatelyTransition: true);
            Instantiate(AttackObj, AttackPosition);
            mob.PlayAudio("Attack");

            var dest = Mathf.Abs(NewPivot.transform.position.x - player.transform.position.x);
            dest *= mob.IsFacingLeft() ? 1 : -1;

            var seq = DOTween.Sequence()
                .Append(mob.transform.DOMoveX(pos.x + dest, AttackTimeScale))
                .AppendInterval(0.05f)
                .AppendCallback(() => mob.PlayAnimation(new AnimationPlayOption("AfterAttack", timeScale: AttackTimeScale), immediatelyTransition: true))
                .Append(mob.transform.DOMoveX(pos.x, AttackTimeScale));
            coroutines.Register("PhoenixAttack", seq);

            yield return new WaitForSeconds(AttackTimeScale * 2);
        }
    }
}