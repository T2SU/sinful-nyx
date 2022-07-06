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
        public float AttackTimeScale;

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            var pos = mob.transform.position;

            mob.PlayAnimation(new AnimationPlayOption("Attack", timeScale: AttackTimeScale), immediatelyTransition: true);
            Instantiate(AttackObj, mob.transform);
            mob.PlayAudio("Attack");

            var seq = DOTween.Sequence()
                .Append(mob.transform.DOMove(player.transform.position + Vector3.up * 2, AttackTimeScale))
                .AppendInterval(0.05f)
                .AppendCallback(() => mob.PlayAnimation(new AnimationPlayOption("AfterAttack", timeScale: AttackTimeScale), immediatelyTransition: true))
                .Append(mob.transform.DOMove(pos, AttackTimeScale));
            coroutines.Register("PhoenixAttack", seq);

            yield return new WaitForSeconds(AttackTimeScale * 2);
        }
    }
}